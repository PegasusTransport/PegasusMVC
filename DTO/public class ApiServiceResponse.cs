using System.Text.Json.Serialization;

namespace Pegasus_MVC.DTO
{
    public class ApiServiceResponse<T>
    {
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
