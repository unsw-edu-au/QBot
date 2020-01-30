using AdaptiveCards;
using Antares.Bot.Channels;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;
using Microsoft.Teams.Apps.QBot.Bot.Services;
using Microsoft.Teams.Apps.QBot.Bot.utility;
using Microsoft.Teams.Apps.QBot.Model;
using Microsoft.Teams.Apps.QBot.Model.Teams;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.Teams.Apps.QBot.Bot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private ResourceService resourceService;

        public RootDialog()
        {
            resourceService = new ResourceService();
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            // Init
            var activity = await result as Microsoft.Bot.Connector.Activity;

            // Handle tagged in a question
            // Check if we already have this particular conversation saved. This is to counter the auto-tagging when replying to a bot in Teams
            if (SQLService.DoesConversationIdExist(activity.Conversation.Id))
            {
                // Do nothing if we find it.
            }
            else
            {
                // Check if this is a group (channel) or a 1-on-1 conversation
                if (activity.Conversation.IsGroup == true)
                {
                    await HandleChannelConversation(context, activity);
                }
                else
                {
                    await HandleOneOnOneConversation(activity);
                }
            }

        }

        private async Task HandleOneOnOneConversation(Microsoft.Bot.Connector.Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            // One-on-one chat isn't support yet, encourage to post question in the channel instead
            var defaultReply = activity.CreateReply("Please post your question in the channel instead -- and don't forget to tag me, so I know about it!");
            await connector.Conversations.ReplyToActivityAsync(defaultReply);
        }

        private async Task HandleChannelConversation(IDialogContext context, Microsoft.Bot.Connector.Activity activity)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            // Channel conversation

            // Get the question text, and team and channel details
            var question = activity.Text;
            var messageId = activity.Id;
            var conversationId = activity.Conversation.Id;
            var channelData = activity.GetChannelData<TeamsChannelData>();
            string tenantId = channelData.Tenant.Id;
            string teamId = channelData.Team.Id;
            string channelId = channelData.Channel.Id;

            TeamDetails teamDetails = connector.GetTeamsConnectorClient().Teams.FetchTeamDetails(teamId);
            var teamName = teamDetails.Name;
            var groupId = teamDetails.AadGroupId;

            ConversationList channelList = connector.GetTeamsConnectorClient().Teams.FetchChannelList(teamId);
            var channel = channelList.Conversations.Where(x => x.Id == channelId).FirstOrDefault();
            var channelName = channel?.Name;
            var topic = channelName ?? "General";

            IList<ChannelAccount> members = await connector.Conversations.GetConversationMembersAsync(teamId);
            var teamsMembers = members.AsTeamsChannelAccounts();

            // Get original poster
            var originalPosterAsTeamsChannelAccount = teamsMembers.Where(x => x.ObjectId == activity.From.Properties["aadObjectId"].ToString()).FirstOrDefault();
            var userUpn = originalPosterAsTeamsChannelAccount.UserPrincipalName;
            var user = SQLService.GetUser(userUpn);

            // strip @bot mention for teams
            if (activity.ChannelId == "msteams")
            {
                activity = MicrosoftTeamsChannelHelper.StripAtMentionText(activity);
            }
            else
            {
                activity.Text = activity.Text.Trim();
            }

            // strip the html tags
            question = MicrosoftTeamsChannelHelper.StripHtmlTags(activity.Text);

            var questionModel = new QuestionModel()
            {
                ID = 0,
                TenantId = tenantId,
                GroupId = groupId,
                TeamId = teamId,
                TeamName = teamName,
                ConversationId = conversationId,
                MessageId = messageId,
                Topic = topic,
                QuestionText = question,
                QuestionSubmitted = DateTime.Now,
                OriginalPoster = user,
                Link = CreateLink(conversationId, tenantId, groupId, messageId, teamName, topic),
            };

            if (string.IsNullOrEmpty(question))
            {
                await HandleNoQuestion(context, activity, questionModel, channelId);
            }
            else
            {
                // get courseID
                var courseID = SQLService.GetCourseIDByName(teamName.Trim());
                var course = SQLService.GetCourse(courseID);
                questionModel.CourseID = courseID;
                await HandleQuestionWorkflow(context, activity, course, questionModel);
            }
        }

        private async Task HandleQuestionWorkflow(IDialogContext context, Microsoft.Bot.Connector.Activity activity, Data.Course course, QuestionModel questionModel)
        {
            var predictiveQnAService = new PredictiveQnAService(course.Id);
            QnAAnswer response = await predictiveQnAService.GetAnswer(questionModel.QuestionText);

            // if result, check confidence level
            if (response != null && response.Score > Convert.ToDouble(course.PredictiveQnAConfidenceThreshold))
            {
                await HandleBotAnswerWorkflow(context, activity, questionModel, response, questionModel.TeamId);
            }
            else
            {
                // a score of 0 or a score below threshold both result in regular workflow
                await HandleTagAdminWorkflow(activity, questionModel, questionModel.TeamId, course.Id);
            }
        }

        private async Task HandleBotAnswerWorkflow(IDialogContext context, Microsoft.Bot.Connector.Activity activity, QuestionModel questionModel, QnAAnswer response, string teamId)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            // above threshold
            // post answer card with helpful/not-helpful

            // Set question state
            questionModel.QuestionStatus = Constants.QUESTION_STATUS_UNANSWERED;

            // Save question
            var questionId = SQLService.CreateOrUpdateQuestion(questionModel);

            var attachment = CreateBotAnswerCard(response.Id, response.Answer, response.Score, questionId, questionModel.OriginalPoster.Email);

            var reply = activity.CreateReply();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply);
        }

        private async Task HandleTagAdminWorkflow(Microsoft.Bot.Connector.Activity activity, QuestionModel questionModel, string teamId, int courseId)
        {
            var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            // Tag admin workflow
            IList<ChannelAccount> members = await connector.Conversations.GetConversationMembersAsync(teamId);
            var teamsMembers = members.AsTeamsChannelAccounts().ToList();

            // Get original poster
            var originalPosterAsTeamsChannelAccount = teamsMembers.Where(x => x.ObjectId == activity.From.Properties["aadObjectId"].ToString()).FirstOrDefault();
            var studentUPN = originalPosterAsTeamsChannelAccount.UserPrincipalName;
            var mappedStudentCourseRole = SQLService.GetUser(studentUPN);

            // Set question state
            var questionStatus = Constants.QUESTION_STATUS_UNANSWERED;

            // Save question
            questionModel.QuestionStatus = questionStatus;
            questionModel.OriginalPoster = mappedStudentCourseRole;
            var questionId = SQLService.CreateOrUpdateQuestion(questionModel);
            questionModel.ID = questionId;

            // TagAdmins
            var mentionOnlyReply = activity.CreateReply();
            var adminsOnTeams = GetAdminChannelAccountsToTag(activity, teamId, courseId, teamsMembers, mappedStudentCourseRole);

            if (adminsOnTeams != null && adminsOnTeams.Count > 0)
            {

                foreach (var admin in adminsOnTeams)
                {
                    mentionOnlyReply.AddMentionToText(admin, MentionTextLocation.AppendText);
                }

                var r1 = await connector.Conversations.ReplyToActivityAsync(mentionOnlyReply);

                var reply = activity.CreateReply();
                var attachment = CreateUserAnswerCard(questionId);
                reply.Attachments.Add(attachment);
                var r2 = await connector.Conversations.ReplyToActivityAsync(reply);

                questionModel.AnswerCardActivityId = r2?.Id;
                SQLService.CreateOrUpdateQuestion(questionModel);
            } 
            else
            {
                var reply = activity.CreateReply();
                reply.Text = "I'm sorry, I could not find anyone to tag. Please ensure the user has been assigned a tutorial group with atleast one demonstrator.";
                var r2 = await connector.Conversations.ReplyToActivityAsync(reply);
            }
        }

        private async Task HandleNoQuestion(IDialogContext context, Microsoft.Bot.Connector.Activity activity, QuestionModel questionModel, string channelId)
        {
            var messageId = questionModel.MessageId;
            // We stripped the @mention and html tags. If nothing remains then this means the user forgot to tag the bot in the original question and tagged it in a reply, so we need to handle it
            // Get the ID of the parent message (which should be the root)
            var thisTeamsMessage = await GetMessage(questionModel.GroupId, channelId, messageId);

            // check for null below
            if (thisTeamsMessage == null)
            {
                // Get root message
                var rootTeamsMessage = await GetRootMessage(questionModel.GroupId, channelId, messageId, questionModel.ConversationId);
                var question = MicrosoftTeamsChannelHelper.StripHtmlTags(rootTeamsMessage.Body.Content);
                messageId = rootTeamsMessage.Id;
                var conversationId = channelId + @";messageid=" + messageId;

                questionModel.QuestionText = question;
                questionModel.MessageId = messageId;
                questionModel.ConversationId = conversationId;

                // Get original poster
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                IList<ChannelAccount> members = await connector.Conversations.GetConversationMembersAsync(questionModel.TeamId);
                var teamsMembers = members.AsTeamsChannelAccounts();
                var originalPosterAsTeamsChannelAccount = teamsMembers.Where(x => x.ObjectId == rootTeamsMessage.From.User.Id).FirstOrDefault();
                var userUpn = originalPosterAsTeamsChannelAccount.UserPrincipalName;
                var user = SQLService.GetUser(userUpn);

                // Handle if the bot gets tagged again in the same set of replies
                if (SQLService.DoesConversationIdExist(conversationId))
                {
                    // WE DO NOTHING!
                }
                else
                {
                    // get courseID
                    var courseID = SQLService.GetCourseIDByName(questionModel.TeamName.Trim());
                    var course = SQLService.GetCourse(courseID);
                    questionModel.CourseID = courseID;
                    await HandleQuestionWorkflow(context, activity, course, questionModel);
                }
            }
            else
            {
                // Empty was the root, which means the user simply forgot to ask a question.
                await context.PostAsync($"If you have a question, please include it as part of your message.");
            }
        }

        private List<ChannelAccount> GetAdminChannelAccountsToTag(Microsoft.Bot.Connector.Activity activity, string teamId, int courseID, List<TeamsChannelAccount> teamsMembers, UserCourseRoleMappingModel mappedStudentCourseRole)
        {
            var adminsOnTeams = new List<ChannelAccount>();
            var tutorialAdmins = new List<UserCourseRoleMappingModel>();

            if (mappedStudentCourseRole != null)
            {
                if (mappedStudentCourseRole.Role != null && mappedStudentCourseRole.Role.Name != Constants.STUDENT_ROLE)
                {
                    // Not a student - notify lecturer
                    tutorialAdmins = SQLService.GetUsersByRole(Constants.LECTURER_ROLE, courseID);
                }
                else
                {
                    // Is a student
                    if (mappedStudentCourseRole.TutorialGroups != null && mappedStudentCourseRole.TutorialGroups.Count > 0)
                    {
                        // Get tutorial demonstrators
                        // Only get the tutorial groups for this course
                        foreach (var tutorialGroup in mappedStudentCourseRole.TutorialGroups.Where(x=>x.CourseId == courseID))
                        {
                            var demonstrators = SQLService.GetDemonstrators(courseID, tutorialGroup.ID);
                            if (demonstrators != null)
                            {
                                tutorialAdmins.AddRange(demonstrators);
                            }
                        }
                    }
                    else
                    {
                        // student without tutorial class
                        tutorialAdmins = SQLService.GetAllAdmins(courseID).Distinct().ToList();
                    }
                }

                // Find channel accounts
                if (tutorialAdmins != null && tutorialAdmins.Count > 0)
                {
                    foreach (var admin in tutorialAdmins)
                    {
                        var adminOnTeams = teamsMembers.Where(x =>
                            (x.Email == admin.Email || x.Email == admin.UserName || x.UserPrincipalName == admin.UserName || x.UserPrincipalName == admin.Email) &&
                            (x.Email != mappedStudentCourseRole.Email && x.Email != mappedStudentCourseRole.UserName && x.UserPrincipalName != mappedStudentCourseRole.UserName && x.UserPrincipalName != mappedStudentCourseRole.Email)
                        ).FirstOrDefault();
                        if (adminOnTeams != null)
                        {
                            adminsOnTeams.Add(adminOnTeams);
                        }
                    }
                }
            }
            else
            {
                // User not in database
                // Notify lecturer
                var lecturers = SQLService.GetUsersByRole(Constants.LECTURER_ROLE, courseID);

                foreach (var admin in lecturers)
                {
                    var adminOnTeams = teamsMembers.Where(x =>
                        (x.Email == admin.Email || x.Email == admin.UserName || x.UserPrincipalName == admin.UserName || x.UserPrincipalName == admin.Email)
                    ).FirstOrDefault();

                    if (adminOnTeams != null)
                    {
                        adminsOnTeams.Add(adminOnTeams);
                    }
                }
            }


            if (adminsOnTeams != null && adminsOnTeams.Count > 0)
            {

                // Avoid tagging same person twice if they are part of multiple tutorial groups
                var distinct = adminsOnTeams
                    .GroupBy(p => p.Id)
                    .Select(g => g.First())
                    .ToList();

                return distinct;
            } 
            else
            {
                return null;
            }
        }

        private Attachment CreateBotAnswerCard(int qnaId, string answerText, double confidenceScore, int questionId, string userUpn)
        {
            var card = new AdaptiveCard();

            var title = new AdaptiveTextBlock("'" + answerText.ReplaceLinksWithMarkdown() + "'");
            title.Weight = AdaptiveTextWeight.Bolder;
            title.Size = AdaptiveTextSize.Medium;
            title.Wrap = true;

            var confidence = new AdaptiveTextBlock("I think that is the answer (confidence score of " + confidenceScore.ToString("0.##") + "%)");
            confidence.Wrap = true;

            card.Body.Add(title);
            card.Body.Add(confidence);

            var actionHelpfulJson = "{\"type\":\"" + Constants.ACTIVITY_BOT_HELPFUL + "\",\"questionId\": \"" + questionId + "\", \"answer\": \"" + answerText + "\", \"qnaId\": \"" + qnaId + "\"}";
            var actionUnhelpfulJson = "{\"type\":\"" + Constants.ACTIVITY_BOT_NOT_HELPFUL + "\",\"questionId\": \"" + questionId + "\" ,\"userUpn\": \"" + userUpn + "\"}";


            card.Actions.AddRange(new List<AdaptiveAction>()
                    {
                        new AdaptiveSubmitAction() { Type = AdaptiveSubmitAction.TypeName, Title = "This is helpful", DataJson = actionHelpfulJson },
                        new AdaptiveSubmitAction() { Type = AdaptiveSubmitAction.TypeName, Title = this.resourceService.GetValueFor("TagAdmins"), DataJson =  actionUnhelpfulJson },
                    });

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card,
            };

            return attachment;
        }

        private Attachment CreateUserAnswerCard(int questionId)
        {
            var actionJson = "{\"type\":\"" + Constants.ACTIVITY_SELECT_ANSWER + "\",\"questionId\": \"" + questionId + "\"}";

            var card = new HeroCard()
            {
                Buttons = new List<CardAction>()
                        {
                            new CardAction(ActivityTypes.Invoke, "Select the answer", value: actionJson),
                        },
            };

            return card.ToAttachment();
        }

        private string CreateLink(string conversationIdString, string tenantId, string groupId, string messageId, string teamName, string channelName)
        {
            var conversationId = conversationIdString.Split(';')[0];
            var linkBuilder = new StringBuilder(string.Empty);

            linkBuilder.Append(@"https://teams.microsoft.com/l/message/" + conversationId);
            linkBuilder.Append(@"/" + messageId);
            linkBuilder.Append(@"?tenantId=" + tenantId);
            linkBuilder.Append(@"&groupId=" + groupId);
            linkBuilder.Append(@"&parentMessageId=" + messageId);
            linkBuilder.Append(@"&teamName=" + HttpUtility.UrlEncode(teamName));
            linkBuilder.Append(@"&channelName=" + HttpUtility.UrlEncode(channelName));
            linkBuilder.Append(@"&createdTime=" + messageId);

            return linkBuilder.ToString();
        }

        private async Task<TeamsMessage> GetRootMessage(string teamsId, string channelId, string messageId, string conversationId)
        {
            // Get root message id from conversationid
            var rootMessageId = conversationId.Split(';')[1].Split('=')[1];

            var authority = ServiceHelper.Authority;
            var resource = ServiceHelper.GraphResource;

            var authService = new AuthService(authority);
            var authResult = await authService.AuthenticateSilently(resource);

            var graphService = new GraphService();
            var teamsMessage = await graphService.GetMessage(authResult.AccessToken, teamsId, channelId, rootMessageId);

            return teamsMessage;
        }

        private async Task<TeamsMessage> GetMessage(string teamsId, string channelId, string messageId)
        {
            var authority = ServiceHelper.Authority;
            var resource = ServiceHelper.GraphResource;

            var authService = new AuthService(authority);
            var authResult = await authService.AuthenticateSilently(resource);

            var graphService = new GraphService();
            var teamsMessage = await graphService.GetMessage(authResult.AccessToken, teamsId, channelId, messageId);

            return teamsMessage;
        }

    }
}