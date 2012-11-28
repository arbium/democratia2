using System;
using System.Runtime.Serialization;

namespace Federation.Core
{
    /// <summary>
    /// Класс ошибок, возникающих при выполнении некоторой логики приложения.
    /// Критерий: локальная ошибка бизнес логики
    /// Действие: остаемся на этой же странице, с этима же данными
    /// Замечание: так как остаемся на этой же странице с этой же моделью, то
    /// мы должны быть ТОЧНО уверены что данная ошибка локальна и не приведет к ошибкам
    /// например в отображении главного Лайаута, т.е ошибкам как бы выше по уровню проверки,
    /// в противном случае это может привести к зацикливанию
    /// Контейнер ошибки: ViewBag.Exception
    /// Контейнер текста ошибки: ViewBag.ExceptionText
    /// </summary>
    [Serializable]
    public class BusinessLogicException : ApplicationException
    {
        public BusinessLogicException() { }
        public BusinessLogicException(string message) : base(message) { }
        public BusinessLogicException(string message, Exception inner) : base(message, inner) { }
        protected BusinessLogicException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}