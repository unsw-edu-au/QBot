using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model.Teams
{
    public class TeamsMessage
    {
        [JsonProperty("replyToId")]
        public string ReplyToId { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }

        [JsonProperty("messageType")]
        public string MessageType { get; set; }

        [JsonProperty("createdDateTime")]
        public string CreatedDateTime { get; set; }

        [JsonProperty("lastModifiedDateTime")]
        public string LastModifiedDateTime { get; set; }

        [JsonProperty("deleted")]
        public string Deleted { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("importance")]
        public string Importance { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("from")]
        public TeamsMember From { get; set; }

        [JsonProperty("body")]
        public TeamsMessageBody Body { get; set; }

        [JsonProperty("attachments")]
        public List<TeamsAttachment> Attachments { get; set; }

        [JsonProperty("mentions")]
        public List<TeamsMention> Mentions { get; set; }

        [JsonProperty("reactions")]
        public List<TeamsReaction> Reactions { get; set; }

        /*
        "replyToId": null,
        "etag": "1532578374803",
        "messageType": "message",
        "createdDateTime": "2018-07-26T04:12:09.161Z",
        "lastModifiedDateTime": null,
        "deleted": false,
        "subject": "",
        "summary": null,
        "importance": "normal",
        "locale": "en-us",
        "id": "1532578329161",
        "from": {
            "application": null,
            "device": null,
            "user": {
                "id": "943a82b8-fdb9-4dbf-aa6c-31e37bb9aed6",
                "displayName": "Wajeed Hanif Shaikh (ZEN3 INFOSOLUTIONS(INDIA)LIMIT)",
                "identityProvider": "Aad"
            }
        },
        "body": {
            "contentType": "text",
            "content": "Hello Graph API"
        },
        "attachments": [],
        "mentions": [],
        "reactions": [
            {
                "type": "like",
                "createdDateTime": "2018-07-26T04:12:54.802Z",
                "content": null,
                "user": {
                    "application": null,
                    "device": null,
                    "user": {
                        "id": "554b25b6-8307-4280-8143-781f7dcb7d30",
                        "displayName": null,
                        "identityProvider": "Aad"
                    }
                }
            }
        ]
        */
    }
}
