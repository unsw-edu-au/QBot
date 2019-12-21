using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model
{
    public class QnAResponse
    {
        [JsonProperty("answers")]
        public IList<QnAAnswer> Answers { get; set; }
    }

    public class QnAAnswer
    {
        [JsonProperty("questions")]
        public IList<string> Questions { get; set; }

        [JsonProperty("answer")]
        public string Answer { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("keywords")]
        public IList<object> Keywords { get; set; }

        [JsonProperty("metadata")]
        public IList<Metadata> Metadata { get; set; }
    }

    public class Metadata
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
