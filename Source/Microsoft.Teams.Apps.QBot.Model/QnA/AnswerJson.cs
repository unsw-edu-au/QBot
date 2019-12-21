using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model
{
    public class AnswerJson
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("lookup")]
        public string Lookup { get; set; }

        [JsonProperty("entity")]
        public List<string> Entity { get; set; }

        [JsonProperty("answer")]
        public string Answer { get; set; }
    }
}
