using Microsoft.Extensions.Caching.Memory;
using Pegasus_MVC.DTO;
using Pegasus_MVC.Response;
using Pegasus_MVC.Services.Interfaces;
using Pegasus_MVC.ViewModels;

namespace Pegasus_MVC.Services
{
    public class BookingStateService(IMemoryCache cache, ILogger<BookingStateService> logger) : IBookingStateService
    {
        public string SaveBookingState(CreateBookingDto booking)
        {
            var stateId = Guid.NewGuid().ToString();

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };

            cache.Set(stateId, booking, cacheOptions);
            logger.LogWarning($"Generated StateId: {stateId}");
            return stateId;
        }

        public CreateBookingDto? GetBookingState(string stateId)
        {
            if (cache.TryGetValue(stateId, out CreateBookingDto? booking))
            {
                cache.Remove(stateId);
                return booking;
            }

            return null;
        }
    }
}
