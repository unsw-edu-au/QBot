using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Teams.Apps.QBot.Data;

using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model
{
    public static class ModelMapper
    {

        #region Question
        public static Question MapFromQuestionModel(QuestionModel questionModel)
        {
            if (questionModel == null)
            {
                return null;
            }

            var question = new Question()
            {
                Id = questionModel.ID,
                TenantId = new Guid(questionModel.TenantId),
                GroupId = new Guid(questionModel.GroupId),
                TeamId = questionModel.TeamId,
                TeamName = questionModel.TeamName,
                ConversationId = questionModel.ConversationId,
                MessageId = questionModel.MessageId,
                Topic = questionModel.Topic,
                Status = questionModel.QuestionStatus,
                QuestionText = questionModel.QuestionText,
                Link = questionModel.Link,
                AnswerText = questionModel.AnswerText,
                AnswerMessageId = questionModel.AnswerMessageId,
                DateSubmitted = questionModel.QuestionSubmitted,
                DateAnswered = questionModel.QuestionAnswered,
                AnswerCardActivityId = questionModel.AnswerCardActivityId,
                CourseId = questionModel.CourseID ?? default(int),
                ///Course = SQLAdapter.GetCourse(questionModel.CourseID ?? default(int)),
            };

            if (questionModel.OriginalPoster != null)
            {
                question.OriginalPosterId = int.Parse(questionModel.OriginalPoster.UserId);
            }

            if (questionModel.AnswerPoster != null)
            {
                question.AnswerPosterId = int.Parse(questionModel.AnswerPoster.UserId);
            }
            //if (questionModel.QRQuestion != null)
            //    question.QRQuestionID = questionModel.QRQuestion.ID;

            //if (questionModel.Attachments != null)
            //{
            //    question.Attachments = new List<Attachment>();
            //    question.Attachments.ToList().AddRange(MapFromAttachmentModels(questionModel.Attachments));
            //}

            return question;
        }

        public static QuestionModel MapToQuestionModel(Question question)
        {
            if (question == null)
            {
                return null;
            }

            var questionModel = new QuestionModel()
            {
                ID = question.Id,
                TenantId = question.TenantId.ToString(),
                GroupId = question.GroupId.ToString(),
                TeamId = question.TeamId,
                TeamName = question.TeamName,
                ConversationId = question.ConversationId,
                MessageId = question.MessageId,
                Topic = question.Topic,
                QuestionStatus = question.Status,
                QuestionText = question.QuestionText,
                Link = question.Link,
                AnswerText = question.AnswerText,
                AnswerMessageId = question.AnswerMessageId,
                QuestionSubmitted = question.DateSubmitted,
                QuestionAnswered = question.DateAnswered,
                AnswerCardActivityId = question.AnswerCardActivityId,
                OriginalPoster = MapToUserModelOnly(question.QuestionUser),
                AnswerPoster = MapToUserModelOnly(question.AnswerUser),
                CourseID = question.CourseId,
            };

            return questionModel;
        }

        public static List<QuestionModel> MapToQuestionModels(List<Question> questions)
        {
            if (questions == null)
            {
                return null;
            }

            var questionModels = new List<QuestionModel>();

            foreach (var question in questions)
            {
                questionModels.Add(MapToQuestionModel(question));
            }

            return questionModels;
        }
        #endregion

        #region Attachment
        //public static Attachment MapFromAttachmentModel(AttachmentModel attachmentModel)
        //{
        //    if (attachmentModel == null)
        //        return null;

        //    var attachment = new Attachment()
        //    {
        //        Id = attachmentModel.Id,
        //        QuestionId = attachmentModel.QuestionId,
        //        AttachmentType = attachmentModel.AttachmentType,
        //        AttachmentUrl = attachmentModel.AttachmentUrl,
        //        FileType = attachmentModel.FileType
        //    };

        //    return attachment;
        //}

        //public static List<Attachment> MapFromAttachmentModels(List<AttachmentModel> attachmentModels)
        //{
        //    if (attachmentModels == null)
        //        return null;

        //    var attachments = new List<Attachment>();

        //    foreach (var attachmentModel in attachmentModels)
        //    {
        //        attachments.Add(MapFromAttachmentModel(attachmentModel));
        //    }

        //    return attachments;
        //}

        //public static AttachmentModel MapToAttachmentModel(Attachment attachment)
        //{
        //    if (attachment == null)
        //        return null;

        //    var attachmentModel = new AttachmentModel()
        //    {
        //        Id = attachment.Id,
        //        QuestionId = attachment.QuestionId,
        //        AttachmentType = attachment.AttachmentType,
        //        AttachmentUrl = attachment.AttachmentUrl,
        //        FileType = attachment.FileType
        //    };

        //    return attachmentModel;
        //}

        //public static List<AttachmentModel> MapToAttachmentModels(List<Attachment> attachments)
        //{
        //    if (attachments == null)
        //        return null;

        //    var attachmentModels = new List<AttachmentModel>();

        //    foreach (var attachment in attachments)
        //    {
        //        attachmentModels.Add(MapToAttachmentModel(attachment));
        //    }

        //    return attachmentModels;
        //}
        #endregion

        #region TutorialGroup
        public static TutorialGroupModel MapToTutorialGroupModel(TutorialGroupMembership userTutorialGroup)
        {
            if (userTutorialGroup == null)
                return null;

            return new TutorialGroupModel()
            {
                ID = userTutorialGroup.TutorialGroup.Id,
                Code = userTutorialGroup.TutorialGroup.Code,
                Name = userTutorialGroup.TutorialGroup.Name,
                CourseId = userTutorialGroup.TutorialGroup.CourseId,
                //Class = userTutorialGroup.TutorialGroup.Class,
                //Location = userTutorialGroup.TutorialGroup.Location,
            };
        }

        public static List<TutorialGroupModel> MapToTutorialGroupsModel(List<TutorialGroupMembership> userTutorialGroups)
        {
            if (userTutorialGroups == null)
                return null;

            var tutorialGroupsModel = new List<TutorialGroupModel>();

            foreach (var userTutorialGroup in userTutorialGroups)
            {
                tutorialGroupsModel.Add(MapToTutorialGroupModel(userTutorialGroup));
            }

            return tutorialGroupsModel;
        }

        public static List<int> GetIdsFromTutorialGroupModelList(List<TutorialGroupModel> tutorialGroupModels)
        {
            if (tutorialGroupModels == null)
                return null;

            List<int> tutorialIds = new List<int>();
            foreach (TutorialGroupModel tut in tutorialGroupModels)
            {
                tutorialIds.Add(tut.ID);
            }
            return tutorialIds;

        }
        #endregion

        public static TutorialGroupModel MapToTutorialGroupModel(TutorialGroup tutorialGroup)
        {
            if (tutorialGroup == null)
            {
                return null;
            }

            return new TutorialGroupModel()
            {
                ID = tutorialGroup.Id,
                Code = tutorialGroup.Code,
                CourseId = tutorialGroup.CourseId,
                Name = tutorialGroup.Name
                //Class = tutorialGroup.Class,
                //Location = tutorialGroup.Location,
            };
        }

        public static List<TutorialGroupModel> MapToTutorialGroupsModel(List<TutorialGroup> tutorialGroups)
        {
            if (tutorialGroups == null)
            {
                return null;
            }

            var tutorialGroupsModel = new List<TutorialGroupModel>();

            foreach (var tutorialGroup in tutorialGroups)
            {
                tutorialGroupsModel.Add(MapToTutorialGroupModel(tutorialGroup));
            }

            return tutorialGroupsModel;
        }

        #region Role

        public static List<RoleModel> MapToRoleModelList(List<Role> roles)
        {
            if (roles == null)
            {
                return null;
            }

            var roleModelList = new List<RoleModel>();

            foreach (var role in roles)
            {
                roleModelList.Add(MapToRoleModel(role));
            }

            return roleModelList;
        }
        public static RoleModel MapToRoleModel(Role role)
        {
            if (role == null)
                return null;

            return new RoleModel()
            {
                Id = role.Id,
                Name = role.Name,
            };
        }
        #endregion

        #region User
        public static UserCourseRoleMappingModel MapToUserModel(UserCourseRoleMapping ucrm)
        {
            if (ucrm == null)
                return null;
            UserCourseRoleMappingModel user = MapToUserModelOnly(ucrm.User);
            RoleModel role = MapToRoleModel(ucrm.Role);
            var userCourseRoleModel = new UserCourseRoleMappingModel()
            {
                ID = ucrm.Id,
                UserId = ucrm.UserId.ToString(),
                CourseId = ucrm.CourseId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                PersonalConversationContactData = ucrm.User.PersonalConversationContactData != null ? JsonConvert.DeserializeObject<PersonalConversationContactData>(ucrm.User.PersonalConversationContactData) : null,
                Role = role,
                TutorialGroups = MapToTutorialGroupsModel(ucrm.User.TutorialGroupMemberships.ToList()),
            };
            var relatedMappingModels = SQLAdapter.GetUsersByUPN(ucrm.User.UserPrincipalName);
            bool isAdmin = false;

            foreach (UserCourseRoleMapping userCourseRoleMapping in relatedMappingModels) {
                if (userCourseRoleMapping.Role.Name != "Student") {
                    isAdmin = true;
                }
            }
            userCourseRoleModel.IsAdmin = isAdmin;

            if (userCourseRoleModel.Role != null)
            {
                userCourseRoleModel.RoleName = userCourseRoleModel.Role.Name;
            }

            if (userCourseRoleModel.TutorialGroups != null && userCourseRoleModel.TutorialGroups.Count > 0)
            {
                var sb = new StringBuilder("");
                userCourseRoleModel.TutorialGroups.ForEach(tg =>
                {
                    sb.Append(tg.Code);
                    sb.Append(", ");
                });
                sb.Length -= 2;
                userCourseRoleModel.TutorialGroupsString = sb.ToString();
            }
            return userCourseRoleModel;
        }

        public static User MapFromUserModel(UserCourseRoleMappingModel userModel)
        {
            if (userModel == null)
                return null;

            var user = new User()
            {
                Id = userModel.ID,
                StudentId = userModel.UserId,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                UserPrincipalName = userModel.UserName,
                Email = userModel.Email,
                PersonalConversationContactData = userModel.PersonalConversationContactData != null ? JsonConvert.SerializeObject(userModel.PersonalConversationContactData) : null,
                //RoleId = userModel.Role != null? userModel.Role.Id: 0,
            };

            return user;
        }

        public static UserCourseRoleMapping MapFromUserModelToUserCourseMapping(UserCourseRoleMappingModel userModel)
        {
            if (userModel == null)
                return null;

            var user = new UserCourseRoleMapping()
            {
                Id = userModel.ID,
                UserId = int.Parse(userModel.UserId),
                CourseId = userModel.CourseId,
                RoleId = userModel.Role.Id,
            };

            return user;
        }

        public static UserCourseRoleMappingModel MapToUserModelOnly(User user)
        {
            if (user == null)
                return null;

            return new UserCourseRoleMappingModel()
            {
                ID = user.Id,
                UserId = user.Id.ToString(),
                StudentID = user.StudentId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserPrincipalName,
                Email = user.Email,
            };
        }

        public static UserCourseRoleMappingModel MapToUserModelOnly(UserCourseRoleMapping ucrm)
        {
            if (ucrm == null)
                return null;
            UserCourseRoleMappingModel user = MapToUserModelOnly(ucrm.User);
            var userCourseRoleModel = new UserCourseRoleMappingModel()
            {
                ID = ucrm.Id,
                UserId = ucrm.UserId.ToString(),
                CourseId = ucrm.CourseId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email
            };

            return userCourseRoleModel;
        }

        public static List<UserCourseRoleMappingModel> MapToUsersModel(List<UserCourseRoleMapping> users)
        {
            if (users == null)
                return null;

            var userModels = new List<UserCourseRoleMappingModel>();

            foreach (var user in users)
            {
                userModels.Add(MapToUserModel(user));
            }

            return userModels;
        }

        public static List<UserCourseRoleMappingModel> MapToUsersModelOnly(List<UserCourseRoleMapping> users)
        {
            if (users == null)
                return null;

            var userModels = new List<UserCourseRoleMappingModel>();

            foreach (var user in users)
            {
                userModels.Add(MapToUserModelOnly(user));
            }

            return userModels;
        }

        public static List<User> MapFromUserModels(List<UserCourseRoleMappingModel> userModels)
        {
            if (userModels == null)
                return null;

            var users = new List<User>();

            foreach (var userModel in userModels)
            {
                users.Add(MapFromUserModel(userModel));
            }

            return users;
        }


        #endregion

        #region course
        public static CourseModel MapToCourseModel(Course course)
        {
            if (course == null)
                return null;
            else
            {
                return new CourseModel()
                {
                    Id = course.Id,
                    Name = course.Name,
                    GroupId = course.GroupId,
                    PredictiveQnAServiceHost = course.PredictiveQnAServiceHost,
                    PredictiveQnAKnowledgeBaseId = course.PredictiveQnAKnowledgeBaseId,
                    PredictiveQnAEndpointKey = course.PredictiveQnAEndpointKey,
                    PredictiveQnAHttpEndpoint = course.PredictiveQnAHttpEndpoint,
                    PredictiveQnAHttpKey = course.PredictiveQnAHttpKey,
                    PredictiveQnAKnowledgeBaseName = course.PredictiveQnAKnowledgeBaseName,
                    PredictiveQnAConfidenceThreshold = course.PredictiveQnAConfidenceThreshold,
                    DeployedURL = course.DeployedURL,
                };
            }
        }
        public static List<CourseModel> MapToCourseModels(List<Course> courses)
        {
            if (courses == null)
                return null;

            var ccourseModels = new List<CourseModel>();

            foreach (var c in courses)
            {
                ccourseModels.Add(MapToCourseModel(c));
            }

            return ccourseModels;
        }

        #endregion

        #region Assessment

        //public static AssessmentModel MapToAssessmentModel(Assessment assessment)
        //{
        //    if (assessment == null)
        //    {
        //        return null;
        //    }

        //    var assessmentModel = new AssessmentModel()
        //    {
        //        Id = assessment.Id,
        //        Type = assessment.Type,
        //        SubType = assessment.SubType,
        //        Title = assessment.Title,
        //        Topic = assessment.Topic,
        //        Weighting = assessment.Weighting,
        //    };

        //    if (assessment.Date != null)
        //    {
        //        assessmentModel.Date = assessment.Date.Value;
        //    }
        //    else
        //    {
        //        assessmentModel.Date = DateTime.MinValue;
        //    }

        //    if (assessment.DateEnd != null)
        //    {
        //        assessmentModel.DateEnd = assessment.DateEnd.Value;
        //    }
        //    else
        //    {
        //        assessmentModel.DateEnd = DateTime.MinValue;
        //    }

        //    return assessmentModel;
        //}

        //public static List<AssessmentModel> MapToAssessmentModels(List<Assessment> assessments)
        //{
        //    if (assessments == null)
        //        return null;

        //    var assessmentModels = new List<AssessmentModel>();

        //    foreach (var assessment in assessments)
        //    {
        //        assessmentModels.Add(MapToAssessmentModel(assessment));
        //    }

        //    return assessmentModels;
        //}

        #endregion

        #region QR
        //public static QRQuestionModel MapToQRQuestionModel(QRQuestionLookup qrQuestion)
        //{
        //    if (qrQuestion == null)
        //        return null;

        //    return new QRQuestionModel()
        //    {
        //        ID = qrQuestion.ID,
        //        CourseCode = qrQuestion.CourseCode,
        //        CourseName = qrQuestion.CourseName,
        //        QuestionCode = qrQuestion.QuestionCode,
        //        QuestionTopic = qrQuestion.QuestionTopic,
        //        QuestionText = qrQuestion.QuestionText,
        //        HintText = qrQuestion.HintText,
        //        AnswerText = qrQuestion.AnswerText,
        //        StreamLink = qrQuestion.StreamLink,
        //        QuestionNumber = qrQuestion.QuestionNumber,
        //        QuestionName = qrQuestion.QuestionName,
        //        QuestionDisplayName = qrQuestion.QuestionDisplayName,
        //        QuestionDiagram = qrQuestion.QuestionDiagram,
        //    };
        //}
        #endregion
    }
}
