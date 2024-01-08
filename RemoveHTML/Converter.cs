using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RemoveHTML
{
    public class Converter
    {
        /// <summary>
        /// Convert text to jira markup https://jira.atlassian.com/secure/WikiRendererHelpAction.jspa?section=all
        /// </summary>
        public static string ToJiraMarkup(string text)
        {
            string convertedText = "";
            convertedText = Regex.Replace(text, "<b>", "*");
            convertedText = Regex.Replace(convertedText, "</b>", "*");
            convertedText = Regex.Replace(convertedText, "<del>", "-");
            convertedText = Regex.Replace(convertedText, "</del>", "-");
            convertedText = Regex.Replace(convertedText, "<(\\/?)(p|span|div|br|del)(.*?)>", ""); //<page>, <p>, </p>, <p style=... >, <span>, </span>, <span style=... >, <br>, <br />, <br/>
            
            while (convertedText.Contains("<a "))
            {
                convertedText = ConvertHtmlLinkToMarkup(convertedText);
            }

            return convertedText;
        }

        private static string ConvertHtmlLinkToMarkup(string textWithLink)
        {
            //have to make this <a href="https://jira.intetics.com/browse/CWAECG-6429">https://jira.intetics.com/browse/CWAECG-6429</a>
            //<a class="external-link" href="https://twiki.atwss.com/bin/view/Newsroom/EmailNotifications" rel="nofollow">https://twiki.atwss.com/bin/view/Newsroom/EmailNotifications</a>
            //like this [Atlassian|http://atlassian.com]

            //find the link in the string            
            var match = Regex.Match(textWithLink, "<(\\/?)a(.*?)\\/a>");
            string linkHref = Regex.Match(match.Value, "(?<=href=\")(.+?)(?=\")").ToString();
            string linkName = Regex.Match(match.Value, "(?<=>)(.+?)(?=<\\/a>)").ToString();

            textWithLink = Regex.Replace(textWithLink, match.Value, $"[{linkName}|{linkHref}]");

            return textWithLink;
        }
    }
}
