using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model.Teams
{
    public class TeamsMention
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("mentionText")]
        public string MentionText { get; set; }

    }
}
