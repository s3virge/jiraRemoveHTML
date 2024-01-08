using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            string convertedText = text;
            convertedText = convertedText.Replace("\'", "\'\'");

            convertedText = Regex.Replace(convertedText, "<(\\/?)(p|span|div|br)(.*?)>", ""); //<page>, <p>, </p>, <p style=... >, <span>, </span>, <span style=... >, <br>, <br />, <br/>
            convertedText = Regex.Replace(convertedText, "<(\\/?)(b|strong)(.*?)>", "*");
            convertedText = Regex.Replace(convertedText, "<(\\/?)(del)(.*?)>", "-");
            
            if(convertedText.Contains("<a "))
            {
                do
                {
                    convertedText = ConvertHtmlLinkToMarkup(convertedText);
                }
                while (convertedText.Contains("<a ") == true);
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

            textWithLink = textWithLink.Replace(match.Value, $"[{linkName}|{linkHref}]");

            return textWithLink;
        }
    }
}
