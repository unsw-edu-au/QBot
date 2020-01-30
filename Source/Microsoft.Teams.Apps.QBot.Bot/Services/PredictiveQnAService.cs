using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Teams.Apps.QBot.Model;
using Microsoft.Teams.Apps.QBot.Model.QnA;

using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Bot.Services
{
    /// <summary>
    /// QnAMakerService is a wrapper over the QnA Maker REST endpoint
    /// </summary>
    [Serializable]
    public class PredictiveQnAService
    {
        private string _predictiveQnAServiceHostName;
        private string _predictiveKnowledgeBaseId;
        private string _predictiveEndpointKey;
        private string _predictiveQnAKnowledgeBaseName;
        private string _predictiveQnAHttpEndpoint;
        private string _predictiveQnAHttpKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredictiveQnAService"/> class.
        /// Initialize a particular endpoint with it's details
        /// </summary>
        public PredictiveQnAService(int courseID)
        {

            var course = SQLService.GetCourse(courseID);
            _predictiveQnAServiceHostName = course.PredictiveQnAServiceHost;
            _predictiveKnowledgeBaseId = course.PredictiveQnAKnowledgeBaseId;
            _predictiveEndpointKey = course.PredictiveQnAEndpointKey;
            _predictiveQnAKnowledgeBaseName = course.PredictiveQnAKnowledgeBaseName;
            _predictiveQnAHttpEndpoint = course.PredictiveQnAHttpEndpoint;
            _predictiveQnAHttpKey = course.PredictiveQnAHttpKey;

            if (!string.IsNullOrEmpty(_predictiveQnAServiceHostName) && !_predictiveQnAServiceHostName.EndsWith("/"))
            {
                _predictiveQnAServiceHostName += "/";
            }

            if (!string.IsNullOrEmpty(_predictiveQnAHttpEndpoint) && !_predictiveQnAHttpEndpoint.EndsWith("/"))
            {
                _predictiveQnAHttpEndpoint += "/";
            }
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
                string requestUri = string.Format(
                    "{0}{1}{2}{3}",
                    _predictiveQnAServiceHostName,
                    @"knowledgebases/",
                    _predictiveKnowledgeBaseId,
                    @"/generateAnswer");

                var httpContent = new StringContent("{\"question\": \"" + query + "\"}}", Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("EndpointKey", _predictiveEndpointKey);

                var msg = await client.PostAsync(requestUri, httpContent);

                var jsonDataResponse = await msg.Content.ReadAsStringAsync();
                if (msg.IsSuccessStatusCode)
                {
                    qnaResponse = JsonConvert.DeserializeObject<QnAResponse>(jsonDataResponse);
                }
                else
                {
                    Trace.TraceError("Error getting QnA response for query: " + query);
                }
            }

            if (qnaResponse != null && qnaResponse.Answers != null && qnaResponse.Answers.Count > 0)
            {
                return qnaResponse.Answers.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> PatchQnA(QnAPayload payload)
        {
            using (HttpClient client = new HttpClient())
            {
                string requestUri = string.Format(
                    "{0}{1}{2}",
                    _predictiveQnAHttpEndpoint,
                    @"knowledgebases/",
                    _predictiveKnowledgeBaseId);

                var httpContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _predictiveQnAHttpKey);

                var msg = await client.PatchAsync(new Uri(requestUri), httpContent);

                if (msg.IsSuccessStatusCode)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> AddQnAPair(string question, string answer)
        {
            var payload = new QnAPayload()
            {
                Add = new AddPayLoad()
                {
                    QnaList = new List<QnAItemToAdd>(),
                },
            };

            var qnaItemToAdd = new QnAItemToAdd();
            qnaItemToAdd.Answer = answer;
            qnaItemToAdd.Questions.Add(question);

            payload.Add.QnaList.Add(qnaItemToAdd);

            var addResult = await PatchQnA(payload);

            return addResult;
        }

        public async Task<bool> UpdateQnAPair(List<string> questionsToAdd = null, List<string> questionsToDelete = null, string answerToUpdate = null, int id = 0)
        {
            var payload = new QnAPayload()
            {
                Update = new UpdatePayload()
                {
                    Name = _predictiveQnAKnowledgeBaseName,
                    QnaList = new List<QnaItemToUpdate>(),
                },
            };

            var qnaItemToUpdate = new QnaItemToUpdate
            {
                Id = id,
            };

            if (questionsToAdd != null && questionsToAdd.Count > 0)
            {
                qnaItemToUpdate.Questions.Add.AddRange(questionsToAdd);
            }

            if (questionsToDelete != null && questionsToDelete.Count > 0)
            {
                qnaItemToUpdate.Questions.Delete.AddRange(questionsToDelete);
            }

            if (!string.IsNullOrEmpty(answerToUpdate))
            {
                qnaItemToUpdate.Answer = answerToUpdate;
            }

            payload.Update.QnaList.Add(qnaItemToUpdate);

            var addResult = await PatchQnA(payload);

            return addResult;
        }
    }
}