using FluentValidation;
using Pegasus_MVC.ViewModels;
using System.Globalization;

public class BookingValidator : AbstractValidator<CreateBookingVM>
{
    private readonly ILogger<BookingValidator> logger;

    private readonly int minLon = -180;
    private readonly int maxLon = 180;
    private readonly int minLat = -90;
    private readonly int maxLat = 90;

    public BookingValidator(ILogger<BookingValidator> logger)
    {
        this.logger = logger;

        // Customer info
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName is required.")
            .MaximumLength(50).WithMessage("FirstName can't exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName is required.")
            .MaximumLength(50).WithMessage("LastName can't exceed 50 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^[\d\s\+\-\(\)]+$").WithMessage("Invalid phone number format.");

        // Pickup
        RuleFor(x => x.PickUpDateTime)
            .NotEmpty().WithMessage("PickUpDateTime is required.")
            .Must(date => date >= DateTime.UtcNow.AddHours(48)).WithMessage("PickUpDateTime must be in the 48 hours from now.");

        RuleFor(x => x.PickUpAddress)
            .NotEmpty().WithMessage("PickUpAddress is required.")
            .MaximumLength(300).WithMessage("PickUpAddress can't exceed 300 characters.");

        RuleFor(x => x.PickUpLatitude)
            .Must(coord => BeValidCoordinate(coord, minLat, maxLat)).WithMessage("PickUpLatitude must be between -90 and 90.");

        RuleFor(x => x.PickUpLongitude)
            .Must(coord => BeValidCoordinate(coord, minLon, maxLon)).WithMessage("PickUpLongitude must be between -180 and 180.");

        // First stop (conditional)
        When(x => !string.IsNullOrEmpty(x.FirstStop), () =>
        {
            RuleFor(x => x.FirstStop)
                .MaximumLength(300).WithMessage("FirstStop can't exceed 300 characters.");

            RuleFor(x => x.FirstStopLatitude)
                .NotNull().WithMessage("FirstStopLatitude required when FirstStop provided.")
                .Must(coord => BeValidCoordinate(coord, minLat, maxLat)).WithMessage("FirstStopLatitude must be between -90 and 90.");

            RuleFor(x => x.FirstStopLongitude)
                .NotNull().WithMessage("FirstStopLongitude required when FirstStop provided.")
                .Must(coord => BeValidCoordinate(coord, minLon, maxLon)).WithMessage("FirstStopLongitude must be between -180 and 180.");
        });

        // Second stop (conditional)
        When(x => !string.IsNullOrEmpty(x.SecStop), () =>
        {
            RuleFor(x => x.SecStop)
                .MaximumLength(300).WithMessage("SecStop can't exceed 300 characters.");

            RuleFor(x => x.SecStopLatitude)
                .NotNull().WithMessage("SecStopLatitude required when SecStop provided.")
                .Must(coord => BeValidCoordinate(coord, minLat, maxLat)).WithMessage("SecStopLatitude must be between -90 and 90.");

            RuleFor(x => x.SecStopLongitude)
                .NotNull().WithMessage("SecStopLongitude required when SecStop provided.")
                .Must(coord => BeValidCoordinate(coord, minLon, maxLon)).WithMessage("SecStopLongitude must be between -180 and 180.");
        });

        // Dropoff
        RuleFor(x => x.DropOffAddress)
            .NotEmpty().WithMessage("DropOffAddress is required.")
            .MaximumLength(300).WithMessage("DropOffAddress can't exceed 300 characters.");

        RuleFor(x => x.DropOffLatitude)
            .Must(coord => BeValidCoordinate(coord, minLat, maxLat)).WithMessage("DropOffLatitude must be between -90 and 90.");

        RuleFor(x => x.DropOffLongitude)
            .Must(coord => BeValidCoordinate(coord, minLon, maxLon)).WithMessage("DropOffLongitude must be between -180 and 180.");

        // Optional fields
        When(x => !string.IsNullOrEmpty(x.Flightnumber), () =>
        {
            RuleFor(x => x.Flightnumber)
                .MaximumLength(20).WithMessage("Flightnumber can't exceed 20 characters.");
        });

        When(x => !string.IsNullOrEmpty(x.Comment), () =>
        {
            RuleFor(x => x.Comment)
                .MaximumLength(500).WithMessage("Comment can't exceed 500 characters.");
        });
    }

    private bool BeValidCoordinate(string? coordinate, int min, int max)
    {
        logger.LogInformation($"BeValidCoordinate called with: {coordinate}, min: {min}, max: {max}");

        if (string.IsNullOrEmpty(coordinate))
            return true;

        if (!double.TryParse(coordinate, CultureInfo.InvariantCulture, out double coord))
        {
            logger.LogWarning("Parsing cordniates failed");
            return false;
        }
           

        return coord >= min && coord <= max;
    }
}