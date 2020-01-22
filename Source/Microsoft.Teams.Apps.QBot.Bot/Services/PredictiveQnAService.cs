using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
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

        public async Task<bool> PublishQnA()
        {
            using (HttpClient client = new HttpClient())
            {
                string requestUri = string.Format(
                    "{0}{1}{2}",
                    _predictiveQnAHttpEndpoint,
                    @"knowledgebases/",
                    _predictiveKnowledgeBaseId);

                var httpContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _predictiveQnAHttpKey);

                var msg = await client.PostAsync(new Uri(requestUri), httpContent);

                if (msg.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    Trace.WriteLine("QnA KB Published succesfully for " + requestUri);
                    return true;
                }
                else
                {
                    var jsonDataResponse = await msg.Content.ReadAsStringAsync();
                    Trace.TraceError("There was an error publishing the QnA KB. " + msg.StatusCode);
                    Trace.TraceError(jsonDataResponse);
                }
            }

            return false;
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
                    // Iteratively gets the state of the operation updating the
                    // knowledge base. Once the operation state is something other
                    // than "Running" or "NotStarted", the loop ends.
                    var location = msg.Headers.GetValues("Location").First();

                    var done = false;

                    while (!done)
                    {
                        var operationStatusResponse = await GetStatus(location);

                        var fields = JsonConvert.DeserializeObject<Dictionary<string, string>>(operationStatusResponse.ResponseBody);

                        // Gets and checks the state of the operation.
                        String state = fields["operationState"];
                        if (state.CompareTo("Running") == 0 || state.CompareTo("NotStarted") == 0)
                        {
                            // QnA Maker is still updating the knowledge base. The thread is
                            // paused for a number of seconds equal to the Retry-After
                            // header value, and then the loop continues.
                            var wait = operationStatusResponse.ResponseHeaders.GetValues("Retry-After").First();
                            Console.WriteLine("Waiting " + wait + " seconds...");
                            Thread.Sleep(Int32.Parse(wait) * 1000);
                        }
                        else
                        {
                            // QnA Maker has completed updating the knowledge base.
                            done = true;
                        }
                    }

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

            if (addResult)
            {
                var publishResult = await PublishQnA();
                return publishResult;
            }
            else
            {
                return addResult;
            }
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
            if (addResult)
            {
                var publishResult = await PublishQnA();

                return publishResult;
            }
            else
            {
                return addResult;
            }
        }

        private async Task<QnAStatusResponse> GetStatus(string operation)
        {
            string uri = _predictiveQnAHttpEndpoint + operation;
            Console.WriteLine("Calling " + uri + ".");

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(uri);
                request.Headers.Add("Ocp-Apim-Subscription-Key", _predictiveQnAHttpKey);

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                return new QnAStatusResponse(response.Headers, responseBody);
            }
        }
    }
}