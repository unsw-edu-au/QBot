using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Model.QnA
{
    public class QnAPayload
    {
        [JsonProperty("add")]
        public AddPayLoad Add { get; set; }

        [JsonProperty("delete")]
        public DeletePayload Delete { get; set; }

        [JsonProperty("update")]
        public UpdatePayload Update { get; set; }
    }

    public class AddPayLoad
    {
        [JsonProperty("qnaList")]
        public List<QnAItemToAdd> QnaList { get; set; }

        [JsonProperty("urls")]
        public List<string> Urls { get; set; }

        [JsonProperty("files")]
        public List<File> Files { get; set; }
    }

    public class QnAItemToAdd
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("answer")]
        public string Answer { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("questions")]
        public List<string> Questions { get; set; }

        [JsonProperty("metadata")]
        public List<MetadataItem> Metadata { get; set; }

        public QnAItemToAdd()
        {
            Id = 0;
            Answer = string.Empty;
            Source = @"Editorial";
            Questions = new List<string>();
            Metadata = new List<MetadataItem>();
        }
    }

    public class File
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("fileUri")]
        public string FileUri { get; set; }
    }

    public class DeletePayload
    {
        [JsonProperty("ids")]
        public List<int> Ids { get; set; }

        [JsonProperty("sources")]
        public List<string> Sources { get; set; } = new List<string> { "Editorial" };
    }

    public class UpdatePayload
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("qnaList")]
        public List<QnaItemToUpdate> QnaList { get; set; }

        [JsonProperty("urls")]
        public List<string> Urls { get; set; }
    }

    public class QnaItemToUpdate
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("answer")]
        public string Answer { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("questions")]
        public Questions Questions { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        public QnaItemToUpdate()
        {
            Id = 0;
            Answer = string.Empty;
            Source = @"Editorial";
            Questions = new Questions();
            Metadata = new Metadata();
        }
    }

    public class Questions
    {
        [JsonProperty("add")]
        public List<string> Add { get; set; }

        [JsonProperty("delete")]
        public List<string> Delete { get; set; }

        public Questions()
        {
            Add = new List<string>();
            Delete = new List<string>();
        }
    }

    public class Metadata
    {
        [JsonProperty("add")]
        public List<MetadataItem> Add { get; set; }

        [JsonProperty("delete")]
        public List<MetadataItem> Delete { get; set; }

        public Metadata()
        {
            Add = new List<MetadataItem>();
            Delete = new List<MetadataItem>();
        }
    }

    public class MetadataItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
