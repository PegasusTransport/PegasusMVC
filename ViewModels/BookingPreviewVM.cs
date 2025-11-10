namespace Pegasus_MVC.ViewModels
{
    public class BookingPreviewVM
    {
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        // Pickup info (mandatory)
        public DateTime PickUpDateTime { get; set; }
        public string PickUpAddress { get; set; } = null!;
        // First stop (optional)
        public string? FirstStopAddress { get; set; }
        // Second stop (optional)
        public string? SecondStopAddress { get; set; }

        // Dropoff info (mandatory)
        public string DropOffAddress { get; set; } = null!;

        // Optional fields
        public string? Flightnumber { get; set; }
        public string? Comment { get; set; }
        public decimal DistanceKm { get; set; }
        public decimal DurationMinutes { get; set; }
        public decimal Price { get; set; }
    }
}
