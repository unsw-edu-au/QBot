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
            return await AuthenticateSilently(resource, ServiceHelper.ClientPermissionType, false);
        }

        public async Task<AuthenticationResult> AuthenticateSilently(string resource, string clientPermissionType, bool fallback = true)
        {
            AuthenticationResult result = null;
            if (clientPermissionType == "Application")
            {
                result = await AuthenticateUsingApplicationPermissions(resource);
            }
            else
            {
                result = await AuthenticateUsingDelegatePermissions(resource);
            }

            if (result == null && fallback)
            {
                // Fallback to authenticating using the OTHER permission type if the first try failed
                if (clientPermissionType == "Application")
                {
                    result = await AuthenticateUsingDelegatePermissions(resource);
                }
                else
                {
                    result = await AuthenticateUsingApplicationPermissions(resource);
                }
            }

            return result;
        }

        private async Task<AuthenticationResult> AuthenticateUsingApplicationPermissions(string resource)
        {
            AuthenticationResult result = null;

            // Try get Application permissions (AppId + Secret)
            try
            {
                var appCreds = new ClientCredential(ServiceHelper.ClientId, ServiceHelper.ClientSecret);
                result = await context.AcquireTokenAsync(resource, appCreds);

                Trace.WriteLine("Authenticated OK using application permissions for AppID:  " + ServiceHelper.ClientId);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Could not authenticate using application permissions for AppID:  " + ServiceHelper.ClientId);
                Trace.WriteLine(e.ToString());
                result = null;
            }

            return result;
        }

        private async Task<AuthenticationResult> AuthenticateUsingDelegatePermissions(string resource)
        {
            AuthenticationResult result = null;

            // Use Delegated permissions
            try
            {
                var uc = new UserPasswordCredential(ServiceHelper.ServiceAccountName, ServiceHelper.ServiceAccountPassword);
                result = await context.AcquireTokenAsync(resource, ServiceHelper.ClientId, uc);

                Trace.WriteLine("Authenticate OK using delegated permissions for AppID:  " + ServiceHelper.ClientId + ", Username: " + ServiceHelper.ServiceAccountName);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Could not authenticate using delegated permissions for AppID:  " + ServiceHelper.ClientId + ", Username: " + ServiceHelper.ServiceAccountName);
                Trace.WriteLine(e.ToString());
                result = null;
            }

            return result;
        }

    }
}