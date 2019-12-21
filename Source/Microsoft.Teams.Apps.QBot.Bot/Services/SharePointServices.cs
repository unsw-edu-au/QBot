using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Teams.Apps.QBot.Data;
using Microsoft.Teams.Apps.QBot.Model;

namespace Microsoft.Teams.Apps.QBot.Bot.Services
{
    public static class SharePointServices
    {
        private static SharePointListAdapter adapter;

        private static void Init()
        {
            if (adapter == null)
                adapter = new SharePointListAdapter(
                    ConfigurationManager.AppSettings["siteUrl"],
                    Convert.ToBoolean(ConfigurationManager.AppSettings["isOnPrem"]),
                    ConfigurationManager.AppSettings["username"],
                    ConfigurationManager.AppSettings["password"]
                );
        }

        public static void Init(string tenantUrl, string username, string password)
        {
            if (adapter == null)
                adapter = new SharePointListAdapter(
                    tenantUrl,
                    false,
                    username,
                    password
                );
        }

        #region Question
        public static int CreateOrUpdateQuestion(QuestionModel questionModel)
        {
            try
            {
                Init();

                var updateProperties = new Dictionary<string, string>();
                updateProperties.Add("ID", questionModel.ID.ToString());
                updateProperties.Add("QuestionText", questionModel.QuestionText.ToString());
                updateProperties.Add("OriginalPosterEmail", questionModel.OriginalPosterEmail.ToString());
                updateProperties.Add("Topic", questionModel.Topic.ToString());
                updateProperties.Add("Status", questionModel.QuestionStatus.ToString());
                updateProperties.Add("ConversationId", questionModel.ConversationId.ToString());

                return adapter.AddOrUpdateItem("Questions", updateProperties);
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
                Init();

                var p1 = @"<View><Query><Where><Eq><FieldRef Name='ID' /><Value Type='Counter'>";
                var p2 = @"</Value></Eq></Where></Query></View>";

                var camlQuery = p1 + questionId + p2;

                var questions = adapter.GetItems("Questions", camlQuery);

                var questionList = new List<QuestionModel>();
                foreach (var question in questions)
                {
                    questionList.Add(new QuestionModel()
                    {
                        ID = Convert.ToInt32(question["ID"].ToString()),
                        QuestionText = question["QuestionText"].ToString(),
                        QuestionStatus = question["Status"].ToString(),
                        OriginalPosterEmail = question["OriginalPosterEmail"].ToString(),
                        Topic = question["Topic"].ToString(),
                        ConversationId = question["ConversationId"].ToString()
                    });
                }

                var foundQuestion = questionList.FirstOrDefault();

                return foundQuestion;

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
                Init();

                var p1 = @"<View><Query><Where><Eq><FieldRef Name='ConversationId' /><Value Type='Text'>";
                var p2 = @"</Value></Eq></Where></Query></View>";

                var camlQuery = p1 + conversationId + p2;

                var questions = adapter.GetItems("Questions", camlQuery);

                var questionList = new List<QuestionModel>();
                foreach (var question in questions)
                {
                    questionList.Add(new QuestionModel()
                    {
                        ID = Convert.ToInt32(question["ID"].ToString()),
                        QuestionText = question["QuestionText"].ToString(),
                        QuestionStatus = question["Status"].ToString(),
                        OriginalPosterEmail = question["OriginalPosterEmail"].ToString(),
                        Topic = question["Topic"].ToString(),
                        ConversationId = question["ConversationId"].ToString()
                    });
                }

                return questionList.Count > 0;

            }
            catch (Exception e)
            {
                return false;
            }

        }
        #endregion

        #region User
        // What will we use to get the user? What do we have access to?
        public static UserModel GetUser(string userPrincipleName)
        {
            try
            {
                Init();

                var p1 = @"<View><Query><Where><Or>";

                var p2 = @"<Eq><FieldRef Name='Username' /><Value Type='Text'>" + userPrincipleName + @"</Value></Eq>";
                var p3 = @"<Eq><FieldRef Name='Email' /><Value Type='Text'>" + userPrincipleName + @"</Value></Eq>";

                var p4 = @"</Or></Where></Query></View>";

                //var p1 = @"<View><Query><Where><And>";

                //var p2 = @"</Value></Eq><And></Where></Query></View>";
                //var camlQuery = p1 + userPrincipleName + p2;
                var camlQuery = p1 + p2 + p3 + p4;

                var users = adapter.GetItems("Users", camlQuery);

                var userList = new List<UserModel>();
                foreach (var user in users)
                {
                    userList.Add(new UserModel()
                    {
                        ID = Convert.ToInt32(user["ID"].ToString()),
                        UserName = user["Username"].ToString(),
                        StudentID = user["StudentID"].ToString(),
                        LastName = user["LastName"].ToString(),
                        FirstName = user["FirstName"].ToString(),
                        Email = user["Email"].ToString(),
                        RoleName = user["Role"].ToString(),
                        TutorialGroupsString = SPUtilities.ParseLookup(user["TutorialGroup"] as FieldLookupValue[]),
                        //TeamsUserID = user["TeamsUserID"]?.ToString(),
                    });
                }

                var foundUser = userList.FirstOrDefault();

                return foundUser;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<UserModel> GetUsersByRole(string role)
        {
            try
            {
                Init();

                var p1 = @"<View><Query><Where><Eq><FieldRef Name='Role' /><Value Type='Choice'>";

                var p2 = @"</Value></Eq></Where></Query></View>";


                var camlQuery = p1 + role + p2;

                var users = adapter.GetItems("Users", camlQuery);

                var userList = new List<UserModel>();
                foreach (var user in users)
                {
                    userList.Add(new UserModel()
                    {
                        ID = Convert.ToInt32(user["ID"].ToString()),
                        UserName = user["Username"].ToString(),
                        StudentID = user["StudentID"].ToString(),
                        LastName = user["LastName"].ToString(),
                        FirstName = user["FirstName"].ToString(),
                        Email = user["Email"].ToString(),
                        RoleName = user["Role"].ToString(),
                        TutorialGroupsString = SPUtilities.ParseLookup(user["TutorialGroup"] as FieldLookupValue[]),
                        //TeamsUserID = user["TeamsUserID"]?.ToString(),
                    });
                }

                return userList;
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
                Init();

                var p1 = @"<View><Query><Where><And>";

                var p2 = @"<Eq><FieldRef Name='Email' /><Value Type='Text'>" + email + @"</Value></Eq>";

                var p3 = @"<Neq><FieldRef Name='Role'/><Value Type='Choice'>Student</Value></Neq></And></Where></Query></View>";

                var camlQuery = p1 + p2 + p3;

                var users = adapter.GetItems("Users", camlQuery);

                var userList = new List<UserModel>();
                foreach (var user in users)
                {
                    userList.Add(new UserModel()
                    {
                        ID = Convert.ToInt32(user["ID"].ToString()),
                        UserName = user["Username"].ToString(),
                        StudentID = user["StudentID"].ToString(),
                        LastName = user["LastName"].ToString(),
                        FirstName = user["FirstName"].ToString(),
                        Email = user["Email"].ToString(),
                        RoleName = user["Role"].ToString(),
                        TutorialGroupsString = SPUtilities.ParseLookup(user["TutorialGroup"] as FieldLookupValue[]),
                        //TeamsUserID = user["TeamsUserID"]?.ToString(),
                    });
                }

                return userList.Count > 0;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public static List<UserModel> GetAdmins(string tutorialCode)
        {
            try
            {
                Init();

                var p1 = @"<View><Query><Where><And>";


                var split = tutorialCode.Split(';');
                var conditions = new List<string>();
                foreach (var code in split)
                {
                    if (!string.IsNullOrEmpty(code))
                        conditions.Add(@"<Eq><FieldRef Name='TutorialGroup' /><Value Type='LookupMulti'>" + code + @"</Value></Eq>");
                }
                var p2 = MergeCAMLConditions(conditions, "OR");

                var p3 = @"<Neq><FieldRef Name='Role' /><Value Type='Choice'>Student</Value></Neq></And></Where></Query></View>";

                var camlQuery = p1 + p2 + p3;

                var users = adapter.GetItems("Users", camlQuery);

                var userList = new List<UserModel>();
                foreach (var user in users)
                {
                    userList.Add(new UserModel()
                    {
                        ID = Convert.ToInt32(user["ID"].ToString()),
                        UserName = user["Username"].ToString(),
                        StudentID = user["StudentID"].ToString(),
                        LastName = user["LastName"].ToString(),
                        FirstName = user["FirstName"].ToString(),
                        Email = user["Email"].ToString(),
                        RoleName = user["Role"].ToString(),
                        TutorialGroupsString = SPUtilities.ParseLookup(user["TutorialGroup"] as FieldLookupValue[]),
                        //TeamsUserID = user["TeamsUserID"]?.ToString(),
                    });
                }

                return userList;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static List<UserModel> GetAllAdmins()
        {
            try
            {
                Init();

                var p1 = @"<View><Query><Where>";

                var p3 = @"<Neq><FieldRef Name='Role'/><Value Type='Choice'>Student</Value></Neq></Where></Query></View>";

                var camlQuery = p1 + p3;

                var users = adapter.GetItems("Users", camlQuery);

                var userList = new List<UserModel>();
                foreach (var user in users)
                {
                    userList.Add(new UserModel()
                    {
                        ID = Convert.ToInt32(user["ID"].ToString()),
                        UserName = user["Username"].ToString(),
                        StudentID = user["StudentID"].ToString(),
                        LastName = user["LastName"].ToString(),
                        FirstName = user["FirstName"].ToString(),
                        Email = user["Email"].ToString(),
                        RoleName = user["Role"].ToString(),
                        TutorialGroupsString = SPUtilities.ParseLookup(user["TutorialGroup"] as FieldLookupValue[]),
                        //TeamsUserID = user["TeamsUserID"]?.ToString(),
                    });
                }

                return userList;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion

        #region QR
        public static Stream GetImageStream(string siteUrl, string imageUrl)
        {

            try
            {
                return adapter.GetImageStream(siteUrl, imageUrl);
            }
            catch (Exception e)
            {
                return null;
            }

        }

        #endregion

        private static string MergeCAMLConditions(List<string> conditions, string type)
        {
            if (conditions.Count == 0) return "";

            string typeStart = (type == "AND" ? "<And>" : "<Or>");
            string typeEnd = (type == "AND" ? "</And>" : "</Or>");

            // Build hierarchical structure
            while (conditions.Count >= 2)
            {
                List<string> complexConditions = new List<string>();

                for (int i = 0; i < conditions.Count; i += 2)
                {
                    if (conditions.Count == i + 1) // Only one condition left
                        complexConditions.Add(conditions[i]);
                    else // Two condotions - merge
                        complexConditions.Add(typeStart + conditions[i] + conditions[i + 1] + typeEnd);
                }

                conditions = complexConditions;
            }

            return conditions[0];
        }
    }
}