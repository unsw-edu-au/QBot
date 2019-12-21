using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Teams.Apps.QBot.Model
{
    public class CourseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public System.Guid GroupId { get; set; }
        public string PredictiveQnAServiceHost { get; set; }
        public string PredictiveQnAKnowledgeBaseId { get; set; }
        public string PredictiveQnAEndpointKey { get; set; }
        public string PredictiveQnAHttpEndpoint { get; set; }
        public string PredictiveQnAHttpKey { get; set; }
        public string PredictiveQnAKnowledgeBaseName { get; set; }
        public string PredictiveQnAConfidenceThreshold { get; set; }
        public string DeployedURL { get; set; }
    }
}
