using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model.Graph
{
    public class GraphMemberOfResult
    {
        [JsonProperty("value")]
        public List<GraphMemberOfResultItem> Items { get; set; }
    }

    public class GraphMemberOfResultItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
