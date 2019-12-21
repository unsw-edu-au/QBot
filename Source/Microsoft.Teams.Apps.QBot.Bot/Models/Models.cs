using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Microsoft.Bot.Connector;

namespace Microsoft.Teams.Apps.QBot.Bot.Models
{
    public class BotFrameworkCardValue<T>
    {
        [JsonProperty("type")]
        public object Type { get; set; } = "task/fetch";

        [JsonProperty("data")]
        public T Data { get; set; }
    }

    public class AdaptiveCardValue<T>
    {
        [JsonProperty("msteams")]
        public object Type { get; set; } = JsonConvert.DeserializeObject("{\"type\": \"task/fetch\" }");

        [JsonProperty("data")]
        public T Data { get; set; }
    }

    public class TaskModuleActionData<T>
    {
        [JsonProperty("data")]
        public BotFrameworkCardValue<T> Data { get; set; }
    }

    public class TaskInfo
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("card")]
        public Attachment Card { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("height")]
        public object Height { get; set; }

        [JsonProperty("width")]
        public object Width { get; set; }

        [JsonProperty("fallbackUrl")]
        public string FallbackUrl { get; set; }

        [JsonProperty("completionBotId")]
        public string CompletionBotId { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class TaskEnvelope
    {
        [JsonProperty("task")]
        public Task Task { get; set; }
    }

    public class Task
    {
        [JsonProperty("value")]
        public TaskInfo TaskInfo { get; set; }

        [JsonProperty("type")]
        public TaskType Type { get; set; }
    }

    public enum TaskType
    {
        /// <summary>
        /// Teams will display the value of value in a popup message box.
        /// </summary>
        [EnumMember(Value = "message")]
        Message,

        /// <summary>
        /// Allows you to "chain" sequences of Adaptive cards together in a wizard/multi-step experience.
        /// </summary>
        [EnumMember(Value = "continue")]
        Continue
    }


    public class StreamVideoProperties
    {
        public string VideoTitle { get; set; }
        public string VideoID { get; set; }
        public int VideoStartTime { get; set; }
       
    }
}