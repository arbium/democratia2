namespace Federation.Web.Services
{
    public class ApiResponse
    {
        public int Code { get; set; }
        public string ExceptionMessage { get; set; }
        public ApiResponseCode? ApiExceptionCode { get; set; }

        public string Message
        {
            get
            {
                string apiErrorMessage = string.Empty;
                if (ApiExceptionCode.HasValue)
                    apiErrorMessage = ApiExceptionCode.Value.PrepareForUser();

                if (string.IsNullOrEmpty(apiErrorMessage))
                    return "Произошла ошибка (код: " + Code + (ApiExceptionCode.HasValue ? "." + (int)ApiExceptionCode.Value : "") + ")";

                return apiErrorMessage;
            }
        }

        public static ApiResponse Empty = new ApiResponse()
        {
            Code = (int)PlatformErrorCode.Empty,
            ExceptionMessage = "Empty response!"
        };

        public static ApiResponse UnableToConnect = new ApiResponse()
        {
            Code = (int)PlatformErrorCode.UnableToConnect,
            ExceptionMessage = "Unable to connect to server!"
        };

        public static ApiResponse BadResponse = new ApiResponse()
        {
            Code = (int)PlatformErrorCode.BadResponse,
            ExceptionMessage = "Bad response format!"
        };

        public static ApiResponse UnknownError = new ApiResponse()
        {
            Code = (int)PlatformErrorCode.UnknownError,
            ExceptionMessage = "Unknown error!"
        };

        public static ApiResponse ApiError(ApiResponseCode code)
        {
            return new ApiResponse()
            {
                Code = (int)PlatformErrorCode.ApiError,
                ExceptionMessage = "Api error!",
                ApiExceptionCode = code
            };
        }
    }
}