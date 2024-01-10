using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

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
                    convertedText = ConvertLink(convertedText);
                }
                while (convertedText.Contains("<a ") == true);
            }

            if (convertedText.Contains("<ul"))
            {
                do
                {
                    convertedText = ConvertList(convertedText);
                }
                while (convertedText.Contains("<ul") == true);
            }

            return convertedText;
        }

        private static string ConvertLink(string textWithLink)
        {
            //have to make this <a href="https://jira.intetics.com/browse/CWAECG-6429">https://jira.intetics.com/browse/CWAECG-6429</a>
            //<a class="external-link" href="https://twiki.atwss.com/bin/view/Newsroom/EmailNotifications" rel="nofollow">https://twiki.atwss.com/bin/view/Newsroom/EmailNotifications</a>
            //like this [Atlassian|http://atlassian.com]

            //find the link in the string            
            var match = Regex.Match(textWithLink, "<a((.|\r\n)*?)\\/a>");
            if (match.Success)
            {
                string linkHref = Regex.Match(match.Value, "(?<=href=\")(.+?)(?=\")").ToString();
                string linkName = Regex.Match(match.Value, "(?<=>)(.+?)(?=<\\/a>)").ToString();

                textWithLink = textWithLink.Replace(match.Value, $"[{linkName}|{linkHref}]");
            }
            return textWithLink;
        }

        private static string ConvertList(string textWithList)
        {
            textWithList = Regex.Replace(textWithList, "<ul(.*?)>", "<ul>");
            textWithList = Regex.Replace(textWithList, "<li(.*?)>", "<li>");
            //выбрать первый список
            int first = textWithList.IndexOf("<ul>") + "<ul>".Length;
            int last = textWithList.IndexOf("</ul>");
            string list = textWithList.Substring(first, last - first + "</ul>".Length);

            string nestedList = "";
            string convertedNestedList = "";
            if (list.Contains("<ul>"))
            {
                first = list.IndexOf("<ul>");
                last = list.IndexOf("</ul>");
                nestedList = list.Substring(first, last - first + "</ul>".Length);
                convertedNestedList = ConvertNestedList(nestedList);
            }

            Debug.WriteLine(list);
            Debug.WriteLine("---------------------------------");
            Debug.WriteLine(nestedList);
            Debug.WriteLine("---------------------------------");
            Debug.WriteLine(convertedNestedList);

            if (nestedList.Length > 0)
            {
                textWithList = textWithList.Replace(nestedList, convertedNestedList);
            }

            Debug.WriteLine("---------------------------------");
            Debug.WriteLine(textWithList);

            textWithList = Regex.Replace(textWithList, "<(\\/?)ul(.*?)>", "");
            textWithList = Regex.Replace(textWithList, "<li>", "*");
            textWithList = Regex.Replace(textWithList, "</li>", "");

            string convertedList = textWithList;
            return convertedList;
        }

        static string ReplaceFirstOccurrence(string original, string search, string replacement)
        {
            int index = original.IndexOf(search);
            if (index != -1)
            {
                return original.Substring(0, index) + replacement + original.Substring(index + search.Length);
            }
            return original;
        }
        
        static string ReplaceLastOccurrence(string original, string search, string replacement)
        {
            int index = original.LastIndexOf(search);
            if (index != -1)
            {
                return original.Substring(0, index) + replacement + original.Substring(index + search.Length);
            }
            return original;
        }
        
        static string ConvertNestedList(string original)
        {
            int firstItemIndex = original.IndexOf("<ul>");
            string lastString = "</ul>";
            int lastItemIndex = original.IndexOf(lastString);
            
            string listItem = original.Substring(firstItemIndex, lastItemIndex - firstItemIndex + lastString.Length);            
            string convertedlistItem = listItem.Replace("<li>", "**");
            convertedlistItem = ReplaceFirstOccurrence(convertedlistItem, "<ul>", "");
            convertedlistItem = ReplaceLastOccurrence(convertedlistItem, "</ul>", "");
            original = original.Replace(listItem, convertedlistItem);

            return original;
        }
    }
}
