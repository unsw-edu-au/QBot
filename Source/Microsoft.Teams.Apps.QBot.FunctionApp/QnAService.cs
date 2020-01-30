using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Teams.Apps.QBot.FunctionApp
{
    public class QnAService
    {
        private string _predictiveKnowledgeBaseId;
        private string _predictiveQnAHttpEndpoint;
        private string _predictiveQnAHttpKey;

        public QnAService(string kbId, string endpoint, string key)
        {
            _predictiveKnowledgeBaseId = kbId;
            _predictiveQnAHttpEndpoint = endpoint;
            _predictiveQnAHttpKey = key;

            if (!string.IsNullOrEmpty(_predictiveQnAHttpEndpoint) && !_predictiveQnAHttpEndpoint.EndsWith("/"))
            {
                _predictiveQnAHttpEndpoint += "/";
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
    }
}
