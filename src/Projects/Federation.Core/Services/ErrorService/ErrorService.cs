
namespace Federation.Core
{
    public static class ErrorService
    {
        private static IErrorService _service = new ErrorServiceImpl();

        public static string Add(string errorText)
        {
            return _service.Add(errorText);
        }

        public static string Get(string key)
        {
            return _service.Get(key);
        }
    }
}
