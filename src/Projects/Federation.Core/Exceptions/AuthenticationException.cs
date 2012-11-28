using System;
using System.Runtime.Serialization;

namespace Federation.Core
{
    /// <summary>
    /// Класс ошибок, возникающих если человек неаутентифицирован (т.е мы не знаем кто он).
    /// Критерий: User.IsAuthenticated()
    /// Действие: переход на страницу логина.
    /// Номер ошибки: 1
    /// Контейнер кода ошибки: ViewBag.ExceptionCode
    /// Контейнер текста ошибки: ViewBag.ExceptionText
    /// </summary>
    [Serializable]
    public class AuthenticationException : RedirectException
    {
        //TODO: MESSAGE вынести в CONST HELPER
        public AuthenticationException(Exception inner = null)
            : base("К сожалению, система не знает кто вы такой. Пожалуйста представьтесь", ConstHelper.LoginUrl, inner)
        {
        }
        protected AuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
