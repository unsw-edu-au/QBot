using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Microsoft.Teams.Apps.QBot.Data
{
    public static class SQLAdapter
    {
        #region Question
        public static int AddOrUpdateQuestion(Question question)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var dbQuestion = entities.Questions.Where(x => x.Id == question.Id)
                        .Include("QuestionUser")
                        .Include("AnswerUser")
                        .Include("Course")
                        .Include(x => x.Course.UserCourseRoleMappings)
                        .Include(x => x.Course.Questions)
                        .Include(x => x.Course.TutorialGroups)
                        .FirstOrDefault();
                    if (dbQuestion != null)
                    {
                        dbQuestion.Status = question.Status;
                        dbQuestion.AnswerText = question.AnswerText;
                        dbQuestion.AnswerMessageId = question.AnswerMessageId;
                        dbQuestion.AnswerPosterId = question.AnswerPosterId;
                        dbQuestion.DateAnswered = question.DateAnswered;
                        dbQuestion.AnswerCardActivityId = question.AnswerCardActivityId;
                        entities.SaveChanges();
                    }
                    else
                    {
                        entities.Questions.Add(question);
                    }

                    entities.SaveChanges();
                    return question.Id;
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return 0;
                }
            }
        }

        public static Question GetQuestionById(int id)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var questions =
                        entities.Questions
                        .Include("QuestionUser")
                        .Include("AnswerUser")
                        .Include("Course")
                        .Include(x => x.Course.UserCourseRoleMappings)
                        .Include(x => x.Course.Questions)
                        .Include(x => x.Course.TutorialGroups)
                        .Where(x => x.Id == id);
                    if (questions != null)
                    {
                        return questions.FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static Question GetQuestionByMessageId(string messageId)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var questions =
                        entities.Questions
                        .Include("QuestionUser")
                        .Include("AnswerUser")
                        .Include("Attachments")
                        .Include("QRQuestionLookup")
                        .Where(x => x.MessageId == messageId);
                    if (questions != null)
                    {
                        return questions.FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static Question GetQuestionByConversationId(string conversationId)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var questions = entities.Questions
                        .Include("QuestionUser")
                        .Include("AnswerUser")
                        .Where(x => x.ConversationId == conversationId);
                    if (questions != null)
                    {
                        return questions.FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static List<Question> GetAllQuestions(string tenantId = null)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    List<Question> questions;

                    if (tenantId != null)
                    {
                        questions = entities.Questions
                            .Include("QuestionUser")
                            .Include("AnswerUser")
                            .Where(q => q.TenantId == new Guid(tenantId))
                            .OrderBy(q => q.Id)
                            .ToList();
                    }
                    else
                    {
                        questions = entities.Questions
                            .Include("QuestionUser")
                            .Include("AnswerUser")
                            .OrderBy(q => q.Id)
                            .ToList();
                    }

                    return questions;
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns a list of O365 Group IDs where a question has been asked, which also matches the list of Group Ids passed in
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="memberOf">List of all Group Ids to filter by</param>
        /// <returns>List of all O365 Group IDs</returns>
        public static List<string> GetTeamGroupIdsWithQuestions(string tenantId, List<string> memberOf)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var groupIds = entities.Questions
                            .Where(q => q.TenantId == new Guid(tenantId))
                            .OrderBy(q => q.Id)
                            .Select(x => x.GroupId)
                            .Distinct()
                            .ToList();

                    var result = groupIds.Where(g1 => memberOf.Any(g2 => new Guid(g2) == g1)).Select(guid => guid.ToString()).ToList();

                    return result;
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static List<Question> GetQuestionsByTutorialGroup(string tenantId, string tutorialGroup)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var questions =
                        (from q in entities.Questions
                         join u in entities.Users
                             on q.OriginalPosterId equals u.Id
                         join tgm in entities.TutorialGroupMemberships
                             on u.Id equals tgm.UserId
                         join tg in entities.TutorialGroups
                             on tgm.TutorialGroupId equals tg.Id
                         where tg.Code == tutorialGroup && q.TenantId == new Guid(tenantId)
                         select q)
                        .OrderBy(q => q.Id)
                        .ToList();
                    return questions;
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static List<Question> GetQuestionsByGroup(string groupId)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var questions = entities.Questions
                            .Include("QuestionUser")
                            .Include("AnswerUser")
                            .Where(q => q.GroupId == new Guid(groupId))
                            .OrderBy(q => q.Id)
                            .ToList();

                    return questions;
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }
        #endregion

        #region User
        public static User GetUserByUPN(string upn)
        {
            using (var entities = new QBotEntities())
            {
                return entities.Users.FirstOrDefault(u => u.UserPrincipalName == upn);
            }
        }

        public static UserCourseRoleMapping GetUserMappingsByUPN(string upn)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var users = entities.UserCourseRoleMappings
                        .Include(x => x.User)
                        .Include(x => x.User.TutorialGroupMemberships)
                        .Include(x => x.User.TutorialGroupMemberships.Select(t => t.TutorialGroup))
                        .Include(x => x.User.AnswerPoster)
                        .Include(x => x.User.OriginalPoster)
                        .Include(x => x.User.UserCourseRoleMappings)
                        .Include(x => x.Role)
                        .Include(x => x.Role.UserCourseRoleMappings)
                        .Include(x => x.Course)
                        .Include(x => x.Course.Questions)
                        .Include(x => x.Course.TutorialGroups)
                        .Include(x => x.Course.UserCourseRoleMappings)
                        .Where(x => x.User.UserPrincipalName == upn).Distinct();
                    if (users != null)
                    {
                        return users.FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static List<UserCourseRoleMapping> GetUsersByUPN(string upn)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var users = entities.UserCourseRoleMappings
                        .Include(x => x.User)
                        .Include(x => x.User.TutorialGroupMemberships)
                        .Include(x => x.User.AnswerPoster)
                        .Include(x => x.User.OriginalPoster)
                        .Include(x => x.User.UserCourseRoleMappings)
                        .Include(x => x.Role)
                        .Include(x => x.Role.UserCourseRoleMappings)
                        .Include(x => x.Course)
                        .Include(x => x.Course.Questions)
                        .Include(x => x.Course.TutorialGroups)
                        .Include(x => x.Course.UserCourseRoleMappings)
                        .Where(x => x.User.UserPrincipalName == upn).Distinct();

                    if (users != null)
                    {
                        return users.ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static UserCourseRoleMapping GetUserById(int id)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var users = entities.UserCourseRoleMappings
                        .Where(x => x.UserId == id);

                    if (users != null)
                    {
                        return users.FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static bool UpdateUser(User user)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var userId = user.Id;
                    entities.Users.AddOrUpdate(user);
                    entities.SaveChanges();
                    return user.Id == userId;
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return false;
                }
            }
        }

        public static List<UserCourseRoleMapping> GetUsersByRole(string role, int courseID)
        {
            using (var entities = new QBotEntities())
            {
                try
                {

                    return entities.UserCourseRoleMappings
                        .Include(x => x.User)
                        .Include(x => x.User.TutorialGroupMemberships)
                        .Include(x => x.User.AnswerPoster)
                        .Include(x => x.User.OriginalPoster)
                        .Include(x => x.User.UserCourseRoleMappings)
                        .Include(x => x.Role)
                        .Include(x => x.Role.UserCourseRoleMappings)
                        .Include(x => x.Course)
                        .Include(x => x.Course.Questions)
                        .Include(x => x.Course.TutorialGroups)
                        .Include(x => x.Course.UserCourseRoleMappings)
                        .Where(x => x.CourseId == courseID && x.Role.Name == role).ToList();
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static bool NoUsers()
        {
            using (var entities = new QBotEntities())
            {
                return !entities.Users.Any(x => x.FirstName != "Question" && x.LastName != "Bot");
            }
        }

        public static UserCourseRoleMapping GetBotUser(int courseID)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var ucrm = entities.UserCourseRoleMappings
                          .Include(x => x.User)
                        .Include(x => x.User.TutorialGroupMemberships)
                        .Include(x => x.User.AnswerPoster)
                        .Include(x => x.User.OriginalPoster)
                        .Include(x => x.User.UserCourseRoleMappings)
                        .Include(x => x.Role)
                        .Include(x => x.Role.UserCourseRoleMappings)
                        .Include(x => x.Course)
                        .Include(x => x.Course.Questions)
                        .Include(x => x.Course.TutorialGroups)
                        .Include(x => x.Course.UserCourseRoleMappings)
                        .Where(x => x.CourseId == courseID && x.RoleId == 4).FirstOrDefault();

                    if (ucrm == null)
                    {
                        var user = entities.Users.Where(x => x.FirstName == "Question" && x.LastName == "Bot").FirstOrDefault();
                        if (user == null)
                        {
                            user = new User()
                            {
                                FirstName = "Question",
                                LastName = "Bot",
                            };
                            entities.Users.Add(user);
                            entities.SaveChanges();
                        }

                        ucrm = new UserCourseRoleMapping()
                        {
                            UserId = user.Id,
                            CourseId = courseID,
                            RoleId = 4,
                        };
                        entities.UserCourseRoleMappings.Add(ucrm);
                        entities.SaveChanges();
                    }

                    return ucrm;
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static List<User> GetUsersByTutorialCode(string code)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    entities.Configuration.ProxyCreationEnabled = false;
                    return entities.TutorialGroupMemberships
                        .Where(x => x.TutorialGroup.Code == code)
                        .Select(x => x.User).ToList();
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static List<UserCourseRoleMapping> GetDemonstrators(int courseId, int tutorialId)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var demonstrators = entities.UserCourseRoleMappings.Include(x => x.User)
                        .Include(x => x.User.TutorialGroupMemberships)
                        .Include(x => x.User.AnswerPoster)
                        .Include(x => x.User.OriginalPoster)
                        .Include(x => x.User.UserCourseRoleMappings)
                        .Include(x => x.Role)
                        .Include(x => x.Role.UserCourseRoleMappings)
                        .Include(x => x.Course)
                        .Include(x => x.Course.Questions)
                        .Include(x => x.Course.TutorialGroups)
                        .Include(x => x.Course.UserCourseRoleMappings)
                        .Where(x => 
                            x.CourseId == courseId && 
                            x.RoleId == 2 && 
                            x.User.TutorialGroupMemberships.Any(t => t.TutorialGroupId == tutorialId)
                        ).ToList();
                    return demonstrators;
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        #endregion

        #region Tutorials
        public static List<TutorialGroup> GetAllTutorialGroups()
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var tutorialGroups = entities.TutorialGroups.ToList();
                    return tutorialGroups;
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static List<User> SetUsers(List<User> users)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var u = entities.Users.AddRange(users);
                    entities.SaveChanges();

                    return users.ToList();
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static List<TutorialGroupMembership> SetUserTutorialGroups(List<TutorialGroupMembership> tutorialGroupMembership)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var utg = entities.TutorialGroupMemberships.AddRange(tutorialGroupMembership);
                    entities.SaveChanges();

                    return utg.ToList();
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }
        #endregion

        #region Course
        public static Course GetCourse(int courseID)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var course = entities.Courses.Where(x => x.Id == courseID).FirstOrDefault();
                    return course;
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static int GetCourseIDByName(string name)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    var course = entities.Courses.Where(x => x.Name == name).FirstOrDefault();
                    return course.Id;
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return 0;
                }
            }
        }

        public static List<Course> GetCourses(string cs = null)
        {
            // if a connectionstring is manually provided, ensure the connectionstring is passed to the EF constructor
            // this is used in the Function App
            using (var entities = cs == null ? new QBotEntities() : new QBotEntities(cs))
            {
                try
                {
                    return entities.Courses.ToList();
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static List<Course> SaveCourse(Course course)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    if (course.Id == 0)//new course
                    {
                        entities.Courses.Add(course);
                    }
                    else
                    {
                        Course editCourse = entities.Courses.Where(x => x.Id == course.Id).FirstOrDefault();
                        editCourse.Name = course.Name;
                        editCourse.PredictiveQnAServiceHost = course.PredictiveQnAServiceHost;
                        editCourse.PredictiveQnAServiceHost = course.PredictiveQnAServiceHost;
                        editCourse.PredictiveQnAKnowledgeBaseId = course.PredictiveQnAKnowledgeBaseId;
                        editCourse.PredictiveQnAEndpointKey = course.PredictiveQnAEndpointKey;
                        editCourse.PredictiveQnAHttpEndpoint = course.PredictiveQnAHttpEndpoint;
                        editCourse.PredictiveQnAHttpKey = course.PredictiveQnAHttpKey;
                        editCourse.PredictiveQnAKnowledgeBaseName = course.PredictiveQnAKnowledgeBaseName;
                        editCourse.PredictiveQnAConfidenceThreshold = course.PredictiveQnAConfidenceThreshold;
                        editCourse.GroupId = course.GroupId;
                    }
                    entities.SaveChanges();
                    return entities.Courses.ToList();
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static List<Course> DeleteCourse(int id)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    Course editCourse = entities.Courses.Where(x => x.Id == id).FirstOrDefault();
                    if (editCourse != null)
                    {
                        var ucrmToDelete = from ucrm in entities.UserCourseRoleMappings where ucrm.CourseId == editCourse.Id select ucrm;
                        if (ucrmToDelete != null)
                        {
                            entities.UserCourseRoleMappings.RemoveRange(ucrmToDelete);
                        }
                        var tutorialGroupMembershipsToDelete = from tgm in entities.TutorialGroupMemberships where tgm.TutorialGroup.CourseId == id select tgm;
                        if (tutorialGroupMembershipsToDelete != null)
                        {
                            entities.TutorialGroupMemberships.RemoveRange(tutorialGroupMembershipsToDelete);
                        }
                        var tutorialGroupsToDelete = from tg in entities.TutorialGroups where tg.CourseId == id select tg;
                        if (tutorialGroupsToDelete != null)
                        {
                            entities.TutorialGroups.RemoveRange(tutorialGroupsToDelete);
                        }
                        entities.Courses.Remove(editCourse);
                        entities.SaveChanges();
                    }
                    return entities.Courses.ToList();
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }
        #endregion

        #region students
        public static List<UserCourseRoleMapping> AddStudents(List<User> students, string coursename)
        {
            try
            {
                using (var entities = new QBotEntities())
                {

                    Course course = entities.Courses.Where(x => x.Name == coursename).FirstOrDefault();
                    int studentRoleId = entities.Roles.Where(x => x.Name == "Student").FirstOrDefault().Id;
                    foreach (User student in students)
                    {
                        int studentID = 0;
                        if (course != null)
                        {
                            User studentToUpdate = entities.Users.Where(x => x.UserPrincipalName == student.UserPrincipalName).FirstOrDefault();

                            // Existing student
                            if (studentToUpdate != null)
                            {
                                studentToUpdate.FirstName = student.FirstName;
                                studentToUpdate.LastName = student.LastName;
                                studentToUpdate.UserPrincipalName = student.UserPrincipalName;
                                studentToUpdate.Email = student.Email;
                                studentID = studentToUpdate.Id;
                            }
                            else
                            {
                                User newStudent = new User
                                {
                                    StudentId = student.StudentId,
                                    FirstName = student.FirstName,
                                    LastName = student.LastName,
                                    UserPrincipalName = student.UserPrincipalName,
                                    Email = student.Email,
                                };
                                entities.Users.Add(newStudent);

                                // Need this line to generate identity
                                entities.SaveChanges();
                                studentID = newStudent.Id;
                            }

                            // Write course role mapping 
                            UserCourseRoleMapping ucrm = entities.UserCourseRoleMappings.Where(x => x.CourseId == course.Id && x.UserId == studentID).FirstOrDefault();
                            if (ucrm == null)
                            {
                                ucrm = new UserCourseRoleMapping
                                {
                                    UserId = studentID,
                                    CourseId = course.Id,
                                    RoleId = studentRoleId,
                                };
                                entities.UserCourseRoleMappings.Add(ucrm);
                                entities.SaveChanges();
                            }
                        }
                    }

                    entities.SaveChanges();
                    return entities.UserCourseRoleMappings
                        .Include(x => x.User)
                        .Include(x => x.User.TutorialGroupMemberships)
                        .Include(x => x.User.TutorialGroupMemberships.Select(t => t.TutorialGroup))
                        .Include(x => x.User.AnswerPoster)
                        .Include(x => x.User.OriginalPoster)
                        .Include(x => x.User.UserCourseRoleMappings)
                        .Include(x => x.Role)
                        .Include(x => x.Role.UserCourseRoleMappings)
                        .Include(x => x.Course)
                        .Include(x => x.Course.Questions)
                        .Include(x => x.Course.TutorialGroups)
                        .Include(x => x.Course.UserCourseRoleMappings)
                        .Where(x => x.CourseId == course.Id).ToList();
                }
            }
            catch (Exception e)
            {
                LogExceptions(e);
                return null;
            }
        }
        #endregion

        public static List<UserCourseRoleMapping> GetUserCourseRoleMappingsByCourse(int courseId)
        {
            try
            {
                using (var entities = new QBotEntities())
                {
                    var ucrms = entities.UserCourseRoleMappings.Where(x => x.CourseId == courseId && x.RoleId != 4)
                        .Include(x => x.User)
                        .Include(x => x.User.TutorialGroupMemberships)
                        .Include(x => x.User.TutorialGroupMemberships.Select(t => t.TutorialGroup))
                        .Include(x => x.Course)
                        .Include(x => x.Role)
                        .ToList();
                    foreach (UserCourseRoleMapping u in ucrms)
                    {
                        if (u.User.TutorialGroupMemberships?.Count > 0)
                        {
                            var tgmsToRemove = new List<TutorialGroupMembership>();
                            foreach (TutorialGroupMembership tgm in u.User.TutorialGroupMemberships)
                            {
                                if (tgm.TutorialGroup.CourseId != courseId)
                                {
                                    tgmsToRemove.Add(tgm);
                                }
                            }

                            if (tgmsToRemove != null)
                            {
                                foreach (TutorialGroupMembership tgms in tgmsToRemove)
                                {
                                    u.User.TutorialGroupMemberships.Remove(tgms);
                                }
                            }
                        }
                    }

                    return ucrms;
                }
            }
            catch (Exception e)
            {
                LogExceptions(e);
                return null;
            }
        }

        public static List<UserCourseRoleMapping> DeleteUserCourseRoleMapping(UserCourseRoleMapping ucrm)
        {
            try
            {
                using (var entities = new QBotEntities())
                {
                    int courseId = ucrm.CourseId;
                    var tutorialGroupMappingsToDelete = from tutorialMapping in entities.TutorialGroupMemberships
                                                        where tutorialMapping.UserId == ucrm.UserId
                                                        select tutorialMapping;
                    if (tutorialGroupMappingsToDelete != null)
                    {
                        entities.TutorialGroupMemberships.RemoveRange(tutorialGroupMappingsToDelete);
                    }

                    var ucrms = entities.UserCourseRoleMappings.Where(x => x.Id == ucrm.Id).FirstOrDefault();
                    entities.UserCourseRoleMappings.Remove(ucrms);
                    entities.SaveChanges();
                    var updatedList = entities.UserCourseRoleMappings.Where(x => x.CourseId == courseId)
                        .Include(x => x.User)
                        .Include(x => x.User.TutorialGroupMemberships)
                        .Include(x => x.User.TutorialGroupMemberships.Select(t => t.TutorialGroup))
                        .Include(x => x.Course)
                        .Include(x => x.Role)
                        .ToList();
                    return updatedList;
                }
            }
            catch (Exception e)
            {
                LogExceptions(e);
                return null;
            }
        }

        public static List<UserCourseRoleMapping> SaveUserCourseRoleMapping(UserCourseRoleMapping ucrm, List<int> tutorialIds)
        {
            try
            {
                using (var entities = new QBotEntities())
                {
                    int courseId = ucrm.CourseId;
                    var ucrmToUpdate = entities.UserCourseRoleMappings.Where(x => x.Id == ucrm.Id).FirstOrDefault();
                    if (ucrm == null)
                    {
                        ucrmToUpdate = new UserCourseRoleMapping();
                        ucrmToUpdate.UserId = ucrm.UserId;
                        ucrmToUpdate.CourseId = courseId;
                    }

                    ucrmToUpdate.RoleId = ucrm.RoleId;
                    entities.UserCourseRoleMappings.AddOrUpdate(ucrmToUpdate);

                    // Reset all tutorial group memberships for this course
                    List<TutorialGroupMembership> membershipsToDelete = entities.TutorialGroupMemberships
                        .Where(x => x.UserId == ucrm.UserId && x.TutorialGroup.CourseId == courseId)
                        .ToList();

                    if (membershipsToDelete != null)
                    {
                        entities.TutorialGroupMemberships.RemoveRange(membershipsToDelete);
                    }

                    // Add in tutorial group memberships from the UI
                    foreach (int i in tutorialIds)
                    {
                        entities.TutorialGroupMemberships.Add(new TutorialGroupMembership()
                        {
                            TutorialGroupId = i,
                            UserId = ucrm.UserId,
                        });
                    }

                    entities.SaveChanges();

                    var updatedList = entities.UserCourseRoleMappings.Where(x => x.CourseId == courseId)
                        .Include(x => x.User)
                        .Include(x => x.User.TutorialGroupMemberships)
                        .Include(x => x.User.TutorialGroupMemberships.Select(t => t.TutorialGroup))
                        .Include(x => x.Course)
                        .Include(x => x.Role)
                        .ToList();
                    return updatedList;
                }
            }
            catch (Exception e)
            {
                LogExceptions(e);
                return null;
            }
        }

        public static List<Role> GetRoles()
        {
            try
            {
                using (var entities = new QBotEntities())
                {
                    return entities.Roles.Where(x => x.Name != "Bot")
                        .ToList();
                }
            }
            catch (Exception e)
            {
                LogExceptions(e);
                return null;
            }
        }

        public static List<TutorialGroup> GetTutorialsByCourse(int courseId)
        {
            try
            {
                using (var entities = new QBotEntities())
                {
                    var tutorialGroups = entities.TutorialGroups.Where(x => x.CourseId == courseId)
                        .ToList();
                    return tutorialGroups;
                }
            }
            catch (Exception e)
            {
                LogExceptions(e);
                return null;
            }
        }

        public static List<TutorialGroup> SaveTutorialGroup(TutorialGroup tutorial)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    if (tutorial.Id == 0)
                    {
                        entities.TutorialGroups.Add(tutorial);
                    }
                    else
                    {
                        TutorialGroup editTutorial = entities.TutorialGroups.Where(x => x.Id == tutorial.Id).FirstOrDefault();
                        editTutorial.Name = tutorial.Name;
                        editTutorial.Code = tutorial.Code;
                        editTutorial.CourseId = tutorial.CourseId;
                    }
                    entities.SaveChanges();
                    return entities.TutorialGroups.Where(x => x.CourseId == tutorial.CourseId).ToList();
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        public static List<TutorialGroup> DeleteTutorialGroup(TutorialGroup tutorial)
        {
            using (var entities = new QBotEntities())
            {
                try
                {
                    int CourseId = tutorial.CourseId;
                    TutorialGroup tutorialGroupToDelete = entities.TutorialGroups.Where(x => x.Id == tutorial.Id).FirstOrDefault();
                    if (tutorialGroupToDelete != null)
                    {
                        var tutorialGroupMappingsToDelete = from tutorialMapping in entities.TutorialGroupMemberships
                                                            where tutorialMapping.TutorialGroupId == tutorialGroupToDelete.Id
                                                            select tutorialMapping;
                        if (tutorialGroupMappingsToDelete != null)
                        {
                            entities.TutorialGroupMemberships.RemoveRange(tutorialGroupMappingsToDelete);
                        }

                        entities.TutorialGroups.Remove(tutorialGroupToDelete);
                        entities.SaveChanges();
                    }

                    return entities.TutorialGroups.Where(x => x.CourseId == CourseId).ToList();
                }
                catch (Exception e)
                {
                    LogExceptions(e);
                    return null;
                }
            }
        }

        private static void LogExceptions(Exception e)
        {
            System.Diagnostics.Trace.WriteLine(e.ToString());
            if (e.InnerException != null)
            {
                System.Diagnostics.Trace.WriteLine(e.InnerException.ToString());
            }
        }
    }
}
