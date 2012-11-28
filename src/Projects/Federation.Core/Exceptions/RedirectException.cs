using System;
using System.Runtime.Serialization;

namespace Federation.Core
{
    /// <summary>
    /// Класс ошибок, возникающих при серьезных ошибках, или ошбках требующих перехода на другую страницу
    /// Критерий: глобальная ошибка или ошибка требующая перехода на определенный url
    /// Действие: переходим на страницу по указанному url
    /// Код: указывается код ошибки. Все коды лежат в ErrorsDictionary
    /// Контейнер кода ошибки: ViewBag.ExceptionCode
    /// Контейнер текста ошибки: ViewBag.ExceptionText
    /// </summary>
    [Serializable]
    public class RedirectException : ApplicationException
    {
        //TODO: MESSAGE вынести в CONST HELPER
        public RedirectException(string message = "Неустановленная ошибка, для выяснения причин свяжитесь с командой проекта support@democracy2.ru", string redirectUrl = "", Exception inner = null)
            : base(message, inner)
        {
            RedirectUrl = redirectUrl;
        }

        protected RedirectException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public string RedirectUrl { get; private set; }
    }
}
