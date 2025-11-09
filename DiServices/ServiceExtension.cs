using FluentValidation;
using Pegasus_MVC.Services;
using Pegasus_MVC.Services.Interfaces;
using Pegasus_MVC.ViewModels;
using System.Runtime.CompilerServices;

namespace Pegasus_MVC.DiServices
{
    public static class ServiceExtension
    {
        public static IServiceCollection MapServices(this IServiceCollection services) 
        {
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IValidator<CreateBookingVM>, BookingValidator>();
            services.AddScoped<IBookingValidationService, BookingValidationService>();
            services.AddScoped<IBookingStateService, BookingStateService>();
            services.AddMemoryCache();
            services.AddHttpClient("PegasusServer", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7161/api/");
            });
            return services;
        }
        
    }
}
