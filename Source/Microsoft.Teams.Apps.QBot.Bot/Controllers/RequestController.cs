using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

using Microsoft.Teams.Apps.QBot.Bot.Services;
using Microsoft.Teams.Apps.QBot.Data;
using Microsoft.Teams.Apps.QBot.Model;
using Microsoft.Teams.Apps.QBot.Model.Graph;

namespace Microsoft.Teams.Apps.QBot.Bot.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Request")]
    public class RequestController : ApiController
    {
        [HttpPost]
        [Route("InitializeBotService")]
        public async Task<IHttpActionResult> InitializeBotService()
        {
            if (SQLService.NoUsers())
            {
                // Bootstraps the initial admin user for QBot system
                string upn = null;
                string firstName = null;
                string lastName = null;
                string email = null;

                var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    upn = claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.Upn).Select(c => c.Value).FirstOrDefault();
                    if (string.IsNullOrEmpty(upn))
                    {
                        upn = claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).FirstOrDefault();
                    }
                    if (string.IsNullOrEmpty(upn))
                    {
                        upn = claimsIdentity.Claims.Where(c => c.Type == "name").Select(c => c.Value).FirstOrDefault();
                    }

                    firstName = claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.GivenName).Select(c => c.Value).FirstOrDefault();
                    lastName = claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.Surname).Select(c => c.Value).FirstOrDefault();
                    email = claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).FirstOrDefault();
                }

                if (string.IsNullOrEmpty(upn))
                {
                    throw new Exception("Error bootstrapping initial Bot admin account - Could not get current user claim");
                }

                // First user in the system, they are made as global admin
                SQLService.AddGlobalAdminUser(upn, firstName, lastName, email);
                Trace.WriteLine("Bootstrapped initial user to database: " + upn);
            }

            return Ok();
        }

        [HttpGet]
        [Route("GetUserAccess")]
        public async Task<UserAccessModel> GetUserAccess(string upn)
        {
            return SQLService.GetUserAccess(upn);
        }

        /// <summary>
        /// Gets all Questions for all Courses within the given tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of Questions</returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("GetAllQuestions")]
        public List<QuestionModel> GetAllQuestions([FromBody]string tenantId)
        {
            var questions = SQLService.GetAllQuestions(tenantId);

            return questions;
        }

        /// <summary>
        /// Gets all Questions for the given Group
        /// </summary>
        /// <param name="groupId">Office 365 Group ID</param>
        /// <returns>List of Questions</returns>
        [HttpPost]
        [Route("GetQuestionsByGroup")]
        public List<QuestionModel> GetQuestionsByGroup([FromBody]string groupId)
        {
            var questions = SQLService.GetQuestionsByGroup(groupId);

            return questions;
        }

        /// <summary>
        /// Gets all Questions for the given Tutorial Group
        /// </summary>
        /// <param name="obj">Contains Tenant ID and Tutorial Group Code</param>
        /// <returns>List of Questions</returns>
        [HttpPost]
        [Route("GetQuestionsByTutorial")]
        public List<QuestionModel> GetQuestionsByTutorial([FromBody]dynamic obj)
        {
            var tenantId = obj["tenantId"].ToObject<string>();
            var code = obj["code"].ToObject<string>();
            var questions = SQLService.GetAllQuestionsByTutorial(tenantId, code);

            return questions;
        }

        /// <summary>
        /// Get a User by their User Principal Name
        /// </summary>
        /// <param name="upn">User Principal Name</param>
        /// <returns>User record</returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("GetUserByUpn")]
        public UserCourseRoleMappingModel GetUserByUpn([FromBody]string upn)
        {
            var user = SQLService.GetUser(upn);

            return user;
        }

        /// <summary>
        /// Gets list of Students for a given Tutorial Group
        /// </summary>
        /// <param name="tutorialCode">The Tutorial Group Code</param>
        /// <returns>User record</returns>
        [HttpPost]
        [Route("GetStudentsByTutorial")]
        public List<UserCourseRoleMappingModel> GetStudentsByTutorial([FromBody]string tutorialCode)
        {
            var users = SQLService.GetStudents(tutorialCode);

            return users;
        }

        /// <summary>
        /// Gets a list of all O365 Group Ids for which the given user is a member of, which also contains questions
        /// </summary>
        /// <param name="obj">Parameter contains the Tenant ID and UPN</param>
        /// <returns>List of Team Group IDs where a question is asked</returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("GetTeamGroupIdsWithQuestions")]
        public async Task<List<string>> GetTeamGroupIdsWithQuestions([FromBody]dynamic obj)
        {
            try
            {
                var tenantId = obj["tenantId"].ToObject<string>();
                var upn = obj["upn"].ToObject<string>();

                var authService = new AuthService(ServiceHelper.Authority);
                var authResult = await authService.AuthenticateSilently(ServiceHelper.GraphResource);
                if (authResult != null)
                {
                    var graphService = new GraphService();
                    var memberOf = await graphService.GetGroupMemberOf(authResult.AccessToken, upn);
                    var groupIds = SQLService.GetTeamGroupIdsWithQuestions(tenantId, memberOf);

                    return groupIds;
                }

                return null;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Gets details such as name, description, photo, etc about a specific Team
        /// </summary>
        /// <param name="groupId">The O365 Group ID</param>
        /// <returns>List of Groups</returns>
        [HttpPost]
        [Route("GetTeamGroupDetail")]
        public async Task<TeamGroupDetail> GetTeamGroupDetail([FromBody]string groupId)
        {
            var teamGroupDetail = new TeamGroupDetail();

            var authService = new AuthService(ServiceHelper.Authority);
            var authResult = await authService.AuthenticateSilently(ServiceHelper.GraphResource);
            if (authResult != null)
            {
                var graphService = new GraphService();
                var teamGroup = await graphService.GetTeamGroupSPUrl(authResult.AccessToken, groupId);

                teamGroupDetail.Id = groupId;
                teamGroupDetail.Name = teamGroup.Name;
                teamGroupDetail.DisplayName = teamGroup.DisplayName;
                teamGroupDetail.Description = teamGroup.Description;
                teamGroupDetail.WebUrl = teamGroup.WebUrl;

                var imageBytes = await graphService.GetGroupPhoto(authResult.AccessToken, groupId);
                if (imageBytes != null)
                {
                    var picUrl = Convert.ToBase64String(imageBytes);
                    teamGroupDetail.PhotoByteUrl = picUrl;
                }

                return teamGroupDetail;
            }

            return null;
        }

        /// <summary>
        /// Gets the list of Channels for the given Team
        /// </summary>
        /// <param name="groupId">Office 365 Group ID</param>
        /// <returns>A list of channels within the Team</returns>
        [HttpPost]
        [Route("GetTeamChannels")]
        public async Task<List<TeamChannel>> GetTeamChannels([FromBody]string groupId)
        {
            var teamGroupDetail = new TeamGroupDetail();

            var authService = new AuthService(ServiceHelper.Authority);
            var authResult = await authService.AuthenticateSilently(ServiceHelper.GraphResource);
            if (authResult != null)
            {
                var graphService = new GraphService();
                var teamChannels = await graphService.GetChannels(authResult.AccessToken, groupId);
                return teamChannels;
            }

            return null;
        }

        /// <summary>
        /// Gets the Team photo
        /// </summary>
        /// <param name="groupId">Office 365 Group ID</param>
        /// <returns>Base64 encoded representation of the Team photo</returns>
        [HttpPost]
        [Route("GetTeamGroupPhoto")]
        public async Task<string> GetTeamGroupPhoto([FromBody]string groupId)
        {
            var authService = new AuthService(ServiceHelper.Authority);
            var authResult = await authService.AuthenticateSilently(ServiceHelper.GraphResource);
            if (authResult != null)
            {
                var graphService = new GraphService();
                var imageBytes = await graphService.GetGroupPhoto(authResult.AccessToken, groupId);
                if (imageBytes != null)
                {
                    var picUrl = @"data:image/png;base64," + Convert.ToBase64String(imageBytes);
                    return picUrl;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all Courses
        /// </summary>
        /// <returns>List of all Courses</returns>
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("GetCourses")]
        public List<CourseModel> GetCourses()
        {
            try
            {
                var courses = SQLService.GetCourses();
                return ModelMapper.MapToCourseModels(courses);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Adds or updates a Course record
        /// </summary>
        /// <param name="course">Course model to be saved</param>
        /// <returns>The full list of courses after the opearation completes</returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("SaveCourse")]
        public async Task<List<CourseModel>> SaveCourse(Course course)
        {
            try
            {
                var courses = SQLService.SaveCourse(course);
                if (courses != null)
                {
                    var authService = new AuthService(ServiceHelper.Authority);
                    var authResult = await authService.AuthenticateSilently(ServiceHelper.GraphResource);
                    if (authResult != null)
                    {
                        var graphService = new GraphService();
                        var graphUsers = await graphService.GetMembers(authResult.AccessToken, course.GroupId.ToString());
                        if (graphUsers != null)
                        {
                            var courseId = SQLService.GetCourseIDByName(course.Name);
                            List<User> users = new List<User>();
                            foreach (GraphUser graphUser in graphUsers.Items)
                            {
                                users.Add(new User()
                                {
                                    Id = 0,
                                    StudentId = graphUser.id,
                                    FirstName = graphUser.givenName,
                                    LastName = graphUser.surname,
                                    UserPrincipalName = graphUser.userPrincipalName,
                                    Email = graphUser.mail,
                                });
                            }

                            var result = SQLService.AddStudents(users, course.Name);
                        }
                    }
                }

                return ModelMapper.MapToCourseModels(courses);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Synchronises Users based on Teams membership for a course
        /// </summary>
        /// <param name="course">The course record</param>
        /// <returns>List of Users after the operation completes</returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("RefreshUsers")]
        public async Task<List<UserCourseRoleMappingModel>> RefreshUsers(Course course)
        {
            try
            {
                var result = new List<UserCourseRoleMappingModel>();
                var authService = new AuthService(ServiceHelper.Authority);

                // BUG: On some EDU tenants, listing group members return nothing when authenticated using Delegate permissions
                var authResult = await authService.AuthenticateSilently(ServiceHelper.GraphResource, "Application", true);
                if (authResult != null)
                {
                    var graphService = new GraphService();
                    var graphUsers = await graphService.GetMembers(authResult.AccessToken, course.GroupId.ToString());
                    if (graphUsers != null)
                    {
                        var courseId = course.Id;
                        List<User> users = new List<User>();
                        foreach (GraphUser graphUser in graphUsers.Items)
                        {
                            users.Add(new User()
                            {
                                Id = 0,
                                StudentId = graphUser.id,
                                FirstName = graphUser.givenName,
                                LastName = graphUser.surname,
                                UserPrincipalName = graphUser.userPrincipalName,
                                Email = graphUser.mail,
                            });
                        }

                        Trace.WriteLine(string.Format("RefreshUsers - Got {0} members to sync", users.Count));

                        result = ModelMapper.MapToUsersModel(SQLService.AddStudents(users, course.Name));
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Deletes a Course
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <returns>List of Courses after the operation completes</returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("DeleteCourse")]
        public List<CourseModel> DeleteCourse([FromBody]int id)
        {
            return ModelMapper.MapToCourseModels(SQLService.DeleteCourse(id));
        }

        /// <summary>
        /// Adds a list of Student to a Course. User records will be updated if they exist, otherwise they are added for the given Course
        /// </summary>
        /// <param name="students">List of students to update</param>
        /// <param name="coursename">The course that the students belong to</param>
        /// <returns>List of Students after the operation completes</returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("AddStudents")]
        public List<UserCourseRoleMappingModel> AddStudents(List<UserCourseRoleMappingModel> students, string coursename)
        {
            try
            {
                List<User> mappedStudents = ModelMapper.MapFromUserModels(students);
                var user = SQLService.AddStudents(mappedStudents, coursename);
                List<UserCourseRoleMappingModel> studentsToReturn = ModelMapper.MapToUsersModel(user);
                return studentsToReturn;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all Teams that a given user currently owns
        /// </summary>
        /// <param name="obj">Parameter contains the Tenant ID and the UPN</param>
        /// <returns>List of Teams that the user owns</returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("GetOwnedTeams")]
        public async Task<GraphTeamsOwnedResult> GetOwnedTeams([FromBody]dynamic obj)
        {
            try
            {
                var tenantId = obj["tenantId"].ToObject<string>();
                var upn = obj["upn"].ToObject<string>();
                var authService = new AuthService(ServiceHelper.Authority);
                var authResult = await authService.AuthenticateSilently(ServiceHelper.GraphResource);
                if (authResult != null)
                {
                    var graphService = new GraphService();
                    var ownerOf = await graphService.GetOwnedObjects(authResult.AccessToken, upn);
                    return ownerOf;
                }
                return null;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Get Users for a Course
        /// </summary>
        /// <param name="courseid">Course ID</param>
        /// <returns>List of Users</returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("GetUserCourseRoleMappingsByCourse")]
        public async Task<List<UserCourseRoleMappingModel>> GetUserCourseRoleMappingsByCourse([FromBody]int courseid)
        {
            try
            {
                return SQLService.GetUserCourseRoleMappingsByCourse(courseid);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Deletes a User for a Course
        /// </summary>
        /// <param name="ucrm">User Role mapping that contains the User record & course they belong to</param>
        /// <returns>A list of Users for the course, after this operation completes</returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("DeleteUserCourseRoleMapping")]
        public async Task<List<UserCourseRoleMappingModel>> DeleteUserCourseRoleMapping([FromBody]UserCourseRoleMapping ucrm)
        {
            try
            {
                return SQLService.DeleteUserCourseRoleMapping(ucrm);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Saves a User for a Course
        /// </summary>
        /// <param name="ucrm">User Role mapping that contains the User record & course they belong to</param>
        /// <returns>A list of Users for the course, after this operation completes</returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("SaveUserCourseRoleMapping")]
        public async Task<List<UserCourseRoleMappingModel>> SaveUserCourseRoleMapping([FromBody]UserCourseRoleMappingModel ucrm)
        {
            try
            {
                return SQLService.SaveUserCourseRoleMapping(ucrm);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Gets all Roles within the system
        /// </summary>
        /// <returns>List of Roles</returns>
        [HttpGet]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("GetRoles")]
        public async Task<List<RoleModel>> GetRoles()
        {
            try
            {
                return SQLService.GetRoles();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Gets a list of Tutorial Groups within a Course
        /// </summary>
        /// <param name="courseid">The Course ID</param>
        /// <returns>The list of Tutorial Groups</returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("GetTutorialsByCourse")]
        public async Task<List<TutorialGroupModel>> GetTutorialsByCourse([FromBody]int courseid)
        {
            try
            {
                return SQLService.GetTutorialsByCourse(courseid);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Saves a Tutorial Group record
        /// </summary>
        /// <param name="tutorial">The Tutorial Group to save</param>
        /// <returns>The list of Tutorial Groups after the operation</returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("SaveTutorialGroup")]
        public async Task<List<TutorialGroupModel>> SaveTutorialGroup([FromBody]TutorialGroup tutorial)
        {
            try
            {
                return SQLService.SaveTutorialGroup(tutorial);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Deletes a Tutorial Group
        /// </summary>
        /// <param name="tutorial">The Tutorial Group to delete</param>
        /// <returns>The list of Tutorial Groups after the operation</returns>
        [HttpPost]
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [Route("DeleteTutorialGroup")]
        public async Task<List<TutorialGroupModel>> DeleteTutorialGroup([FromBody]TutorialGroup tutorial)
        {
            try
            {
                return SQLService.DeleteTutorialGroup(tutorial);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }
    }
}