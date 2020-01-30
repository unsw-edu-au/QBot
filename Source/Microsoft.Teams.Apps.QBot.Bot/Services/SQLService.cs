using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Teams.Apps.QBot.Data;
using Microsoft.Teams.Apps.QBot.Model;

namespace Microsoft.Teams.Apps.QBot.Bot.Services
{
    public static class SQLService
    {
        /// <summary>
        /// Returns a list of O365 Group IDs where a question has been asked, which also matches the list of Group Ids passed in
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="memberOf">List of all Group Ids to filter by</param>
        /// <returns>List of all O365 Group IDs</returns>
        public static List<string> GetTeamGroupIdsWithQuestions(string tenantId, List<string> memberOf)
        {
            try
            {

                var groupIds = SQLAdapter.GetTeamGroupIdsWithQuestions(tenantId, memberOf);

                return groupIds;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #region Question
        public static int CreateOrUpdateQuestion(QuestionModel questionModel)
        {
            try
            {
                var question = ModelMapper.MapFromQuestionModel(questionModel);

                return SQLAdapter.AddOrUpdateQuestion(question);
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public static QuestionModel GetQuestion(int questionId)
        {
            try
            {
                var question = SQLAdapter.GetQuestionById(questionId);

                //if (question.QuestionUser != null)
                //{
                //    var questionUser = SQLAdapter.GetUserById(question.QuestionUser.Id);
                //    question.QuestionUser = questionUser;
                //}

                //if (question.AnswerUser != null)
                //{
                //    var answerUser = SQLAdapter.GetUserById(question.AnswerUser.Id);
                //    question.AnswerUser = answerUser;
                //}

                return ModelMapper.MapToQuestionModel(question);
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public static QuestionModel GetQuestionByMessageId(string messageId)
        {
            try
            {
                var question = SQLAdapter.GetQuestionByMessageId(messageId);

                //if (question.QuestionUser != null)
                //{
                //    var questionUser = SQLAdapter.GetUserById(question.QuestionUser.Id);
                //    question.QuestionUser = questionUser;
                //}

                //if (question.AnswerUser != null)
                //{
                //    var answerUser = SQLAdapter.GetUserById(question.AnswerUser.Id);
                //    question.AnswerUser = answerUser;
                //}

                return ModelMapper.MapToQuestionModel(question);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<QuestionModel> GetAllQuestions(string tenantId = null)
        {
            try
            {
                var questions = SQLAdapter.GetAllQuestions(tenantId);

                var questionModels = ModelMapper.MapToQuestionModels(questions);

                return questionModels;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<QuestionModel> GetQuestionsByGroup(string groupId)
        {
            try
            {
                var questions = SQLAdapter.GetQuestionsByGroup(groupId);

                var questionModels = ModelMapper.MapToQuestionModels(questions);

                return questionModels;

            }
            catch (Exception e)
            {
                return null;
            }
        }


        public static List<QuestionModel> GetAllQuestionsByTutorial(string tenantId, string code)
        {
            try
            {
                var questions = SQLAdapter.GetQuestionsByTutorialGroup(tenantId, code);

                var questionModels = ModelMapper.MapToQuestionModels(questions);

                return questionModels;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool DoesConversationIdExist(string conversationId)
        {
            try
            {

                var question = SQLAdapter.GetQuestionByConversationId(conversationId);

                return question != null;

            }
            catch (Exception e)
            {
                return false;
            }

        }
        #endregion

        #region User

        public static bool NoUsers()
        {
            return SQLAdapter.NoUsers();
        }

        public static void AddGlobalAdminUser(string upn, string firstName, string lastName, string email)
        {
            var user = new User
            {
                UserPrincipalName = upn,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                IsGlobalAdmin = true,
            };

            SQLAdapter.UpdateUser(user);
        }

        public static UserAccessModel GetUserAccess(string upn)
        {
            var result = new UserAccessModel();
            var user = SQLAdapter.GetUserByUPN(upn);
            if (user == null)
            {
                throw new Exception("User " + upn + " does not exist");
            }

            result.IsGlobalAdmin = user.IsGlobalAdmin == true;

            var userRoleMappings = SQLAdapter.GetUsersByUPN(upn);
            result.IsLecturer = userRoleMappings.Any(m => m.Role.Name == Constants.LECTURER_ROLE);
            result.IsDemonstrator = userRoleMappings.Any(m => m.Role.Name == Constants.DEMONSTRATOR_ROLE);

            return result;
        }

        public static UserCourseRoleMappingModel GetUser(string userPrincipleName)
        {
            try
            {
                var user = SQLAdapter.GetUserMappingsByUPN(userPrincipleName);
                var ucrm = ModelMapper.MapToUserModel(user);
                return ucrm;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<UserCourseRoleMappingModel> GetUsersByRole(string role, int courseID)
        {
            try
            {
                var users = SQLAdapter.GetUsersByRole(role, courseID);

                return ModelMapper.MapToUsersModel(users);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static UserCourseRoleMappingModel GetBotUser(int courseID)
        {
            try
            {
                var user = SQLAdapter.GetBotUser(courseID);

                return ModelMapper.MapToUserModel(user);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static bool IsUserAdmin(string email)
        {
            try
            {
                var users = SQLAdapter.GetUsersByUPN(email);
                if (users == null)
                {
                    return false;
                }
                else
                {
                    foreach (UserCourseRoleMapping user in users) {
                        if (user.Role.Name != "Student") {
                            return true;
                        }
                    }
                }
                return false;

            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static bool UpdateUser(UserCourseRoleMappingModel userModel)
        {
            try
            {
                var user = ModelMapper.MapFromUserModel(userModel);

                return SQLAdapter.UpdateUser(user);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static List<UserCourseRoleMappingModel> GetDemonstrators(int courseId, int tutorialId)
        {
            try
            {
                var demonstrators = SQLAdapter.GetDemonstrators(courseId, tutorialId);

                var result = ModelMapper.MapToUsersModelOnly(demonstrators);

                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<UserCourseRoleMappingModel> GetStudents(string tutorialCode)
        {
            try
            {
                var students = new List<UserCourseRoleMapping>();

                var rawUsers = SQLAdapter.GetUsersByTutorialCode(tutorialCode);

                foreach (var user in rawUsers)
                {
                    var retrievedUser = SQLAdapter.GetUserById(user.Id);
                    if (retrievedUser.Role.Name == "Student")
                        students.Add(retrievedUser);
                }

                var result = ModelMapper.MapToUsersModel(students);

                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<UserCourseRoleMappingModel> GetAllAdmins(int courseID)
        {
            try
            {

                var admins = new List<UserCourseRoleMappingModel>();
                var demonstrators = SQLAdapter.GetUsersByRole("Demonstrator", courseID);
                var lecturers = SQLAdapter.GetUsersByRole("Lecturer", courseID);

                admins.AddRange(ModelMapper.MapToUsersModel(demonstrators));
                admins.AddRange(ModelMapper.MapToUsersModel(lecturers));

                return admins;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion

        #region Course
        public static Course GetCourse(int courseID)
        {
            try
            {
                return SQLAdapter.GetCourse(courseID);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static int GetCourseIDByName(string name)
        {
            try
            {
                return SQLAdapter.GetCourseIDByName(name);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public static List<Course> GetCourses()
        {
            try
            {
                return SQLAdapter.GetCourses();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<Course> SaveCourse(Course course)
        {
            try
            {
                return SQLAdapter.SaveCourse(course);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<Course> DeleteCourse(int id)
        {
            try
            {
                return SQLAdapter.DeleteCourse(id);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<UserCourseRoleMapping> AddStudents(List<User> students, string coursename)
        {
            try
            {
                return SQLAdapter.AddStudents(students, coursename);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        public static List<UserCourseRoleMappingModel> GetUserCourseRoleMappingsByCourse(int courseId)
        {
            try
            {
                var ucrms = SQLAdapter.GetUserCourseRoleMappingsByCourse(courseId);
                var mappedUcrms = ModelMapper.MapToUsersModel(ucrms);
                return mappedUcrms;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<UserCourseRoleMappingModel> DeleteUserCourseRoleMapping(UserCourseRoleMapping ucrm)
        {
            try
            {
                var ucrms = SQLAdapter.DeleteUserCourseRoleMapping(ucrm);
                var mappedUcrms = ModelMapper.MapToUsersModel(ucrms);
                return mappedUcrms;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<UserCourseRoleMappingModel> SaveUserCourseRoleMapping(UserCourseRoleMappingModel ucrm)
        {
            try
            {
                var ucrms = SQLAdapter.SaveUserCourseRoleMapping(ModelMapper.MapFromUserModelToUserCourseMapping(ucrm), ModelMapper.GetIdsFromTutorialGroupModelList(ucrm.TutorialGroups));
                var mappedUcrms = ModelMapper.MapToUsersModel(ucrms);
                return mappedUcrms;
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public static List<RoleModel> GetRoles()
        {
            try
            {

                var roles = SQLAdapter.GetRoles();
                return ModelMapper.MapToRoleModelList(roles);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<TutorialGroupModel> GetTutorialsByCourse(int courseId)
        {
            try
            {
                var tutorialGroups = SQLAdapter.GetTutorialsByCourse(courseId);
                var tutorialGroupModels = ModelMapper.MapToTutorialGroupsModel(tutorialGroups);
                return tutorialGroupModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<TutorialGroupModel> SaveTutorialGroup(TutorialGroup tutorial)
        {
            try
            {
                var tutorialGroups = SQLAdapter.SaveTutorialGroup(tutorial);
                var tutorialGroupModels = ModelMapper.MapToTutorialGroupsModel(tutorialGroups);
                return tutorialGroupModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<TutorialGroupModel> DeleteTutorialGroup(TutorialGroup tutorial)
        {
            try
            {
                var tutorialGroups = SQLAdapter.DeleteTutorialGroup(tutorial);
                var tutorialGroupModels = ModelMapper.MapToTutorialGroupsModel(tutorialGroups);
                return tutorialGroupModels;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion
    }
}