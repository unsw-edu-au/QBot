using System;

namespace Microsoft.Teams.Apps.QBot.Model
{
    public class QuestionModel
    {
        public int ID { get; set; }

        public string TenantId { get; set; }

        public string GroupId { get; set; }

        public string TeamId { get; set; }

        public string TeamName { get; set; }

        public string ConversationId { get; set; }

        public string MessageId { get; set; }

        public string Topic { get; set; }

        public string QuestionStatus { get; set; }

        public string QuestionText { get; set; }

        public string OriginalPosterEmail { get; set; } // TODO: change to getter only

        public UserCourseRoleMappingModel OriginalPoster { get; set; }

        public DateTime QuestionSubmitted { get; set; }

        public string AnswerCardActivityId { get; set; }

        public string Link { get; set; }

        public string AnswerText { get; set; }

        public string AnswerMessageId { get; set; }

        public string AnswerPosterEmail
        {
            get
            {
                return AnswerPoster == null ? string.Empty : AnswerPoster.Email;
            }
        }

        public UserCourseRoleMappingModel AnswerPoster { get; set; }

        public DateTime? QuestionAnswered { get; set; }

        public int? CourseID { get; set; }

        public QuestionModel()
        {

        }
    }
}
