using System;
using System.Runtime.Serialization;

namespace Federation.Core
{
    /// <summary>
    /// Класс ошибок, возникающих при вводе некоректных данных для формы.
    /// Критерий: валидационная ошибка формы
    /// Действие: остаемся на этой же странице, с этими же данными. Ошибка показывается в форме
    /// Замечание: так как остаемся на этой же странице с этой же моделью, то
    /// мы должны быть ТОЧНО уверены что данная ошибка локальна и не приведет к ошибкам
    /// например в отображении главного Лайаута, т.е ошибкам как бы выше по уровню проверки,
    /// в противном случае это может привести к зацикливанию
    /// Контейнер ошибки: ViewBag.ValidationException
    /// Контейнер текста ошибки: ViewBag.ValidationExceptionText
    /// </summary>
    [Serializable]
    public class ValidationException : ApplicationException
    {
        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception inner) : base(message, inner) { }
        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
