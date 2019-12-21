using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.Teams.Apps.QBot.Bot.Services
{
    sealed class BotJwtRefreshWorker : IDisposable
    {
        CancellationTokenSource _Cts = new CancellationTokenSource();

        public BotJwtRefreshWorker()
        {
            var appID = ServiceHelper.AppId;
            var appPassword = ServiceHelper.AppSecret;
            if (!string.IsNullOrEmpty(appID) && !string.IsNullOrEmpty(appPassword))
            {
                var credentials = new MicrosoftAppCredentials(appID, appPassword);
                Task.Factory.StartNew(
                    async () =>
                    {
                        var ct = _Cts.Token;
                        while (!ct.IsCancellationRequested)
                        {
                            try
                            {
                            // GetTokenAsync method internally calls RefreshAndStoreToken, meaning that the token will automatically be cached at this point and you don’t need to do anything else – the bot will always have a valid token.
                            await credentials.GetTokenAsync().ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceError(ex.ToString());
                            }
                            await Task.Delay(TimeSpan.FromMinutes(30), ct).ConfigureAwait(false);
                        }
                    },
                     TaskCreationOptions.LongRunning);
            }
        }

        public void Dispose()
        {
            _Cts.Cancel();
        }
    }
}