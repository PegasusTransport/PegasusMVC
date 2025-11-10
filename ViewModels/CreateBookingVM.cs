using System.ComponentModel.DataAnnotations;

namespace Pegasus_MVC.ViewModels
{
    public class CreateBookingVM
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)] // For show in view
        public DateTime PickUpDateTime { get; set; } = GetSwedishTime().AddHours(48);
        private static DateTime GetSwedishTime()
        {
            var swedishTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, swedishTimeZone);
        }
        // Pickup address 
        public string PickUpAddress { get; set; } = null!;
        public string PickUpAddressPlaceId { get; set; } = null!;
        public string PickUpLatitude { get; set; } = null!;
        public string PickUpLongitude { get; set; } = null!;
        // First stop (optional) 
        public string? FirstStop { get; set; }
        public string? FirstStopPlaceId { get; set; }
        public string? FirstStopLatitude { get; set; }
        public string? FirstStopLongitude { get; set; }
        // Second stop (optional) 
        public string? SecStop { get; set; }
        public string? SecStopPlaceId { get; set; }
        public string? SecStopLatitude { get; set; }
        public string? SecStopLongitude { get; set; }
        // Dropoff address 
        public string DropOffAddress { get; set; } = null!;
        public string DropOffAddressPlaceId { get; set; } = null!;
        public string DropOffLatitude { get; set; } = null!;
        public string DropOffLongitude { get; set; } = null!;
        public string? Flightnumber { get; set; }
        public string? Comment { get; set; }
    }
}