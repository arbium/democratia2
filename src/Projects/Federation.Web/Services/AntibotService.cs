using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Xml;

namespace Federation.Web.Services
{
    public static class AntibotService
    {
        private static ConcurrentDictionary<Guid, KeyData> _safetyKeyLibrary = new ConcurrentDictionary<Guid, KeyData>();
        private static List<string> _questions = new List<string>();

        static AntibotService()
        {
            _questions.Add("слон");
            _questions.Add("космос");
            _questions.Add("кошка");
            _questions.Add("ворона");
            _questions.Add("дом");
            _questions.Add("ребенок");
            _questions.Add("велосипед");
            _questions.Add("очки");
            _questions.Add("луна");
            _questions.Add("волк");
            _questions.Add("заяц");
            _questions.Add("медведь");
            _questions.Add("ананас");
            _questions.Add("арбуз");
            _questions.Add("дракон");
            _questions.Add("яблоко");
            _questions.Add("груша");
            _questions.Add("часы");
            _questions.Add("небо");
            _questions.Add("машина");
            _questions.Add("корабль");
            _questions.Add("дерево");
            _questions.Add("самолет");
            _questions.Add("лебядь");
            _questions.Add("жираф");
            _questions.Add("танк");
            _questions.Add("солдат");
            _questions.Add("гитара");
            _questions.Add("озеро");
            _questions.Add("телефон");
            _questions.Add("вишня");
            _questions.Add("сыр");
            _questions.Add("пианино");
            _questions.Add("навальный");
            _questions.Add("путин");
            _questions.Add("доллар");
            _questions.Add("ложка");
            _questions.Add("собака");
            _questions.Add("ельцин");
            _questions.Add("горбачев");
            _questions.Add("пряник");
            _questions.Add("печенье");
        }

        public struct KeyData
        {
            public DateTime GivenDate;
            public string Question;
            public Guid Key;
            public bool ValidateAnswer;
        }

        public static KeyData GetSefetyKey(bool validateAnswer)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            int questionIndex = random.Next(_questions.Count);
            Guid key = Guid.NewGuid();
            KeyData keyData = new KeyData
            {
                Key = key,
                GivenDate = DateTime.Now,
                Question = _questions[questionIndex],
                ValidateAnswer = validateAnswer
            };
            while (!_safetyKeyLibrary.TryAdd(key, keyData))
            {
                Thread.Sleep(10);
            }
            return keyData;
        }

        public static bool IsKeyActual(Guid key, string answer)
        {
            bool result = false;

            if (_safetyKeyLibrary.ContainsKey(key))
            {
                KeyData value = _safetyKeyLibrary[key];
                if (!value.ValidateAnswer || (!string.IsNullOrWhiteSpace(answer) && answer.ToLower().Trim() == value.Question))
                {
                    result = true;
                    while (!_safetyKeyLibrary.TryRemove(key, out value))
                    {
                        Thread.Sleep(10);
                    }
                }
            }

            return result;
        }

        public static string GetCaptureImageUrl(string key)
        {
            string result = string.Empty;
            var searchResult = BingSearchService.SearchImage(key);
            if (searchResult != null && searchResult.ChildNodes.Count > 0)
            {
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(searchResult.NameTable);
                string imageNamespaceUri =
                "http://schemas.microsoft.com/LiveSearch/2008/04/XML/multimedia";
                nsmgr.AddNamespace("mms", imageNamespaceUri);
                XmlNode image = searchResult.DocumentElement.SelectSingleNode("./mms:Image", nsmgr);
                XmlNodeList results = image.SelectNodes("./mms:Results/mms:ImageResult", nsmgr);
                Random rand = new Random((int)DateTime.Now.Ticks);
                int select = rand.Next(9);
                result = results.Item(select).SelectSingleNode("./mms:Thumbnail/mms:Url", nsmgr).InnerText;
            }

            return result;
        }

        public static string MakeQuestionTip(string key)
        {
            string result = "";
            int lettersCount = key.Length;
            int hiddenLettersCount = Math.Max((lettersCount * 5) / 10, 2);
            HashSet<int> hiddenIndexes = new HashSet<int>();

            while (hiddenLettersCount > 0)
            {
                Random random = new Random((int)DateTime.Now.Ticks);
                int index = random.Next(lettersCount - 1);
                if (!hiddenIndexes.Contains(index))
                {
                    hiddenIndexes.Add(index);
                    hiddenLettersCount--;
                }
            }

            var cahrs = key.ToCharArray();
            for (int i = 0; i < lettersCount; i++)
            {
                if (hiddenIndexes.Contains(i))
                    result += " _ ";
                else
                    result += " " + cahrs[i] + " ";
            }

            return result;
        }
    }
}