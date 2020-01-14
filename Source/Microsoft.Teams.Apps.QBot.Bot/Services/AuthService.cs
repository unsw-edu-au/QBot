using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.Teams.Apps.QBot.Bot.Services
{
    public class AuthService
    {
        private AuthenticationContext context = null;
        private AuthenticationResult result = null;

        public AuthService(string authority)
        {
            context = new AuthenticationContext(authority);
        }

        public async Task<AuthenticationResult> AuthenticateSilently(string resource)
        {
            // Try get Application permissions (AppId + Secret)
            try
            {
                var appCreds = new ClientCredential(ServiceHelper.ClientId, ServiceHelper.AppSecret);
                result = await context.AcquireTokenAsync(resource, appCreds);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Could not authenticate using application permissions for AppID:  " + ServiceHelper.ClientId);
                Trace.WriteLine(e.ToString());
                result = null;
            }

            if (result == null)
            {
                // Fallback to delegated permissions
                try
                {
                    var uc = new UserPasswordCredential(ServiceHelper.ServiceAccountName, ServiceHelper.ServiceAccountPassword);
                    result = await context.AcquireTokenAsync(resource, ServiceHelper.ClientId, uc);
                }
                catch (Exception e)
                {
                    Trace.WriteLine("Could not authenticate using delegated permissions for AppID:  " + ServiceHelper.ClientId + ", Username: " + ServiceHelper.ServiceAccountName);
                    Trace.WriteLine(e.ToString());
                    result = null;
                }
            }

            return result;
        }
    }
}