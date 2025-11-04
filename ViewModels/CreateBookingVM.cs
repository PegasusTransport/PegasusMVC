using System.ComponentModel.DataAnnotations;
namespace Pegasus_MVC.ViewModels
{
    public class CreateBookingVM
    {
        [Required(ErrorMessage = "Need a First name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Need a last name")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Need a Email")]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Need a Phonenumber")]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Need a pickup time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime PickUpDateTime { get; set; } = DateTime.UtcNow;

        // Pickup address 
        [Required(ErrorMessage = "Need a pickup address")]
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
        [Required(ErrorMessage = "Need a dropoff address")]
        public string DropOffAddress { get; set; } = null!;
        public string DropOffAddressPlaceId { get; set; } = null!;
        public string DropOffLatitude { get; set; } = null!;
        public string DropOffLongitude { get; set; } = null!;

        public string? Flightnumber { get; set; }

        [StringLength(300)]
        public string? Comment { get; set; }
    }
}