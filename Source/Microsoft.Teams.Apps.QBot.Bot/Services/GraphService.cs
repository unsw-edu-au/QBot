using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Teams.Apps.QBot.Model;
using Microsoft.Teams.Apps.QBot.Model.Graph;
using Microsoft.Teams.Apps.QBot.Model.Teams;

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
            return null;
        }

       
        public async Task<byte[]> GetGroupPhoto(string accessToken, string groupId)
        {
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
                return null;
            }
            catch (Exception e)
            {
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
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<TeamChannel>> GetChannels(string accessToken, string groupId)
        {
            try
            {
                string endpoint = ServiceHelper.GraphRootUri + @"/beta";
                string queryParameter = "/groups/" + groupId + "/channels";
                HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint + queryParameter, accessToken);
                if (response != null && response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var teamsChannelResult = JsonConvert.DeserializeObject<TeamChannelResult>(jsonString);
                    return teamsChannelResult.TeamChannels;
                }
                return null;
            }
            catch (Exception e)
            {
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
                    return teamsSPSiteResult;
                }
                return null;
            }
            catch (Exception e)
            {
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

                return null;
            }
            catch (Exception e)
            {
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
                    //var deserializedResultDictionary = deserializedResult.Items.ToDictionary(x => x.id, x => x.displayName);
                    //return deserializedResultDictionary;
                }
                return null;
            }
            catch (Exception e)
            {
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
                    var test =  JsonConvert.DeserializeObject<GraphUsers>(jsonString);
                    return test;
                    //var deserializedResultDictionary = deserializedResult.Items.ToDictionary(x => x.id, x => x.displayName);
                    //return deserializedResultDictionary;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }

        }




        /*
        public async Task<IEnumerable<ResultsItem>> GetMyTeams(string accessToken)
        {
            string resourcePropId = @"ID :";

            string endpoint = ServiceHelper.GraphRootUri + "me/joinedTeams";
            string idPropertyName = "id";
            string displayPropertyName = "displayName";

            List<ResultsItem> items = new List<ResultsItem>();
            HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint, accessToken);
            if (response != null && response.IsSuccessStatusCode)
            {
                items = await ServiceHelper.GetResultsItem(response, idPropertyName, displayPropertyName, resourcePropId);

            }
            return items;
        }

        
        public async Task<Tuple<HttpResponseMessage, string>> CreateNewTeamAndGroup(string accessToken, string groupName, string userPrincipalName)
        {
            Group group = new Group()
            {
                id = Guid.NewGuid().ToString(),
                displayName = groupName,
                description = groupName,
                mailNickname = groupName.Replace(" ", ""),
            };

            // create group
            string endpoint = ServiceHelper.GraphRootUri + "groups";
            if (group != null)
            {
                group.groupTypes = new string[] { "Unified" };
                group.mailEnabled = true;
                group.securityEnabled = false;
                group.visibility = "Private";
            }

            HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Post, endpoint, accessToken, group);
            if (!response.IsSuccessStatusCode)
            {
                return new Tuple<HttpResponseMessage, string>(response, string.Empty);
            }

            string responseBody = await response.Content.ReadAsStringAsync(); ;
            string groupId = responseBody.Deserialize<Group>().id;

            // add me as member
            //string me = await GetMyId(accessToken);

            // add user as member
            string me = await GetUserId(userPrincipalName, accessToken);
            string payload = $"{{ '@odata.id': '{ServiceHelper.GraphRootUri}users/{me}' }}";
            HttpResponseMessage responseRef = await ServiceHelper.SendRequest(HttpMethod.Post,
                ServiceHelper.GraphRootUri + $"groups/{groupId}/members/$ref",
                accessToken, payload);


            // add user as OWNER
            //HttpResponseMessage responseRef = await ServiceHelper.SendRequest(HttpMethod.Post,
            //    ServiceHelper.GraphRootUri + $"groups/{groupId}/owners/$ref",
            //    accessToken, payload);


            if (!responseRef.IsSuccessStatusCode)
            {
                return new Tuple<HttpResponseMessage, string>(responseRef, groupId);
            }

            // create team
            var responseFinal = await AddTeamToGroup(groupId, accessToken);
            return new Tuple<HttpResponseMessage, string>(responseFinal, groupId);
        }

        public async Task<string> GetSiteUrlFromGroup(string groupId, String accessToken)
        {
            string endpoint = ServiceHelper.GraphRootUri + @"groups/" + groupId + @"/sites/root";
            String webUrl = "";
            HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint, accessToken);
            if (response != null && response.IsSuccessStatusCode)
            {
                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                webUrl = json.GetValue("webUrl").ToString();
            }
            return webUrl?.Trim();
        }

        public async Task<string> GetUserId(string userPrincipalName, String accessToken)
        {
            string endpoint = ServiceHelper.GraphRootUri + @"users/";
            string queryParameter = userPrincipalName;
            String userId = "";
            HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint + queryParameter, accessToken);
            if (response != null && response.IsSuccessStatusCode)
            {
                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                userId = json.GetValue("id").ToString();
            }
            return userId?.Trim();
        }

        public async Task<string> GetMyId(String accessToken)
        {
            string endpoint = "https://graph.microsoft.com/v1.0/me";
            string queryParameter = "?$select=id";
            String userId = "";
            HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Get, endpoint + queryParameter, accessToken);
            if (response != null && response.IsSuccessStatusCode)
            {
                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                userId = json.GetValue("id").ToString();
            }
            return userId?.Trim();
        }

        public async Task<HttpResponseMessage> AddTeamToGroup(string groupId, string accessToken)
        {
            string endpoint = ServiceHelper.GraphRootUri + "groups/" + groupId + "/team";
            Team team = new TeamsPoC.Services.Models.Team();
            team.guestSettings = new TeamsPoC.Services.Models.TeamGuestSettings() { allowCreateUpdateChannels = false, allowDeleteChannels = false };

            HttpResponseMessage response = await ServiceHelper.SendRequest(HttpMethod.Put, endpoint, accessToken, team);
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.ReasonPhrase);
            return response;
        }
            */
    }
}