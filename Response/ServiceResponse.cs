using System.Net;

namespace Pegasus_MVC.Response
{
    public class ServiceResponse<T>
    {
        public HttpStatusCode StatusCode { get; }
        public T? Data { get; }

        private ServiceResponse(HttpStatusCode statusCode, T? data)
        {
            StatusCode = statusCode;
            Data = data;
        }

        public static ServiceResponse<T> SuccessResponse(HttpStatusCode statusCode, T data) =>
            new ServiceResponse<T>(statusCode, data);

        public static ServiceResponse<T> FailResponse(HttpStatusCode statusCode) =>
            new ServiceResponse<T>(statusCode, default);
    }
}
