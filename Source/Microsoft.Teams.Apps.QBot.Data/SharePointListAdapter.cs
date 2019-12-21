using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Teams.Apps.QBot.Data
{
    public class SharePointListAdapter
    {
        string tenantUrl;
        bool isOnPrem;
        string username;
        SecureString password;

        public SharePointListAdapter()
        {

        }

        public SharePointListAdapter(string tenantUrl, bool isOnPrem, string username, string password)
        {
            if (SPUtilities.AuthenticateSharePointUserAccess(tenantUrl, isOnPrem, username, SPUtilities.GetPassword(password)))
            {
                this.tenantUrl = tenantUrl;
                this.isOnPrem = isOnPrem;
                this.username = username;
                this.password = SPUtilities.GetPassword(password);
            }
            else
            {
                throw new Exception("Unauthorised.");
            }
        }

        public ListItemCollection GetItems(string listName, string query)
        {
            using (ClientContext clientContext = new ClientContext(tenantUrl))
            {
                try
                {
                    if (!isOnPrem)
                    {
                        SharePointOnlineCredentials credentials = new SharePointOnlineCredentials(username, password);
                        clientContext.Credentials = credentials;
                    }

                    List oList = clientContext.Web.Lists.GetByTitle(listName);

                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = query;

                    ListItemCollection collListItem = oList.GetItems(camlQuery);
                    clientContext.Load(collListItem);
                    clientContext.ExecuteQuery();

                    return collListItem;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public int AddOrUpdateItem(string listName, Dictionary<string, string> properties)
        {
            using (ClientContext clientContext = new ClientContext(tenantUrl))
            {
                try
                {
                    if (!isOnPrem)
                    {
                        SharePointOnlineCredentials credentials = new SharePointOnlineCredentials(username, password);
                        clientContext.Credentials = credentials;
                    }

                    List oList = clientContext.Web.Lists.GetByTitle(listName);

                    ListItemCreationInformation itemInfo = new ListItemCreationInformation();

                    ListItem myItem;
                    if (properties["ID"] != "0")
                    {
                        myItem = oList.GetItemById(properties["ID"]);
                    }
                    else
                    {
                        myItem = oList.AddItem(itemInfo);
                    }

                    foreach (var p in properties)
                    {
                        if (p.Key != "ID")
                            myItem[p.Key] = p.Value;
                    }
                    myItem.Update();


                    clientContext.ExecuteQuery();
                    return myItem.Id;
                }
                catch (Exception e)
                {
                    return -1;
                }
            }
        }

        public bool AddOrUpdateItems(string listName, List<Dictionary<string, string>> items)
        {
            using (ClientContext clientContext = new ClientContext(tenantUrl))
            {
                try
                {
                    if (!isOnPrem)
                    {
                        SharePointOnlineCredentials credentials = new SharePointOnlineCredentials(username, password);
                        clientContext.Credentials = credentials;
                    }

                    List oList = clientContext.Web.Lists.GetByTitle(listName);

                    ListItemCreationInformation itemInfo = new ListItemCreationInformation();

                    foreach (var item in items)
                    {
                        ListItem myItem;
                        if (item.ContainsKey("ID") && item["ID"] != "0")
                        {
                            myItem = oList.GetItemById(item["ID"]);
                        }
                        else
                        {
                            myItem = oList.AddItem(itemInfo);
                        }

                        foreach (var p in item)
                        {
                            if (p.Key != "ID")
                                myItem[p.Key] = p.Value;
                        }
                        myItem.Update();
                    }

                    clientContext.ExecuteQuery();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }


        public Stream GetImageStream(string siteUrl, string imageUrl)
        {

            using (ClientContext clientContext = new ClientContext(siteUrl))
            {
                try
                {
                    if (!isOnPrem)
                    {

                        SharePointOnlineCredentials credentials = new SharePointOnlineCredentials(username,  password);
                        clientContext.Credentials = credentials;
                    }

                    SharePoint.Client.Web web = clientContext.Web;
                    clientContext.Load(web, website => website.ServerRelativeUrl);
                    clientContext.ExecuteQuery();
                    Regex regex = new Regex(siteUrl, RegexOptions.IgnoreCase);
                    string strSiteRelavtiveURL = regex.Replace(imageUrl, string.Empty);
                    string strServerRelativeURL = CombineUrl(web.ServerRelativeUrl, strSiteRelavtiveURL);

                    Microsoft.SharePoint.Client.File oFile = web.GetFileByServerRelativeUrl(strServerRelativeURL);
                    clientContext.Load(oFile);
                    ClientResult<Stream> stream = oFile.OpenBinaryStream();
                    clientContext.ExecuteQuery();
                    return ReadFully(stream.Value);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        private string CombineUrl(string path1, string path2)
        {
            return path1.TrimEnd('/') + '/' + path2.TrimStart('/');
        }

        private Stream ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return new MemoryStream(ms.ToArray()); ;
            }
        }

    }
}
