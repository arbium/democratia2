using System;
using System.Collections.Generic;

namespace Federation.Core
{
    public class ErrorsDictionary
    {
        private Dictionary<int, string> _errors = new Dictionary<int, string>()
                                                             {
                                                                 {-1, ""},//ничего не показываем - просто редирект
                                                                 {0, "Возникла системная ошибка. Если ошибка будет повторяться, пожалуйста, сообщите об этом администратору."},//TODO: ConstHelper.AdminEmail
                                                                 {1, "К сожалению, система не знает кто вы такой. Пожалуйста представьтесь"},
                                                                 {2, "Уже набрано установленное количество модераторов. "},
                                                                 {3, "Указанный ключ не верен или устарел!"}
                                                             };

        public void LoadDictionary(string url)//TODO: Вынести в сервис
        {
            
        }

        public string this[int code]
        {
            get
            {
                if(_errors.ContainsKey(code))
                {
                    return _errors[code];
                }
                return String.Empty;
            }
        }

        private static ErrorsDictionary _globalErrors = new ErrorsDictionary();

        public static ErrorsDictionary Errors
        {
            get { return _globalErrors; }
        }
    }
}
