using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model.Teams
{
    public class TeamsReaction
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("createdDateTime")]
        public string CreatedDateTime { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("user")]
        public TeamsUser User { get; set; }
    }
}
