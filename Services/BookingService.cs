using Pegasus_MVC.DTO;
using Pegasus_MVC.ViewModels;
using System.Globalization;

namespace Pegasus_MVC.Services
{
    public class BookingService : IBookingService
    {
        public Task<CreateBookingDto> CreateBookingAsync(CreateBookingDto dto)
        {
            throw new NotImplementedException();
        }
        private CreateBookingDto CreateBookingRequest(CreateBookingVM createBooking)
        {
            var newBooking = new CreateBookingDto
            {
                FirstName = createBooking.FirstName,
                LastName = createBooking.LastName,
                Email = createBooking.Email,
                PhoneNumber = createBooking.PhoneNumber,

                PickUpDateTime = createBooking.PickUpDateTime,
                PickUpAddress = createBooking.PickUpAddress,


                PickUpLatitude = double.Parse(createBooking.PickUpLatitude, CultureInfo.InvariantCulture),
                PickUpLongitude = double.Parse(createBooking.PickUpLongitude, CultureInfo.InvariantCulture),

                DropOffAddress = createBooking.DropOffAddress,
                DropOffLatitude = double.Parse(createBooking.DropOffLatitude, CultureInfo.InvariantCulture),
                DropOffLongitude = double.Parse(createBooking.DropOffLongitude, CultureInfo.InvariantCulture),


                FirstStopAddress = !string.IsNullOrEmpty(createBooking.SecStop) ? createBooking.SecStop : null,
                FirstStopLatitude = !string.IsNullOrEmpty(createBooking.SecStopLatitude)
                    ? double.Parse(createBooking.SecStopLatitude, CultureInfo.InvariantCulture)
                    : 0.0,
                FirstStopLongitude = !string.IsNullOrEmpty(createBooking.SecStopLongitude)
                    ? double.Parse(createBooking.SecStopLongitude, CultureInfo.InvariantCulture)
                    : 0.0,


                SecondStopAddress = !string.IsNullOrEmpty(createBooking.SecStop) ? createBooking.SecStop : null,
                SecondStopLatitude = !string.IsNullOrEmpty(createBooking.SecStopLatitude)
                    ? double.Parse(createBooking.SecStopLatitude, CultureInfo.InvariantCulture)
                    : 0.0,
                SecondStopLongitude = !string.IsNullOrEmpty(createBooking.SecStopLongitude)
                    ? double.Parse(createBooking.SecStopLongitude, CultureInfo.InvariantCulture)
                    : 0.0,

                Flightnumber = !string.IsNullOrEmpty(createBooking.Flightnumber) ? createBooking.Flightnumber : null,
                Comment = !string.IsNullOrEmpty(createBooking.Comment) ? createBooking.Comment : null

            };

            return newBooking;
        }
    }
}
