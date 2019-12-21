using Microsoft.Bot.Connector;
using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Antares.Bot.Channels
{
    public static class MicrosoftTeamsChannelHelper
    {
        public static Activity StripAtMentionText(Activity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            Mention[] m = activity.GetMentions();
            for (int i = 0; i < m.Length; i++)
            {
                if (m[i].Mentioned.Id == activity.Recipient.Id)
                {
                    //Bot is in the @mention list.  
                    //The below example will strip the bot name out of the message, so you can parse it as if it wasn't included.  Note that the Text object will contain the full bot name, if applicable.
                    if (m[i].Text != null)
                        activity.Text = activity.Text.Replace(m[i].Text, "").Trim();
                }
            }

            return activity;
        }

        public static string StripAtMentionText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var cleanText = Regex.Replace(text, "<at.*>(.*?)</at>", "", RegexOptions.IgnoreCase);

            return cleanText;
        }

        public static string StripHtmlTags(string html)
        {
            if (String.IsNullOrEmpty(html)) return "";

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var postHtml = HttpUtility.HtmlDecode(doc.DocumentNode.InnerText);
            var clean = Regex.Replace(postHtml, @"\t|\n|\r", "");

            return postHtml.Trim();
        }

        public static string StripMentionAndHtml(string text)
        {
            var firstpass = StripAtMentionText(text);
            var secondpass = StripHtmlTags(firstpass);

            return secondpass;
        }

        public static Activity ConvertActivityTextToLower(Activity activity)
        {
            //Convert input command in lower case for 1To1 and Channel users
            if (activity.Text != null)
            {
                activity.Text = activity.Text.ToLower();
            }

            return activity;
        }


    }
}
