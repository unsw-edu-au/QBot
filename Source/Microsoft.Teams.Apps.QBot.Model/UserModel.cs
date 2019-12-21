using System.Collections.Generic;

namespace Microsoft.Teams.Apps.QBot.Model
{
    public class UserCourseRoleMappingModel
    {
        public int ID { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string CourseName { get; set; }
        public int CourseId { get; set; }
        public string TutorialGroupID { get; set; }

        public PersonalConversationContactData PersonalConversationContactData { get; set; }

        public string RoleName { get; set; }

        public RoleModel Role { get; set; }

        //public int TutorialGroupID { get; set; }
        public string TutorialGroupsString { get; set; }

        public List<TutorialGroupModel> TutorialGroups { get; set; }

        public bool IsAdmin { get; set; }
        public string StudentID { get; set; }


        public string FullName
        {
            get
            {
                return FirstName + @" " + LastName;
            }
        }
    }

    public class UserModel
    {
        public int ID { get; set; }
        public string StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class UserAccessModel
    {
        public bool IsGlobalAdmin { get; set; }

        public bool IsLecturer { get; set; }

        public bool IsDemonstrator { get; set; }
    }


    public class PersonalConversationContactData
    {
        public string ToId { get; set; }

        public string ToName { get; set; }

        public string FromId { get; set; }

        public string FromName { get; set; }

        public string ServiceUrl { get; set; }

        public string ChannelId { get; set; }

        public string ConversationId { get; set; }
    }
}
