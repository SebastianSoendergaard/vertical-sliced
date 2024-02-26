using System.Net;

namespace WebApi
{
    public class ApiError
    {
        public ApiError(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}
