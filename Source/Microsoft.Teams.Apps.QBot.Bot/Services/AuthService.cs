using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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
            // Try get token silently
            try
            {
                result = await context.AcquireTokenSilentAsync(resource, ServiceHelper.AppId);
            }
            catch (Exception e)
            {
                result = null;
            }

            if (result == null)
            {
                // Try with credentials
                try
                {
                    var uc = new UserPasswordCredential(ServiceHelper.ServiceAccountName, ServiceHelper.ServiceAccountPassword);
                    //var cc = new ClientCredential(ServiceHelper.AppId, ServiceHelper.AppSecret);
                    result = await context.AcquireTokenAsync(resource, ServiceHelper.ClientId, uc);
                }
                catch (Exception e)
                {
                    result = null;
                }
            }

            return result;
        }
    }
}