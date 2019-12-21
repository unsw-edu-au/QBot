using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model.Graph
{
    public class GraphTeamSPSite
    {
        /*
    
    "@odata.context": "https://graph.microsoft.com/v1.0/$metadata#sites/$entity",
    "createdDateTime": "2017-11-30T04:17:27.09Z",
    "description": "blah",
    "id": "antares3.sharepoint.com,336e4867-c345-4d59-a96c-f7a3173a4723,2ba2c220-7cec-44a2-87db-d66b1de0d945",
    "lastModifiedDateTime": "2018-08-10T03:03:59Z",
    "name": "Bottest",
    "webUrl": "https://antares3.sharepoint.com/sites/Bottest",
    "displayName": "Bot test",
    "root": {},
    "siteCollection": {
        "hostname": "antares3.sharepoint.com"
    }

       */

        [JsonProperty("createdDateTime")]
        public string CreatedDateTime { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("lastModifiedDateTime")]
        public string LastModifiedDateTime { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("webUrl")]
        public string WebUrl { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("root")]
        public SPSite Root { get; set; }

        [JsonProperty("siteCollection")]
        public SPSiteCollection SiteCollection { get; set; }
    }

    public class SPSite
    {

    }

    public class SPSiteCollection
    {
        [JsonProperty("hostname")]
        public string Hostname { get; set; }
    }
}
