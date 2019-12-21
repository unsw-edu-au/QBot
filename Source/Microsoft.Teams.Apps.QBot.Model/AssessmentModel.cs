using System;

namespace Microsoft.Teams.Apps.QBot.Model
{
    public class AssessmentModel
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string SubType { get; set; }

        public string Title { get; set; }

        public string Topic { get; set; }

        public DateTime Date { get; set; }

        public DateTime DateEnd { get; set; }

        public string Weighting { get; set; }
    }
}
