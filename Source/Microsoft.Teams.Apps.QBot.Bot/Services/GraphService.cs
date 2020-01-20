using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Teams.Apps.QBot.Model.Graph;
using Microsoft.Teams.Apps.QBot.Model.Teams;

using Newtonsoft.Json;

namespace Microsoft.Teams.Apps.QBot.Bot.Services
{
    public class GraphService
    {

        public async Task<byte[]> GetProfilePhoto(string accessToken, string userPrincipalName)
        {
            string endpoint = ServiceHelper.GraphRootUri + @"/v1.0";
            string queryParameter = @"users/" + userPrincipalName + @"/photo/$value";
            HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint + queryParameter, accessToken);
            if (response != null && response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                return bytes;
            }
            else
            {
                Trace.TraceError("Error calling Graph API for GetProfilePhoto");
                Trace.TraceError(response.ToString());
            }

            return null;
        }

        public async Task<byte[]> GetGroupPhoto(string accessToken, string groupId)
        {
            // At time of writing, getting Group photo is not supported via Application permissions
            // https://docs.microsoft.com/en-us/graph/known-issues#groups
            string endpoint = ServiceHelper.GraphRootUri + @"/v1.0";
            string queryParameter = @"/groups/" + groupId + @"/photo/$value";
            HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint + queryParameter, accessToken);
            if (response != null && response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                return bytes;
            }
            else
            {
                Trace.TraceError("Error calling Graph API for GetGroupPhoto");
                Trace.TraceError(response.ToString());
            }

            return null;
        }

        public async Task<TeamsMessage> GetMessage(string accessToken, string teamId, string channelId, string messageId)
        {
            try
            {
                string endpoint = ServiceHelper.GraphRootUri + @"/beta";
                string queryParameter = "/teams/" + teamId + "/channels/" + channelId + "/messages/" + messageId;
                HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint + queryParameter, accessToken);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var teamsMessage = JsonConvert.DeserializeObject<TeamsMessage>(jsonString);
                    return teamsMessage;
                }
                else
                {
                    Trace.TraceError("Error calling Graph API for GetMessage");
                    Trace.TraceError(response.ToString());
                }

                return null;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        public async Task<List<TeamsMessage>> GetRepliesToMessage(string accessToken, string teamId, string channelId, string messageId)
        {
            try
            {
                string endpoint = ServiceHelper.GraphRootUri + @"/beta";
                string queryParameter = "/teams/" + teamId + "/channels/" + channelId + "/messages/" + messageId + "/replies";
                HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint + queryParameter, accessToken);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var teamsMessageGraphResult = JsonConvert.DeserializeObject<TeamsMessageGraphResult>(jsonString);
                    var teamsMessages = teamsMessageGraphResult.TeamsMesages;
                    return teamsMessages;
                }
                else
                {
                    Trace.TraceError("Error calling Graph API for GetRepliesToMessage");
                    Trace.TraceError(response.ToString());
                }

                return null;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        public async Task<List<TeamChannel>> GetChannels(string accessToken, string groupId)
        {
            try
            {
                string endpoint = ServiceHelper.GraphRootUri + @"/v1.0";
                string queryParameter = "/teams/" + groupId + "/channels";
                HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint + queryParameter, accessToken);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var teamsChannelResult = JsonConvert.DeserializeObject<TeamChannelResult>(jsonString);
                    return teamsChannelResult.TeamChannels;
                }
                else
                {
                    Trace.TraceError("Error calling Graph API for GetChannels");
                    Trace.TraceError(response.ToString());
                }

                return null;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        public async Task<GraphTeamSPSite> GetTeamGroupSPUrl(string accessToken, string groupId)
        {
            try
            {
                string endpoint = ServiceHelper.GraphRootUri + @"/v1.0";
                string queryParameter = "/groups/" + groupId + "/sites/root";
                HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint + queryParameter, accessToken);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var teamsSPSiteResult = JsonConvert.DeserializeObject<GraphTeamSPSite>(jsonString);

                    string endpoint2 = ServiceHelper.GraphRootUri + @"/v1.0";
                    string queryParameter2 = "/groups/" + groupId;
                    HttpResponseMessage response2 = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint2 + queryParameter2, accessToken);
                    if (response2 != null && response2.IsSuccessStatusCode)
                    {
                        var jsonString2 = await response2.Content.ReadAsStringAsync();
                        dynamic teamsObject = JsonConvert.DeserializeObject(jsonString2);
                        string teamsDisplayName = teamsObject.displayName;

                        teamsSPSiteResult.DisplayName = teamsDisplayName;
                    }
                    else
                    {
                        Trace.TraceError("Error calling Graph API for GetTeamGroupSPUrl - groups");
                        Trace.TraceError(response.ToString());
                    }

                    return teamsSPSiteResult;
                }
                else
                {
                    Trace.TraceError("Error calling Graph API for GetTeamGroupSPUrl - sites/root");
                    Trace.TraceError(response.ToString());
                }

                return null;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Queries Graph API and returns a list of O365 Group IDs for which the given user is a member of
        /// </summary>
        /// <param name="accessToken">Access Token</param>
        /// <param name="upn">Username</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<List<string>> GetGroupMemberOf(string accessToken, string upn)
        {
            try
            {
                string endpoint = ServiceHelper.GraphRootUri + @"/v1.0";
                string queryParameter = "/users/" + upn + "/memberOf?$select=id";
                HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint + queryParameter, accessToken);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var groupMemberOfResult = JsonConvert.DeserializeObject<GraphMemberOfResult>(jsonString);

                    return groupMemberOfResult.Items.Select(x => x.Id).ToList();
                }
                else
                {
                    Trace.TraceError("Error calling Graph API for GetGroupMemberOf");
                    Trace.TraceError(response.ToString());
                }

                return null;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        public async Task<GraphTeamsOwnedResult> GetOwnedObjects(string accessToken, string upn)
        {
            try
            {
                string endpoint = ServiceHelper.GraphRootUri + @"/v1.0";
                string queryParameter = "/users/" + upn + "/ownedObjects";
                HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint + queryParameter, accessToken);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<GraphTeamsOwnedResult>(jsonString);
                }
                else
                {
                    Trace.TraceError("Error calling Graph API for GetOwnedObjects");
                    Trace.TraceError(response.ToString());
                }

                return null;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }

        public async Task<GraphUsers> GetMembers(string accessToken, string groupId)
        {
            try
            {
                string endpoint = ServiceHelper.GraphRootUri + @"/v1.0";
                string queryParameter = "/groups/" + groupId + "/members";
                HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint + queryParameter, accessToken);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<GraphUsers>(jsonString);
                    return result;
                }
                else
                {
                    Trace.TraceError("Error calling Graph API for GetMembers");
                    Trace.TraceError(response.ToString());
                }

                return null;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }
    }
}