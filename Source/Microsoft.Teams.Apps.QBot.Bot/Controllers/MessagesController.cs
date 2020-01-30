using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

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
using Newtonsoft.Json.Serialization;

namespace Microsoft.Teams.Apps.QBot.Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Microsoft.Bot.Connector.Activity activity)
        {
            var activityType = GetActivityTypeFrom(activity);

            if (activityType == ActivityTypes.Message)
            {
                try
                {
                    await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                }
            }
            else if (activityType == ActivityTypes.Invoke)
            {
                return await HandleInvokeMessages(activity);
            }
            else
            {
                await HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<HttpResponseMessage> HandleInvokeMessages(Microsoft.Bot.Connector.Activity message)
        {
            string invokeType = (message.Value as dynamic)["type"];
            string command = (message.Value as dynamic)["commandId"];

            if (invokeType == Constants.ACTIVITY_SELECT_ANSWER)
            {
                await HandleSelectAnswerFromCard(message);
            }
            else if (invokeType == Constants.ACTIVITY_BOT_HELPFUL)
            {
                await HandleBotHelpful(message);
            }
            else if (invokeType == Constants.ACTIVITY_BOT_NOT_HELPFUL)
            {
                await HandleBotNotHelpful(message);
            }
            else if (invokeType == Constants.ACTIVITY_MARKED_ANSWERED)
            {
                await HandleMarkedAnswerFromCard(message);
            }
            else
            {
                if (command == Constants.ACTIVITY_SELECT_ANSWER)
                {
                    return await HandleSelectAnswerFromMessageAction(message);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private async Task HandleBotHelpful(Microsoft.Bot.Connector.Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            // Question has been answered
            // store answer in db.

            // get question details
            int questionId = Convert.ToInt32((activity.Value as dynamic)["questionId"]);
            string answer = (activity.Value as dynamic)["answer"];
            int answerId = Convert.ToInt32((activity.Value as dynamic)["qnaId"]);
            var question = SQLService.GetQuestion(questionId);

            // get user details
            var channelData = activity.GetChannelData<Microsoft.Bot.Connector.Teams.Models.TeamsChannelData>();
            var members = await connector.Conversations.GetConversationMembersAsync(channelData.Team.Id);

            var teamsMembers = members.AsTeamsChannelAccounts();
            var currentUser = teamsMembers.Where(x => x.ObjectId == activity.From.Properties["aadObjectId"].ToString()).FirstOrDefault();
            var studentUPN = currentUser.UserPrincipalName;
            var isUserAdmin = SQLService.IsUserAdmin(studentUPN);

            // store question in qna
            string teamId = channelData.Team.Id;
            TeamDetails teamDetails;
            try
            {
                teamDetails = connector.GetTeamsConnectorClient().Teams.FetchTeamDetails(teamId);
            }
            catch (Exception e)
            {
                teamDetails = connector.GetTeamsConnectorClient().Teams.FetchTeamDetails(teamId);
                Trace.TraceError(e.ToString());
            }

            var teamName = teamDetails.Name;
            var courseID = SQLService.GetCourseIDByName(teamName.Trim());

            // check if user can set question status
            if (isUserAdmin || (studentUPN == question.OriginalPoster.Email) || (studentUPN == question.OriginalPoster.UserName)) // double check question to course? not sure
            {
                var answeredBy = SQLService.GetBotUser(courseID);

                // update question model
                question.QuestionStatus = Constants.QUESTION_STATUS_ANSWERED;
                question.QuestionAnswered = DateTime.Now;
                question.AnswerText = answer;
                question.AnswerPoster = answeredBy;

                SQLService.CreateOrUpdateQuestion(question);

                // update old activity
                Microsoft.Bot.Connector.Activity updatedReply = activity.CreateReply();

                var card = new AdaptiveCard();
                var answerBlock = new AdaptiveTextBlock("Answer: " + question.AnswerText.ReplaceLinksWithMarkdown());
                answerBlock.Weight = AdaptiveTextWeight.Bolder;
                answerBlock.Size = AdaptiveTextSize.Medium;
                answerBlock.Wrap = true;

                var markedBlock = new AdaptiveTextBlock($"Marked as " + question.QuestionStatus + " by " + currentUser.Name);
                markedBlock.Wrap = true;

                var answeredBlock = new AdaptiveTextBlock("Answered by: " + question.AnswerPoster.FullName);
                answeredBlock.Weight = AdaptiveTextWeight.Bolder;
                answeredBlock.Wrap = true;

                card.Body.Add(answerBlock);
                card.Body.Add(answeredBlock);
                card.Body.Add(markedBlock);

                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card,
                };

                updatedReply.Attachments.Add(attachment);

                await connector.Conversations.UpdateActivityAsync(activity.Conversation.Id, activity.ReplyToId, updatedReply);
                var predictiveQnAService = new PredictiveQnAService(courseID);
                var res = await predictiveQnAService.UpdateQnAPair(new List<string> { question.QuestionText }, null, answer, answerId);
            }
        }

        private async Task HandleBotNotHelpful(Microsoft.Bot.Connector.Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            // tag demonstrators, regular workflow

            // get question details
            int questionId = Convert.ToInt32((activity.Value as dynamic)["questionId"]);
            var question = SQLService.GetQuestion(questionId);

            // get user details
            var channelData = activity.GetChannelData<Microsoft.Bot.Connector.Teams.Models.TeamsChannelData>();
            var members = await connector.Conversations.GetConversationMembersAsync(channelData.Team.Id);

            var teamsMembers = members.AsTeamsChannelAccounts();
            var currentUser = teamsMembers.Where(x => x.ObjectId == activity.From.Properties["aadObjectId"].ToString()).FirstOrDefault();
            var studentUPN = currentUser.UserPrincipalName;
            var isUserAdmin = SQLService.IsUserAdmin(studentUPN);
            string teamId = (activity.ChannelData as dynamic)["team"]["id"].ToString();
            TeamDetails teamDetails;
            try
            {
                teamDetails = connector.GetTeamsConnectorClient().Teams.FetchTeamDetails(teamId);
            }
            catch (Exception e)
            {
                teamDetails = connector.GetTeamsConnectorClient().Teams.FetchTeamDetails(teamId);
                Trace.TraceError(e.ToString());
            }

            var teamName = teamDetails.Name;
            var courseID = SQLService.GetCourseIDByName(teamName.Trim());

            // check if user can set question status
            if (isUserAdmin || (studentUPN == question.OriginalPoster.Email) || (studentUPN == question.OriginalPoster.UserName))
            {
                // find replies
                string channelId = (activity.ChannelData as dynamic)["channel"]["id"].ToString();
                var conversationId = activity.Conversation.Id;
                string messageId = conversationId.Split('=')[1];

                string originalStudentUpn = (activity.Value as dynamic)["userUpn"];
                var student = SQLService.GetUser(originalStudentUpn);

                var adminsOnTeams = new List<ChannelAccount>();
                var tutorialAdmins = new List<UserCourseRoleMappingModel>();

                if (student != null)
                {
                    if (student.Role != null && student.Role.Name != Constants.STUDENT_ROLE)
                    {
                        // Not a student - notify lecturer
                        tutorialAdmins = SQLService.GetUsersByRole(Constants.LECTURER_ROLE, courseID);
                    }
                    else
                    {
                        // Is a student
                        if (student.TutorialGroups != null && student.TutorialGroups.Count > 0)
                        {
                            // Notify demonstrator
                            foreach (var tutorialGroup in student.TutorialGroups.Where(x=>x.CourseId == courseID))
                            {
                                var demonstrators = SQLService.GetDemonstrators(courseID, tutorialGroup.ID);
                                if (demonstrators != null && demonstrators.Count > 0)
                                {
                                    tutorialAdmins.AddRange(demonstrators);
                                }
                            }
                        }
                        else
                        {
                            // student without tutorial class?
                            tutorialAdmins = SQLService.GetAllAdmins(student.CourseId).Distinct().ToList();
                        }

                    }
                }
                else
                {
                    // User not in database
                    // Notify lecturer
                    tutorialAdmins = SQLService.GetUsersByRole(Constants.LECTURER_ROLE, courseID);
                }

                if (tutorialAdmins != null && tutorialAdmins.Count > 0)
                {
                    foreach (var admin in tutorialAdmins)
                    {
                        var adminOnTeams = teamsMembers.Where(x =>
                            (x.Email == admin.Email || x.Email == admin.UserName || x.UserPrincipalName == admin.UserName || x.UserPrincipalName == admin.Email) &&
                            (x.Email != student.Email && x.Email != student.UserName && x.UserPrincipalName != student.UserName && x.UserPrincipalName != student.Email)
                        ).FirstOrDefault();
                        if (adminOnTeams != null)
                        {
                            adminsOnTeams.Add(adminOnTeams);
                        }
                    }
                }

                // update old activity
                Microsoft.Bot.Connector.Activity updatedReply = activity.CreateReply();

                var actionJson = "{\"Type\":\"" + Constants.ACTIVITY_SELECT_ANSWER + "\",\"QuestionId\": \"" + questionId + "\"}";

                var card = new HeroCard()
                {
                    Text = "Sorry I couldn't answer your question, I've tagged your demonstrators.",
                };

                updatedReply.Attachments.Add(card.ToAttachment());

                try
                {
                    await connector.Conversations.UpdateActivityAsync(activity.Conversation.Id, activity.ReplyToId, updatedReply);
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                }

                // create new tagging reply
                Microsoft.Bot.Connector.Activity newReply = activity.CreateReply();

                if (adminsOnTeams != null && adminsOnTeams.Count > 0)
                {
                    // Tag Admins
                    var distinct = adminsOnTeams
                    .GroupBy(p => p.Id)
                    .Select(g => g.First())
                    .ToList();

                    foreach (var admin in distinct)
                    {
                        newReply.AddMentionToText(admin, MentionTextLocation.AppendText);
                    }

                    var newActionJson = "{\"type\":\"" + Constants.ACTIVITY_SELECT_ANSWER + "\",\"questionId\": \"" + questionId + "\"}";

                    var newCard = new HeroCard()
                    {
                        Buttons = new List<CardAction>()
                        {
                            new CardAction(ActivityTypes.Invoke, "Select an answer", value: newActionJson),
                        },
                    };

                    newReply.Attachments.Add(newCard.ToAttachment());

                    try
                    {
                        var response = await connector.Conversations.ReplyToActivityAsync(activity.Conversation.Id, activity.ReplyToId, newReply);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.ToString());
                    }
                } else {
                    try
                    {
                        newReply.Text = "I'm sorry, I could not find anyone to tag. Please ensure the user has been assigned a tutorial group with atleast one demonstrator.";
                        var response = await connector.Conversations.ReplyToActivityAsync(activity.Conversation.Id, activity.ReplyToId, newReply);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.ToString());
                    }
                }
            }
        }

        private async Task HandleSelectAnswerFromCard(Microsoft.Bot.Connector.Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            // get question details
            int questionId = Convert.ToInt32((activity.Value as dynamic)["questionId"]);
            var question = SQLService.GetQuestion(questionId);

            // get user details
            var channelData = activity.GetChannelData<Microsoft.Bot.Connector.Teams.Models.TeamsChannelData>();
            var members = await connector.Conversations.GetConversationMembersAsync(channelData.Team.Id);

            var teamsMembers = members.AsTeamsChannelAccounts();
            var currentUser = teamsMembers.Where(x => x.ObjectId == activity.From.Properties["aadObjectId"].ToString()).FirstOrDefault();
            var studentUPN = currentUser.UserPrincipalName;

            // var isUserAdmin = SharePointServices.IsUserAdmin(studentUPN);
            var isUserAdmin = SQLService.IsUserAdmin(studentUPN);

            // check if user can set question status
            if (isUserAdmin || (studentUPN == question.OriginalPoster.Email) || (studentUPN == question.OriginalPoster.UserName))
            {
                // find replies
                string teamId = (activity.ChannelData as dynamic)["team"]["id"].ToString();
                string channelId = (activity.ChannelData as dynamic)["channel"]["id"].ToString();
                var conversationId = activity.Conversation.Id;
                string messageId = conversationId.Split('=')[1];

                // update old activity
                Microsoft.Bot.Connector.Activity updatedReply = activity.CreateReply();

                var replies = await GetAllReplies(question.GroupId, channelId, messageId);
                var adaptiveCardChoices = new List<AdaptiveChoice>();
                if (replies != null && replies.Count > 0)
                {
                    foreach (var reply in replies)
                    {
                        adaptiveCardChoices.Add(new AdaptiveChoice()
                        {
                            Title = MicrosoftTeamsChannelHelper.StripMentionAndHtml(reply.Body.Content),
                            Value = JsonConvert.SerializeObject(reply),
                        });
                    }

                    var bodyTextBlock = new AdaptiveTextBlock() { Text = "Select an answer" };
                    var bodyChoice = new AdaptiveChoiceSetInput()
                    {
                        Id = "Answer",
                        Style = AdaptiveChoiceInputStyle.Compact,
                        IsMultiSelect = false,
                        Choices = adaptiveCardChoices,
                    };

                    var adaptiveCardAnswerSubmit = new AdaptiveCardAnswerSubmit(Constants.ACTIVITY_MARKED_ANSWERED, bodyChoice.Value, questionId.ToString());
                    var adaptiveCardAnswerString = JsonConvert.SerializeObject(adaptiveCardAnswerSubmit);
                    var actionJson = "{\"type\":\"" + Constants.ACTIVITY_SELECT_ANSWER + "\",\"questionId\": \"" + questionId.ToString() + "\"}";

                    var adaptiveCard = new AdaptiveCard()
                    {
                        Actions = new List<AdaptiveAction>()
                                {
                                    new AdaptiveSubmitAction()
                                    {
                                        Title = "Save",
                                        DataJson = adaptiveCardAnswerString,
                                    },
                                    new AdaptiveSubmitAction()
                                    {
                                        Title = "Refresh",
                                        DataJson = actionJson,
                                    },
                                },
                    };

                    adaptiveCard.Body.Add(bodyTextBlock);
                    adaptiveCard.Body.Add(bodyChoice);

                    var attachment = new Attachment()
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = adaptiveCard,
                    };

                    updatedReply.Attachments.Add(attachment);

                    try
                    {
                        // await connector.Conversations.UpdateActivityAsync(updatedReply);
                        updatedReply.Id = activity.ReplyToId;
                        var response = await connector.Conversations.UpdateActivityAsync(activity.Conversation.Id, activity.ReplyToId, updatedReply);
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.ToString());
                    }
                }
            }
        }

        private async Task HandleMarkedAnswerFromCard(Microsoft.Bot.Connector.Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            // Handle answer payload
            var adaptiveCardAnswerJson = activity.Value.ToString();
            var adaptiveCardAnswer = JsonConvert.DeserializeObject<AdaptiveCardAnswerSubmit>(adaptiveCardAnswerJson);

            string selectedReplyJson = adaptiveCardAnswer.Answer;
            string questionId = adaptiveCardAnswer.QuestionId;

            if (string.IsNullOrEmpty(selectedReplyJson))
            {
                Microsoft.Bot.Connector.Activity newReply = activity.CreateReply();
                newReply.Text = "There seems to be an issue saving the answer.";
                await connector.Conversations.ReplyToActivityAsync(activity.Conversation.Id, activity.ReplyToId, newReply);
            }
            else
            {
                var selectedReply = JsonConvert.DeserializeObject<TeamsMessage>(selectedReplyJson);
                var question = SQLService.GetQuestion(Convert.ToInt32(questionId));

                // get user details
                var channelData = activity.GetChannelData<Microsoft.Bot.Connector.Teams.Models.TeamsChannelData>();
                var members = await connector.Conversations.GetConversationMembersAsync(channelData.Team.Id);

                var teamsMembers = members.AsTeamsChannelAccounts();
                var currentUser = teamsMembers.Where(x => x.ObjectId == activity.From.Properties["aadObjectId"].ToString()).FirstOrDefault();
                var studentUPN = currentUser.UserPrincipalName;
                var isUserAdmin = SQLService.IsUserAdmin(studentUPN);
                string teamId = channelData.Team.Id;
                TeamDetails teamDetails;
                try
                {
                    teamDetails = connector.GetTeamsConnectorClient().Teams.FetchTeamDetails(teamId);
                }
                catch (Exception e)
                {
                    teamDetails = connector.GetTeamsConnectorClient().Teams.FetchTeamDetails(teamId);
                    Trace.TraceError(e.ToString());
                }
                var teamName = teamDetails.Name;
                var courseID = SQLService.GetCourseIDByName(teamName.Trim());

                // check if user can set question status
                if (isUserAdmin || (studentUPN == question.OriginalPoster.Email) || (studentUPN == question.OriginalPoster.UserName))
                {
                    var selectedReplyUser = teamsMembers.Where(x => x.ObjectId == selectedReply.From.User.Id).FirstOrDefault();
                    var answeredBy = SQLService.GetUser(selectedReplyUser.UserPrincipalName);

                    // update question model
                    question.QuestionStatus = Constants.QUESTION_STATUS_ANSWERED;
                    question.QuestionAnswered = DateTime.Now;
                    question.AnswerText = MicrosoftTeamsChannelHelper.StripMentionAndHtml(selectedReply.Body.Content);
                    question.AnswerPoster = answeredBy;

                    SQLService.CreateOrUpdateQuestion(question);

                    // update old activity
                    Microsoft.Bot.Connector.Activity updatedReply = activity.CreateReply();

                    var card = new AdaptiveCard();
                    var answer = new AdaptiveTextBlock("Answer: " + question.AnswerText.ReplaceLinksWithMarkdown());
                    answer.Weight = AdaptiveTextWeight.Bolder;
                    answer.Size = AdaptiveTextSize.Medium;
                    answer.Wrap = true;

                    var marked = new AdaptiveTextBlock($"Marked as " + question.QuestionStatus + " by " + currentUser.Name);
                    marked.Wrap = true;

                    var answered = new AdaptiveTextBlock("Answered by: " + question.AnswerPoster.FullName);
                    answered.Weight = AdaptiveTextWeight.Bolder;
                    answered.Wrap = true;

                    card.Body.Add(answer);
                    card.Body.Add(answered);
                    card.Body.Add(marked);

                    Attachment attachment = new Attachment()
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = card,
                    };

                    updatedReply.Attachments.Add(attachment);

                    await connector.Conversations.UpdateActivityAsync(activity.Conversation.Id, activity.ReplyToId, updatedReply);
                    var predictiveQnAService = new PredictiveQnAService(courseID);
                    var res = await predictiveQnAService.AddQnAPair(question.QuestionText, question.AnswerText);
                }
            }
        }

        private async Task<HttpResponseMessage> HandleSelectAnswerFromMessageAction(Microsoft.Bot.Connector.Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            TeamsMessage selectedReply = JsonConvert.DeserializeObject<TeamsMessage>(JsonConvert.SerializeObject((activity.Value as dynamic)["messagePayload"]));

            if (selectedReply.From.User == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            var question = SQLService.GetQuestionByMessageId(selectedReply.ReplyToId);

            if (question == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            // get user details
            var channelData = activity.GetChannelData<Microsoft.Bot.Connector.Teams.Models.TeamsChannelData>();
            var members = await connector.Conversations.GetConversationMembersAsync(channelData.Team.Id);
            var teamsMembers = members.AsTeamsChannelAccounts();
            var currentUser = teamsMembers.Where(x => x.ObjectId == activity.From.Properties["aadObjectId"].ToString()).FirstOrDefault();
            var studentUPN = currentUser.UserPrincipalName;
            var isUserAdmin = SQLService.IsUserAdmin(studentUPN);

            // check if user can set question status
            if (isUserAdmin || (studentUPN == question.OriginalPoster.Email) || (studentUPN == question.OriginalPoster.UserName))
            {
                var selectedReplyUser = teamsMembers.Where(x => x.ObjectId == selectedReply.From.User.Id).FirstOrDefault();
                var answeredBy = SQLService.GetUser(selectedReplyUser.UserPrincipalName);

                // update question model
                question.QuestionStatus = Constants.QUESTION_STATUS_ANSWERED;
                question.QuestionAnswered = DateTime.Now;
                question.AnswerText = MicrosoftTeamsChannelHelper.StripMentionAndHtml(selectedReply.Body.Content);
                question.AnswerPoster = answeredBy;

                SQLService.CreateOrUpdateQuestion(question);

                Microsoft.Bot.Connector.Activity updatedReply = activity.CreateReply();

                var card = new AdaptiveCard();
                var answerBlock = new AdaptiveTextBlock("Answer: " + question.AnswerText.ReplaceLinksWithMarkdown());
                answerBlock.Weight = AdaptiveTextWeight.Bolder;
                answerBlock.Size = AdaptiveTextSize.Medium;
                answerBlock.Wrap = true;

                var markedBlock = new AdaptiveTextBlock($"Marked as " + question.QuestionStatus + " by " + currentUser.Name);
                markedBlock.Wrap = true;

                var answeredBlock = new AdaptiveTextBlock("Answered by: " + question.AnswerPoster.FullName);
                answeredBlock.Weight = AdaptiveTextWeight.Bolder;
                answeredBlock.Wrap = true;

                card.Body.Add(answerBlock);
                card.Body.Add(answeredBlock);
                card.Body.Add(markedBlock);

                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card,
                };

                updatedReply.Attachments.Add(attachment);

                if (question.AnswerCardActivityId == null)
                {
                    var r = await connector.Conversations.ReplyToActivityAsync(updatedReply.Conversation.Id, updatedReply.ReplyToId, updatedReply);

                    question.AnswerCardActivityId = r.Id;
                    SQLService.CreateOrUpdateQuestion(question);

                }
                else
                {
                    updatedReply.ReplyToId = question.AnswerCardActivityId;
                    var r = await connector.Conversations.UpdateActivityAsync(updatedReply.Conversation.Id, updatedReply.ReplyToId, updatedReply);
                    question.AnswerCardActivityId = r.Id;
                    SQLService.CreateOrUpdateQuestion(question);

                }
            }

            var baseUrl = ServiceHelper.BaseUrl;
            var taskInfo = new Models.TaskInfo();

            taskInfo.Title = "Select Answer";
            taskInfo.Height = 300;
            taskInfo.Width = 400;
            taskInfo.Url = baseUrl + @"/home/selectanswer?json=" + "Answer Updated";

            Models.TaskEnvelope taskEnvelope = new Models.TaskEnvelope
            {
                Task = new Models.Task()
                {
                    Type = Models.TaskType.Continue,
                    TaskInfo = taskInfo,
                },
            };
            return Request.CreateResponse(HttpStatusCode.OK, taskEnvelope);
        }

        private async Task<Microsoft.Bot.Connector.Activity> HandleSystemMessage(Microsoft.Bot.Connector.Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                if (message.ChannelId == "msteams")
                {
                    var addedBot = false;
                    for (int i = 0; i < message.MembersAdded.Count; i++)
                    {
                        if (message.MembersAdded[i].Id == message.Recipient.Id)
                        {
                            addedBot = true;
                            break;
                        }
                    }

                    if (addedBot)
                    {
                        var reply = message.CreateReply("Hi, I'm here to learn and help answer your questions! Just add '@Question' to your queries :)");
                        ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }
                }

            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }

            return null;
        }

        private async Task<List<TeamsMessage>> GetAllReplies(string teamsId, string channelId, string messageId)
        {
            var authority = ServiceHelper.Authority;
            var resource = ServiceHelper.GraphResource;

            var authService = new AuthService(authority);
            var authResult = await authService.AuthenticateSilently(resource);

            var graphService = new GraphService();
            var teamsMessages = await graphService.GetRepliesToMessage(authResult.AccessToken, teamsId, channelId, messageId);

            return teamsMessages.Where(x => x.From.Application == null && !string.IsNullOrEmpty(MicrosoftTeamsChannelHelper.StripMentionAndHtml(x.Body.Content))).ToList();
        }

        private string GetActivityTypeFrom(Microsoft.Bot.Connector.Activity activity)
        {
            if (activity.Type == ActivityTypes.Invoke)
            {
                return ActivityTypes.Invoke;
            }
            else if (activity.Type == ActivityTypes.Message)
            {
                var invokeType = string.Empty;
                try
                {
                    // need to determine if its an invoke disguised as a message
                    invokeType = (activity.Value as dynamic)["type"];
                }
                catch (Exception ex)
                {
                    invokeType = string.Empty;
                }

                if (!string.IsNullOrEmpty(invokeType))
                {
                    if (invokeType == Constants.ACTIVITY_SELECT_ANSWER
                        || invokeType == Constants.ACTIVITY_BOT_HELPFUL
                        || invokeType == Constants.ACTIVITY_BOT_NOT_HELPFUL
                        || invokeType == Constants.ACTIVITY_MARKED_ANSWERED)
                    {
                        return ActivityTypes.Invoke;
                    }
                    else
                    {
                        return ActivityTypes.Message;
                    }
                }
                else
                {
                    return ActivityTypes.Message;
                }
            }

            return activity.Type;
        }
    }

    [Serializable]
    public class AdaptiveCardAnswerSubmit
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("answer")]
        public string Answer { get; set; }

        [JsonProperty("questionId")]
        public string QuestionId { get; set; }

        public AdaptiveCardAnswerSubmit(string type, string answer, string questionId)
        {
            Type = type;
            Answer = answer;
            QuestionId = questionId;
        }
    }
}