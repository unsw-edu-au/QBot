using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.Teams.Apps.QBot.Bot.utility
{
    public static class StringHelper
    {
        public static string ReplaceLinksWithMarkdown(this string text)
        {
            var pattern = @"((http|ftp|https):\/\/)?([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:\/~+#-]*[\w@?^=%&\/~+#-])?";
            var regex = new Regex(pattern);

            var replacedText = regex.Replace(text, new MatchEvaluator((match) =>
            {
                string link = match.ToString();
                return $"[{link}]({link})";
            }));

            return replacedText;
        }
    }
}