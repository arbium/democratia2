using System;
using System.Text.RegularExpressions;

namespace Federation.Core
{
    public static class TextHelper
    {
        public static string CleanXssTags(string text)
        {
            var cleanVersion = "";
            if (!string.IsNullOrEmpty(text))
            {
                cleanVersion = Regex.Replace(text, @"<script(.|\n)*?>", String.Empty);
                cleanVersion = Regex.Replace(cleanVersion, @"<iframe(.|\n)*?>", String.Empty);
            }

            return cleanVersion;
        }

        public static string EncodeLinks(string text)
        {
            var result = text;
            if (!string.IsNullOrEmpty(text))
            {
                var matches = Regex.Matches(text, @"<a.*?>(\n|.)*?</a.*?>");
                foreach (var match in matches)
                {
                    result = result.Replace("<","%3C");
                    result = result.Replace(">","%3E");
                }
            }

            return result;
        }

        public static string DecodeLinks(string text)
        {
            var result = text;
            if (!string.IsNullOrEmpty(text))
            {
                var matches = Regex.Matches(text, @"%3Ca.*?%3E(\n|.)*?%3C/a.*?%3E");
                foreach (var match in matches)
                {
                    result = result.Replace("%3C", "<");
                    result = result.Replace("%3E", ">");
                }
            }

            return result;
        }

        public static string ReplaceUrlWithLink(string text)
        {
            var result = text;
            if (!string.IsNullOrEmpty(text))
            {
			    //причина падения сервера   var matches = Regex.Matches(text, @"(?<!<(.*)?)((?xi)\b((?:[a-z][\w-]+:(?:/{1,3})|www\d{0,3}[.]|[a-z0-9.\-]+[.][a-z]{2,4}/)(?:[^\s()<>]+|\(([^\s()<>]+|(\([^\s()<>]+\)))*\))+(?:\(([^\s()<>]+|(\([^\s()<>]+\)))*\)|[^\s`!()\[\]{};:'"".,<>?«»“”‘’])))(?!</.*>)");
                var matches = Regex.Matches(text, @"(?<!<(.*)?)(?<=\s|\n|^)(((https?|ftp)://)|(www.))[^\s\n$*'\\]*\.[^\s\n$*'\\]*(?!</.*>)");
                foreach (var match in matches)
                {
                    var href = Regex.Match(match.ToString(), @".+:/{1,3}").ToString();
                    var host = Regex.Match(match.ToString(), @"(?<=(.+:/{1,3}www\.)|(.+:/{1,3})|(www\.))([a-za-яё0-9%]+(\.[a-za-яё0-9%]+)+)").ToString();
                     
                    if (string.IsNullOrWhiteSpace(host))
                        host = "внешняя ссылка";

                    if (string.IsNullOrWhiteSpace(href))
                       result = result.Replace(match.ToString(), "<a target='_blank' class='external-link' href='http://" + match + "'>" + host + "</a>");
                     else
                        result = result.Replace(match.ToString(), "<a target='_blank' class='external-link' href='" + match + "'>" + host + "</a>");
                 }
            }

            return result;
        }

        public static string CleanTags(string text, bool showHtmlLinks = false, bool showEmbededLinks = false)
        {
            var cleanVersion = "";
            if (!string.IsNullOrEmpty(text))
            {
                cleanVersion = Regex.Replace(text, @"<!--.*>(.|\n)*<!.*-->", String.Empty);
                cleanVersion = Regex.Replace(cleanVersion, @"<xml.*>(\n|.)*?</xml.*>", String.Empty);
                cleanVersion = Regex.Replace(cleanVersion, @"<style.*?>(\n|.)*?</style.*?>", String.Empty); //Очистка от всяческих стилей
                cleanVersion = Regex.Replace(cleanVersion, @"<table.*?>(\n|.)*?</table.*?>", String.Empty); //Очистка от всяческих таблиц
                cleanVersion = Regex.Replace(cleanVersion, @"(\r|\n|\t)", String.Empty); //Убираем переносы, которые использовались html реадктором
                cleanVersion = Regex.Replace(cleanVersion, @"<br\/?>", "\r\n"); //Заменяем переносы строк на /r/n
                
                if (showHtmlLinks)
                    cleanVersion = Regex.Replace(cleanVersion, @"<(?!\/?(p|h.|div|li|ul|a)(?=>|\s.*>))\/?.*?>", String.Empty); // Убиваем все кроме заголовка
                else
                    cleanVersion = Regex.Replace(cleanVersion, @"<(?!\/?(p|h.|div|li|ul)(?=>|\s.*>))\/?.*?>", String.Empty); // Убиваем все кроме заголовка

                cleanVersion = Regex.Replace(cleanVersion, @"<(\/(p|h.|div|li|ul)(?=>|\s.*>))\/?.*?>", "\r\n");
                cleanVersion = Regex.Replace(cleanVersion, @"<((p|h.|div|ul)(?=>|\s.*>))\/?.*?>", "\r\n");

                if (showHtmlLinks)
                    cleanVersion = Regex.Replace(cleanVersion, @"<(?!\/?(a)(?=>|\s.*>))\/?.*?>", String.Empty); //Удаляем весь html
                else
                    cleanVersion = Regex.Replace(cleanVersion, @"<(?!\/?(?=>|\s.*>))\/?.*?>", String.Empty); // Удаляем весь html

                //cleanVersion = Regex.Replace(cleanVersion, @"(\r?\n\s{0,}){3,}", "\r\n\r\n"); //Чистим лищние абзацы, оставляем абзацы
                cleanVersion = Regex.Replace(cleanVersion, @"(\r?\n\s{0,}){2,}", "\r\n"); //Чистим лищние абзацы, убираем абзацы совсем
                cleanVersion = Regex.Replace(cleanVersion, @"(^(\r?\n))|((\r?\n)$)", String.Empty); //Убирем отступ в первой и последней строке
                cleanVersion = Regex.Replace(cleanVersion, "\r?\n", "<br/>"); //Заменяем /r/n на реальный перенос строк <br/>

                if (showEmbededLinks)
                    cleanVersion = ReplaceUrlWithLink(cleanVersion);
            }

            return cleanVersion;
        }

        public static string CleanGroupLabel(string text)
        {
            var cleanVersion = "";
            if (!string.IsNullOrEmpty(text))
            {
                cleanVersion = Regex.Replace(text, @"\s{1,}", "_");
                cleanVersion = Regex.Replace(cleanVersion, @"[^A-Za-z0-9А-Яа-яёЁ_]", String.Empty);
            }

            return cleanVersion;
        }
    }
}