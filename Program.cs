using FluentValidation;
using Pegasus_MVC.Services;
using Pegasus_MVC.Services.Interfaces;
using Pegasus_MVC.ViewModels;

namespace Pegasus_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IValidator<CreateBookingVM>, BookingValidator>();
            builder.Services.AddScoped<IBookingValidationService, BookingValidationService>();

            builder.Services.AddHttpClient("PegasusServer", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7161/api/");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
