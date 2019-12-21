using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Teams.Apps.QBot.Model;

namespace Microsoft.Teams.Apps.QBot.Bot.Services
{
    /// <summary>
    /// QnAMakerService is a wrapper over the QnA Maker REST endpoint
    /// </summary>
    [Serializable]
    public class QnAService
    {
        private string _qnaServiceHostName;
        private string _knowledgeBaseId;
        private string _endpointKey;

        /// <summary>
        /// Initialize a particular endpoint with it's details
        /// </summary>
        public QnAService()
        {
            //_qnaServiceHostName = ConfigurationManager.AppSettings["QnAServiceHostName"];
            //_knowledgeBaseId = ConfigurationManager.AppSettings["QnAknowledgeBaseId"];
            //_endpointKey = ConfigurationManager.AppSettings["QnAEndpointKey"];

            _qnaServiceHostName = ServiceHelper.QnAHost;
            _knowledgeBaseId = ServiceHelper.QnaKnowledgebaseId;
            _endpointKey = ServiceHelper.QnAEndpoint;
        }

        /// <summary>
        /// Call the QnA Maker endpoint and get a response
        /// </summary>
        /// <param name="query">User question</param>
        /// <returns></returns>
        public async Task<QnAAnswer> GetAnswer(string query)
        { 
            var qnaResponse = new QnAResponse();

            using (HttpClient client = new HttpClient())
            {
                string requestUri = String.Format("{0}{1}{2}{3}",
                    _qnaServiceHostName,
                    @"knowledgebases/",
                    _knowledgeBaseId,
                    @"/generateAnswer");

                var httpContent = new StringContent("{\"question\": \""+query+"\"}}", Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("EndpointKey", _endpointKey);

                var msg = await client.PostAsync(requestUri, httpContent);

                if (msg.IsSuccessStatusCode)
                {
                    var jsonDataResponse = await msg.Content.ReadAsStringAsync();
                    qnaResponse = JsonConvert.DeserializeObject<QnAResponse>(jsonDataResponse);
                }
            }

            if (qnaResponse != null && qnaResponse.Answers != null && qnaResponse.Answers.Count > 0)
            {
                return qnaResponse.Answers.FirstOrDefault();
            } else
            {
                return null;
            }
        }
    }
}