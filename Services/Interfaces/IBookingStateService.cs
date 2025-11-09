using Pegasus_MVC.DTO;

namespace Pegasus_MVC.Services.Interfaces
{
    public interface IBookingStateService
    {
        string SaveBookingState(CreateBookingDto booking);
        CreateBookingDto? GetBookingState(string stateId);
    }
}
