using Pegasus_MVC.DTO;

namespace Pegasus_MVC.Services
{
    public interface IBookingService
    {
        Task<CreateBookingDto> CreateBookingAsync(CreateBookingDto dto);
    }
}
