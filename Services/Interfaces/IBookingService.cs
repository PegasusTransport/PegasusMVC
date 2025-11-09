using Pegasus_MVC.DTO;
using Pegasus_MVC.Response;
using Pegasus_MVC.ViewModels;
using System.Net;

namespace Pegasus_MVC.Services.Interfaces
{
    public interface IBookingService
    {
        Task<ServiceResponse<CreateBookingDto>> CreateBookingAsync(CreateBookingDto newBooking);
        Task<ServiceResponse<BookingPreviewVM>> GetPreview(CreateBookingVM bookingRequest);
        CreateBookingDto CreateBookingDto(CreateBookingVM createBooking);
    }
}
