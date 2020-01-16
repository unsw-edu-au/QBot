using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using EncryptionHelper;

using Microsoft.Teams.Apps.QBot.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Teams.Apps.QBot.Bot.Services
{
    public static class ServiceHelper
    {
        #region Config Properties

        public static string BaseUrl
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.BASE_URL_KEY];
            }
        }

        public static string ClientId
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.AAD_CLIENTID_KEY];
            }
        }

        public static string ClientSecret
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.AAD_CLIENTSECRET_KEY];
            }
        }

        public static string ClientPermissionType
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.AAD_PERMISSIONTYPE_KEY];
            }
        }

        public static string Authority
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.AAD_AUTHORITY_KEY];
            }
        }

        public static string GraphResource
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.AAD_GRAPHRESOURCE_KEY];
            }
        }

        public static string ServiceAccountName
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.SERVICE_ACCOUNT_KEY];
            }
        }

        public static SecureString ServiceAccountPassword
        {
            get
            {
                CryptoTransform cryptoTransform = new CryptoTransform(Helper.PASSPHRASE, Helper.INITVECTOR);
                var outputString = cryptoTransform.Decrypt(ConfigurationManager.AppSettings[Constants.SERVICE_ACCOUNT_PASSWORD_KEY]);

                return GetPassword(outputString);
            }
        }

        public static string GraphRootUri
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.AAD_GRAPHROOT_KEY];
            }
        }

        public static string AppId
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.BOT_APPID_KEY];
            }
        }

        public static string AppSecret
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.BOT_APPPASSWORD_KEY];
            }
        }

        public static string QnAEndpoint
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.QNA_ENDPOINT_KEY];
            }
        }

        public static string QnAHost
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.QNA_HOST_KEY];
            }
        }

        public static string QnaKnowledgebaseId
        {
            get
            {
                return ConfigurationManager.AppSettings[Constants.QNA_ID_KEY];
            }
        }

        #endregion

        public static async Task<HttpResponseMessage> SendRequest(HttpMethod method, String endPoint, string accessToken, dynamic content = null)
        {
            HttpResponseMessage response = null;
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(method, endPoint))
                {
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    if (content != null)
                    {
                        string c;
                        if (content is string)
                            c = content;
                        else
                            c = JsonConvert.SerializeObject(content);
                        request.Content = new StringContent(c, Encoding.UTF8, "application/json");
                    }

                    response = await client.SendAsync(request);
                }
            }
            return response;

        }

        /// <summary>
        /// Helper function to prepare the ResultsItem list from request response.
        /// </summary>
        /// <param name="response">Request response</param>
        /// <param name="idPropertyName">Property name of the item Id</param>
        /// <param name="displayPropertyName">Property name of the item display name</param>
        /// <returns></returns>
        public static async Task<List<ResultsItem>> GetResultsItem(
            HttpResponseMessage response, string idPropertyName, string displayPropertyName, string resourcePropId)
        {
            List<ResultsItem> items = new List<ResultsItem>();

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
            foreach (JProperty content in json.Children<JProperty>())
            {
                if (content.Name.Equals("value"))
                {
                    var res = content.Value.AsJEnumerable().GetEnumerator();
                    res.MoveNext();

                    while (res.Current != null)
                    {
                        string display = "";
                        string id = "";

                        foreach (JProperty prop in res.Current.Children<JProperty>())
                        {
                            if (prop.Name.Equals(idPropertyName))
                            {
                                id = prop.Value.ToString();
                            }

                            if (prop.Name.Equals(displayPropertyName))
                            {
                                display = prop.Value.ToString();
                            }
                        }

                        items.Add(new ResultsItem
                        {
                            Display = display,
                            Id = id,
                            Properties = new Dictionary<string, object>
                                            {
                                                { resourcePropId, id }
                                            }
                        });

                        res.MoveNext();
                    }
                }
            }

            return items;
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
    }
}