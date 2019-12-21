using System.Web;
using System.Web.Mvc;

namespace Microsoft.Teams.Apps.QBot.Bot
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
