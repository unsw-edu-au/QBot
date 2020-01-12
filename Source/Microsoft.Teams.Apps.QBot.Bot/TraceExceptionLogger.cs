using System.Diagnostics;
using System.Web.Http.ExceptionHandling;

namespace Microsoft.Teams.Apps.QBot.Bot
{
    public class TraceExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            Trace.TraceError(context.ExceptionContext.Exception.ToString());
        }
    }

}