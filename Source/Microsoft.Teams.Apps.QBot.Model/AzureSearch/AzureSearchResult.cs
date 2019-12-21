using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Teams.Apps.QBot.Model.AzureSearch
{
    public class AzureSearchResult
    {
        [JsonProperty("value")]
        public List<AzureSearchResultItem> Results { get; set; }
    }

    public class AzureSearchResultItem
    {
        [JsonProperty("@search.score")]
        public double Score { get; set; }
        [JsonProperty("Number")]
        public string Number { get; set; }
        [JsonProperty("Start_time_in_milliseconds")]
        public int StartTime { get; set; }
        [JsonProperty("End_time_in_milliseconds")]
        public int EndTime { get; set; }
        [JsonProperty("Text")]
        public string Text { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("keyphrases")]
        public List<string> KeyPhrases { get; set; }

        public string StreamUrl { get; set; }
    }
}
