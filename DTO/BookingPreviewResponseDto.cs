using System.Text.Json.Serialization;

namespace Pegasus_MVC.DTO
{
    public class BookingPreviewResponseDto
    {
        [JsonPropertyName("distanceKm")]
        public decimal DistanceKm { get; set; }

        [JsonPropertyName("durationMinutes")]
        public decimal DurationMinutes { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}
