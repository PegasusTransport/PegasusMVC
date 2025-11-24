# Pegasus Transport - Airport Transfer Booking System

A modern ASP.NET Core MVC web application for booking reliable taxi transfers to and from Arlanda Airport in Stockholm, Sweden.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [API Integration](#api-integration)
- [Deployment](#deployment)
- [Development](#development)
- [Contributing](#contributing)

## 🎯 Overview

Pegasus Transport is a professional airport transfer booking platform designed to provide seamless, reliable transportation services between Arlanda Airport and various locations in Stockholm. The application offers transparent pricing, real-time flight tracking, and an intuitive booking experience.

### Key Highlights

- **Fixed pricing** within the Arlanda zone following Swedavia guidelines
- **48-hour advance booking** requirement for quality service
- **Flight tracking** for automatic pickup time adjustments
- **Multiple stop support** (up to 2 additional stops)
- **Responsive design** optimized for mobile and desktop
- **GDPR compliant** with cookie consent management

## ✨ Features

### Customer-Facing Features

#### Interactive Booking Form
- Address autocomplete with Google Maps integration
- Dynamic price calculation preview
- Support for up to 2 additional stops
- Flight number tracking for airport pickups
- Real-time coordinate validation
- Date/time picker with minimum 48-hour advance requirement

#### Booking Management
- Two-step confirmation process (Preview → Confirm)
- Session-based state management with secure tokens
- Email confirmation system
- Idempotency key protection against duplicate submissions
- Automatic session cleanup after 30 minutes

#### User Experience
- Mobile-responsive Bootstrap 5 design
- Custom animated 404/500 error pages
- Cookie consent management (GDPR compliant)
- Comprehensive FAQ page
- Contact form with Web3Forms integration
- Interactive "How it Works" section
- Social proof and trust indicators

### Technical Features

#### Validation
- **FluentValidation** for comprehensive input validation
- Coordinate validation (-90 to 90 for latitude, -180 to 180 for longitude)
- Email format and phone number validation
- Address verification via backend API
- Arlanda requirement enforcement (at least one address must be Arlanda)
- Flight number requirement for Arlanda pickups
- Character limits on all text fields

#### State Management
- In-memory caching using `IMemoryCache`
- 30-minute absolute expiration for booking states
- Secure GUID-based state identifiers
- Automatic cache cleanup after retrieval

#### API Communication
- HttpClient factory pattern for connection pooling
- Structured error handling with custom `ServiceResponse<T>` wrapper
- Network timeout handling
- HTTP status code mapping
- Idempotency key generation for safe retries

## 🛠 Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 MVC
- **Language**: C# with nullable reference types enabled
- **Validation**: FluentValidation 12.1.0
- **Caching**: ASP.NET Core In-Memory Cache
- **HTTP Client**: IHttpClientFactory with typed clients

### Frontend
- **UI Framework**: Bootstrap 5.3
- **Icons**: Font Awesome 6.5.0
- **Fonts**: Google Fonts (Open Sans, Oswald)
- **JavaScript**: Vanilla JS with jQuery 3.x
- **Notifications**: SweetAlert2
- **Forms**: Web3Forms for contact submissions

### DevOps & Deployment
- **Containerization**: Docker with multi-stage builds
- **CI/CD**: GitHub Actions workflow
- **Hosting**: Azure App Service
- **Version Control**: Git with .gitignore for sensitive files

### Development Tools
- **IDE Support**: Visual Studio 2022, VS Code
- **User Secrets**: ASP.NET Core User Secrets for local development
- **Hot Reload**: .NET 8 hot reload support

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- A code editor (Visual Studio 2022, VS Code, or Rider)
- Docker Desktop (optional, for containerized development)
- Access to the Pegasus API backend

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-org/pegasus-mvc.git
   cd pegasus-mvc
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure application settings**
   
   Create `appsettings.Development.json` (not tracked in Git):
   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*",
     "LoginPageLink": "https://portal.pegasustransport.se/login"
   }
   ```

4. **Update API base URL**
   
   In `DiServices/ServiceExtension.cs`, configure the API endpoint:
   ```csharp
   services.AddHttpClient("PegasusServer", client =>
   {
       // Development
       client.BaseAddress = new Uri("https://localhost:7161/api/");
       
       // Production
       // client.BaseAddress = new Uri("https://api.pegasustransport.se/api/");
   });
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```
   
   The application will be available at:
   - HTTP: `http://localhost:5221`
   - HTTPS: `https://localhost:7051`

### Docker Setup

1. **Build the Docker image**
   ```bash
   docker build -t pegasus-mvc .
   ```

2. **Run the container**
   ```bash
   docker run -d -p 8080:8080 --name pegasus-app pegasus-mvc
   ```

3. **Access the application**
   
   Navigate to `http://localhost:8080`

## 📁 Project Structure

```
Pegasus_MVC/
├── Controllers/              # MVC Controllers
│   ├── BookingController.cs  # Booking flow logic
│   └── HomeController.cs     # Static pages
├── Views/                    # Razor views
│   ├── Booking/
│   │   ├── Index.cshtml      # Booking form
│   │   ├── ConfirmBooking.cshtml
│   │   └── BookingSuccess.cshtml
│   ├── Home/
│   │   ├── Index.cshtml      # Landing page
│   │   ├── About.cshtml
│   │   ├── Contact.cshtml
│   │   ├── FAQ.cshtml
│   │   ├── Error404.cshtml
│   │   └── Error500.cshtml
│   └── Shared/
│       ├── _Layout.cshtml    # Master layout
│       ├── _Header.cshtml
│       ├── _Footer.cshtml
│       └── _CookieBanner.cshtml
├── Services/                 # Business logic services
│   ├── BookingService.cs     # Core booking operations
│   ├── BookingValidationService.cs
│   ├── BookingStateService.cs
│   └── Interfaces/           # Service contracts
├── DTO/                      # Data Transfer Objects
│   ├── CreateBookingDto.cs
│   ├── BookingPreviewRequestDto.cs
│   ├── BookingPreviewResponseDto.cs
│   └── Wrapper/
│       └── ApiServiceResponse.cs
├── ViewModels/               # View-specific models
│   ├── CreateBookingVM.cs
│   └── BookingPreviewVM.cs
├── Validators/               # FluentValidation validators
│   └── BookingValidator.cs
├── Response/                 # Response wrappers
│   └── ServiceResponse.cs
├── DiServices/               # Dependency injection
│   └── ServiceExtension.cs
├── wwwroot/                  # Static files
│   ├── css/                  # Stylesheets
│   ├── js/                   # JavaScript files
│   └── lib/                  # Third-party libraries
├── Properties/
│   └── launchSettings.json   # Launch profiles
├── appsettings.json          # Configuration
├── Program.cs                # Application entry point
├── Dockerfile                # Container definition
└── Pegasus_MVC.csproj        # Project file
```

## ⚙️ Configuration

### Application Settings

The `appsettings.json` file contains the core configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "LoginPageLink": "https://portal.pegasustransport.se/login"
}
```

### Environment-Specific Settings

- **Development**: `appsettings.Development.json` (gitignored)
- **Production**: Azure App Service Configuration or environment variables

### Key Configuration Points

1. **API Base URL**: Configured in `ServiceExtension.MapServices()`
2. **Session Timeout**: 30 minutes (in `BookingStateService`)
3. **Booking Lead Time**: 48 hours minimum (in `BookingValidator`)
4. **Cache Settings**: In-memory with absolute expiration

## 🔌 API Integration

### Backend API Endpoints

The application communicates with the following API endpoints:

#### 1. Preview Booking Price
```
POST /api/Booking/previewBookingPrice
```
**Request Body**: `BookingPreviewRequestDto`
**Response**: `ApiServiceResponse<BookingPreviewResponseDto>`

Calculates distance, duration, and price for the journey.

#### 2. Create Booking
```
POST /api/Booking/createBooking
```
**Request Body**: `CreateBookingDto`
**Headers**: `Idempotency-Key` (GUID)
**Response**: `ApiServiceResponse<CreateBookingDto>`

Creates a new booking in the system.

#### 3. Validate Coordinates
```
GET /api/Map/GetLongNLat?placeId={placeId}
```
**Query Parameters**: `placeId` (Google Place ID)
**Response**: Coordinate validation result

Verifies that a place ID corresponds to a valid location.

### API Response Structure

All API responses follow this wrapper format:

```json
{
  "statusCode": 200,
  "data": { /* Response data */ },
  "message": "Success message or error details"
}
```

### Error Handling

The application handles various HTTP status codes:

- **200 OK**: Success
- **400 Bad Request**: Validation errors
- **408 Request Timeout**: Network timeout
- **500 Internal Server Error**: Server-side errors
- **503 Service Unavailable**: Backend unavailable

## 🚢 Deployment

### Azure App Service Deployment

The project includes a GitHub Actions workflow for automated deployment:

**Workflow**: `.github/workflows/main_pegasustransport.yml`

#### Deployment Steps:

1. **Build**: Compiles the .NET 8 application
2. **Publish**: Creates deployment artifacts
3. **Deploy**: Pushes to Azure App Service using federated credentials

#### Azure Configuration:

- **App Name**: PegasusTransport
- **Runtime**: .NET 8
- **Authentication**: Managed Identity (Federated)

### Manual Deployment

1. **Publish the application**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Deploy to Azure**
   ```bash
   az webapp deployment source config-zip \
     --resource-group <resource-group> \
     --name PegasusTransport \
     --src ./publish.zip
   ```

### Docker Deployment

1. **Build and tag**
   ```bash
   docker build -t pegasus-mvc:latest .
   docker tag pegasus-mvc:latest your-registry.azurecr.io/pegasus-mvc:latest
   ```

2. **Push to registry**
   ```bash
   docker push your-registry.azurecr.io/pegasus-mvc:latest
   ```

3. **Deploy to Azure Container Instances or App Service**

## 💻 Development

### Running in Development Mode

```bash
dotnet watch run
```

This enables hot reload for rapid development.

### Launch Profiles

The application includes several launch profiles:

- **http**: Runs on HTTP only (port 5221)
- **https**: Runs on HTTPS (port 7051) + HTTP (port 5221)
- **IIS Express**: Runs under IIS Express
- **Docker**: Runs in a Docker container

### Adding New Features

1. **Create a new service**
   - Add interface in `Services/Interfaces/`
   - Implement in `Services/`
   - Register in `ServiceExtension.cs`

2. **Add a new page**
   - Create controller action
   - Add view in `Views/`
   - Add navigation link in `_Header.cshtml`

3. **Add validation**
   - Extend `BookingValidator` or create new validator
   - Register with FluentValidation

### Debugging

- Enable detailed logging in `appsettings.Development.json`
- Use Visual Studio debugger or VS Code launch configurations
- Check browser console for JavaScript errors
- Monitor network requests in browser DevTools

## 📝 Key Business Rules

### Booking Requirements

1. **Arlanda Requirement**: At least one address (pickup or drop-off) must be Arlanda Airport
2. **Lead Time**: All bookings must be placed at least 48 hours in advance
3. **Flight Number**: Required when pickup location is Arlanda
4. **Stops**: Maximum of 2 additional stops allowed
5. **Coordinates**: All addresses must have valid coordinates

### Pricing

- **Fixed Zone**: Swedavia-approved fixed pricing within Arlanda zone
- **Outside Zone**: Taximeter pricing until entering the fixed zone
- **Transparency**: Final price shown before booking confirmation

### Waiting Times

- **From Arlanda**: 1 hour free waiting time from landing
- **Other Pickups**: 10 minutes free waiting time

## 🧪 Testing

### Manual Testing Checklist

- [ ] Booking form validation (all fields)
- [ ] Address autocomplete functionality
- [ ] Price calculation preview
- [ ] Booking confirmation flow
- [ ] Email confirmation receipt
- [ ] Error page displays (404, 500)
- [ ] Mobile responsiveness
- [ ] Cookie consent banner
- [ ] Contact form submission

### Integration Testing

Test API integration:
- Preview price calculation
- Booking creation
- Coordinate validation
- Idempotency key handling

## 🔒 Security

### Implemented Measures

- **HTTPS**: Enforced in production
- **CSRF Protection**: ASP.NET Core anti-forgery tokens
- **Input Validation**: FluentValidation + server-side checks
- **Idempotency Keys**: Prevent duplicate submissions
- **Session Security**: Secure, time-limited state tokens
- **GDPR Compliance**: Cookie consent management

### Best Practices

- Never commit `appsettings.Development.json`
- Use User Secrets for local development
- Rotate API keys regularly
- Monitor application logs for suspicious activity

## 🤝 Contributing

### Getting Started

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Style

- Follow C# naming conventions
- Use async/await for I/O operations
- Add XML documentation comments for public APIs
- Keep methods focused and single-purpose
- Write meaningful commit messages

### Pull Request Process

1. Update documentation for any new features
2. Ensure all validation still works
3. Test on both desktop and mobile
4. Update the README if needed
5. Request review from maintainers

## 📄 License

This project is proprietary software owned by Pegasus Transport AB.

## 📧 Contact

**Pegasus Transport AB**
- Website: [https://pegasustransport.se](https://pegasustransport.se)
- Email: info@pegasustransport.se
- Phone: +46 70 540 01 85
- Address: Globen, Stockholm, Sweden

## 🙏 Acknowledgments

- Built with ASP.NET Core and Bootstrap
- Icons by Font Awesome
- Maps integration via Google Maps API
- Form submissions via Web3Forms
- Hosted on Microsoft Azure

---

**Version**: 1.0.0  
**Last Updated**: November 2025  
**Maintained by**: Pegasus Transport Development Team
