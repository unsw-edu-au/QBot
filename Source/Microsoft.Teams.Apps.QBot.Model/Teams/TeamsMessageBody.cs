using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model.Teams
{
    public class TeamsMessageBody
    {

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
