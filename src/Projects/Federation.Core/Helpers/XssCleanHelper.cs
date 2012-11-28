using System;
using System.Text.RegularExpressions;

namespace Federation.Core
{
    public static class XssCleanHelper
    {
        public static string Clean(string text)
        {
            string cleanVersion = "";
            if (!string.IsNullOrEmpty(text))
            {
                cleanVersion = Regex.Replace(text, @"<script(.|\n)*?>", String.Empty);
                cleanVersion = Regex.Replace(cleanVersion, @"<iframe(.|\n)*?>", String.Empty); 
            }

            return cleanVersion;
        }

        public static string CleanTags(string text)
        {
            string cleanVersion = "";
            if (!string.IsNullOrEmpty(text))
            {
                cleanVersion = text.Replace("</p>", "/r/n");
                cleanVersion = cleanVersion.Replace("<br/>", "/r/n");
                cleanVersion = Regex.Replace(cleanVersion, @"<(.|\n)*?>", String.Empty);
                cleanVersion = Regex.Replace(cleanVersion, @"&lt;(.|\n)*?&gt;", String.Empty);
                cleanVersion = cleanVersion.Replace("/r/n", "<br/>");
                
            }

            return cleanVersion;
        }
    }
}