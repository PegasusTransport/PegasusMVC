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

        [Required(ErrorMessage = "Need a pickup addres")]
        public string PickUpAddress { get; set; } = null!;
        public string PickUpAddressPlaceId { get; set; } = null!;  

        public string? FirstStop { get; set; }
        public string? FirstStopPlaceId { get; set; } 

        public string? SecStop { get; set; }
        public string? SecStopPlaceId { get; set; } 

        [Required(ErrorMessage = "Need a dropoff addres")]
        public string DropOffAddress { get; set; } = null!;
        public string DropOffAddressPlaceId { get; set; } = null!; 
        public string? Flightnumber { get; set; }
        [StringLength(300)]
        public string? Comment { get; set; }
        public decimal Price { get; set; }
    }
}