using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model.Graph
{

    public class GraphUsers
    {
        [JsonProperty("value")]
        public List<GraphUser> Items { get; set; }

    }

    public class GraphUser
    {
        public string displayName { get; set; }
        public string id { get; set; }
        public string givenName { get; set; }
        public string surname { get; set; }
        public string userPrincipalName { get; set; }
        public string jobTitle { get; set; }
        public string mail { get; set; }
    }

}
