using Microsoft.AspNetCore.Mvc.ModelBinding;
using Pegasus_MVC.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Pegasus_MVC.Services.Interfaces
{
    public interface IBookingValidationService
    {
        Task ValidateBookingAsync(CreateBookingVM booking, ModelStateDictionary modelState);
    }
}
