using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Teams.Apps.QBot.Data
{
    public static class SPUtilities
    {
        public static bool AuthenticateSharePointUserAccess(string tenantUrl, bool isOnPrem, string username, SecureString password)
        {
            if (string.IsNullOrEmpty(tenantUrl))
                return false;
            if (string.IsNullOrEmpty(username))
                return false;
            if (password == null)
                return false;


            // connect to the tenant
            using (ClientContext context = new ClientContext(tenantUrl))
            {
                // only relevant if SharePoint Online
                if (!isOnPrem)
                {
                    SharePointOnlineCredentials credentials = new SharePointOnlineCredentials(username, password);
                    context.Credentials = credentials;
                }
            }

            return true;
        }

        public static SecureString GetPassword(string password)
        {
            SecureString sStrPwd = new SecureString();

            try
            {
                if (!string.IsNullOrEmpty(password))
                {
                    var secure = new SecureString();
                    foreach (char c in password)
                    {
                        secure.AppendChar(c);
                    }

                    return secure;
                }
                else
                {
                    throw new Exception("Password cannot be empty");
                }
            }
            catch (Exception e)
            {
                sStrPwd = null;
            }

            return sStrPwd;
        }

        public static string ParseLookup(FieldLookupValue[] lookupValues)
        {
            var r = string.Empty;
            if (lookupValues != null)
            {
                foreach (var lookup in lookupValues)
                {
                    r += lookup.LookupValue + @";";
                }
            }
            return r.Trim();
        }
    }
}
