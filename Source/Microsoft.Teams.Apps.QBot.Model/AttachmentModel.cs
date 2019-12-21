namespace Microsoft.Teams.Apps.QBot.Model
{
    public class AttachmentModel
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        // Question or Answer?
        public string AttachmentType { get; set; }

        public string FileType { get; set; }

        public string AttachmentUrl { get; set; }
    }
}
