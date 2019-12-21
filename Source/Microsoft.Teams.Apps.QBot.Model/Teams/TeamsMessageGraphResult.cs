using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model.Teams
{
    public class TeamsMessageGraphResult
    {
        [JsonProperty("value")]
        public List<TeamsMessage> TeamsMesages { get; set; }
    }
}
