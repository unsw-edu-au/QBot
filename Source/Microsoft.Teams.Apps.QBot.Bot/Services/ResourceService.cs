using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Http;

namespace Microsoft.Teams.Apps.QBot.Bot.Services
{
    [Serializable]
    public class ResourceService
    {
        private const string RESOURCE_NAME = "Microsoft.Teams.Apps.QBot.Bot.Resources.strings";
        private string suffix = string.Empty;
        private static ResourceService instance;
        public ResourceService()
        {
        }

        public ResourceService(string suffix)
        {
            this.suffix = suffix;
        }
        public string GetValueFor(string key)
        {
            var filename = RESOURCE_NAME;

            if (false == string.IsNullOrEmpty(this.suffix))
            {
                filename = $"{RESOURCE_NAME}.{this.suffix}";
            }

            var resoureManager = new ResourceManager(filename, Assembly.GetExecutingAssembly());
            var value = resoureManager.GetString(key, CultureInfo.CurrentCulture);

            return value;
        }
    }
}