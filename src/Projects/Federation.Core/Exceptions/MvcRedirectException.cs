using System;
using System.Runtime.Serialization;

namespace Federation.Core
{
    /// <summary>
    /// Класс ошибок, возникающих при серьезных ошибках, или ошбках требующих перехода на другую страницу
    /// Критерий: глобальная ошибка или ошибка требующая перехода на определенный action/controller
    /// Действие: переходим на страницу по указанному action/controller
    /// Код: указывается код ошибки. Все коды лежат в ErrorsDictionary
    /// Контейнер кода ошибки: ViewBag.ExceptionCode
    /// Контейнер текста ошибки: ViewBag.ExceptionText
    /// </summary>
    [Serializable]
    public class MvcActionRedirectException : ApplicationException
    {
         //TODO: MESSAGE вынести в CONST HELPER
        public MvcActionRedirectException(string message = "Неустановленная ошибка, для выяснения причин свяжитесь с командой проекта support@democracy2.ru", string redirectController = "", string redirectAction = "", object routeValues = null, Exception inner = null)
            : base(message, inner)
        {
            ControllerName = redirectController;
            ActionName = redirectAction;
            RouteValues = routeValues;
        }

        protected MvcActionRedirectException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public virtual object RouteValues { get; private set; }
        public virtual string ControllerName { get; private set; }
        public virtual string ActionName { get; private set; }
    }
}
