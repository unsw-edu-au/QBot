using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model.Teams
{
    public class TeamsMember
    {
        [JsonProperty("application")]
        public TeamsUser Application { get; set; }

        [JsonProperty("device")]
        public string Device { get; set; }

        [JsonProperty("user")]
        public TeamsUser User { get; set; }
    }
}
