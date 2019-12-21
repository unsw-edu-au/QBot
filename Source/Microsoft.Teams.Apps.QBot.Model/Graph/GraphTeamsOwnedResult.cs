using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model.Graph
{

    public class GraphTeamsOwnedResult {
        [JsonProperty("value")]
        public List<GraphTeamsOwnedResultItem> Items { get; set; }

    }

    public class GraphTeamsOwnedResultItem
    {
        public string displayName { get; set; }
        public string id { get; set; }

    }
}
