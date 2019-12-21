using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model.Graph
{
    public class TeamChannelResult
    {
        [JsonProperty("value")]
        public List<TeamChannel> TeamChannels { get; set; }
    }

    public class TeamChannel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
