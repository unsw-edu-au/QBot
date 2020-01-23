using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Teams.Apps.QBot.Model.QnA
{
    public class QnAStatusResponse
    {
        public HttpResponseHeaders ResponseHeaders { get; set; }
        public string ResponseBody { get; set; }

        public QnAStatusResponse(HttpResponseHeaders responseHeaders, string responseBody)
        {
            ResponseHeaders = responseHeaders;
            ResponseBody = responseBody;
        }
    }
}
