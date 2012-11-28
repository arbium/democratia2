using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Federation.Core.Services
{
    public static class TransliterationService
    {
        public static string Translite(string word)
        {
            var dictionary = new Dictionary<string, string>();

            dictionary.Add("А", "A");
            dictionary.Add("Б", "B");
            dictionary.Add("В", "V");
            dictionary.Add("Г", "G");
            dictionary.Add("Д", "D");
            dictionary.Add("Е", "E");
            dictionary.Add("Ё", "E");
            dictionary.Add("Ж", "Zh");
            dictionary.Add("З", "Z");
            dictionary.Add("И", "I");
            dictionary.Add("Й", "Y");
            dictionary.Add("К", "K");
            dictionary.Add("Л", "L");
            dictionary.Add("М", "M");
            dictionary.Add("Н", "N");
            dictionary.Add("О", "O");
            dictionary.Add("П", "P");
            dictionary.Add("Р", "R");
            dictionary.Add("С", "S");
            dictionary.Add("Т", "T");
            dictionary.Add("У", "U");
            dictionary.Add("Ф", "F");
            dictionary.Add("Х", "Kh");
            dictionary.Add("Ц", "Ts");
            dictionary.Add("Ч", "Ch");
            dictionary.Add("Ш", "Sh");
            dictionary.Add("Щ", "Shch");
            dictionary.Add("Ъ", "");
            dictionary.Add("Ы", "Y");
            dictionary.Add("Ь", "");
            dictionary.Add("Э", "E");
            dictionary.Add("Ю", "Yu");
            dictionary.Add("Я", "Ya");
            dictionary.Add("а", "a");
            dictionary.Add("б", "b");
            dictionary.Add("в", "v");
            dictionary.Add("г", "g");
            dictionary.Add("д", "d");
            dictionary.Add("е", "e");
            dictionary.Add("ё", "e");
            dictionary.Add("ж", "zh");
            dictionary.Add("з", "z");
            dictionary.Add("и", "i");
            dictionary.Add("й", "y");
            dictionary.Add("к", "k");
            dictionary.Add("л", "l");
            dictionary.Add("м", "m");
            dictionary.Add("н", "n");
            dictionary.Add("о", "o");
            dictionary.Add("п", "p");
            dictionary.Add("р", "r");
            dictionary.Add("с", "s");
            dictionary.Add("т", "t");
            dictionary.Add("у", "u");
            dictionary.Add("ф", "f");
            dictionary.Add("х", "kh");
            dictionary.Add("ц", "tc");
            dictionary.Add("ч", "ch");
            dictionary.Add("ш", "sh");
            dictionary.Add("щ", "shch");
            dictionary.Add("ъ", "");
            dictionary.Add("ы", "y");
            dictionary.Add("ь", "");
            dictionary.Add("э", "e");
            dictionary.Add("ю", "yu");
            dictionary.Add("я", "ya");

            StringBuilder newWord = new StringBuilder();
            foreach (var ch in word)
            {
                var key = ch.ToString();
                if (dictionary.ContainsKey(key))
                    newWord.Append(dictionary[key]);
                else
                    newWord.Append(key);
            }

            return newWord.ToString();
        }
    }
}
