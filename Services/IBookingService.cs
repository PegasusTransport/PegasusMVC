using Pegasus_MVC.DTO;
using Pegasus_MVC.Response;
using Pegasus_MVC.ViewModels;
using System.Net;

namespace Pegasus_MVC.Services
{
    public interface IBookingService
    {
        Task<ServiceResponse<CreateBookingDto>> CreateBookingAsync(CreateBookingVM newBooking);
        public bool CheckArlandaRequirement(CreateBookingVM bookingDto);
    }
}
