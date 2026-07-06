# EventEase

A modern event management system built with Blazor WebAssembly and .NET 10.
---------------------------------------------------------------------------
## Features

### Event Management
- **Browse Events**: View and search through corporate and social events
- **Event Details**: Comprehensive event information including date, location, capacity, and descriptions
- **Add/Edit Events**: Create and modify events with validation
- **Category Filtering**: Filter events by Technology, Business, Social, Corporate, Education, and Networking
- **Advanced Search**: Case-insensitive search across event names, locations, and descriptions
- **Sorting**: Sort events by date, name, or capacity

### Attendance Tracking
- **Registration System**: Attendees can register for events with contact information
- **Check-in/Check-out**: Track event attendance in real-time
- **Attendance Statistics**: View comprehensive stats including:
  - Total registered attendees
  - Check-in/check-out counts
  - Attendance rates
  - Average stay duration
  - No-shows and cancellations
- **Attendee Search**: Search attendees by name, email, or company
- **Auto-refresh**: Real-time statistics updates

### User Session Tracking
- **Session Management**: Track user activity and session duration
- **Event View History**: Monitor which events users have viewed
- **Browser Storage**: Persistent session data using localStorage

## Architecture

### Repository Pattern
The application implements a clean architecture with separation of concerns:

```
├── Models/              # Data models and DTOs
├── Repositories/        # Data access layer
│   ├── IEventRepository
│   └── EventRepository
├── Services/           # Business logic layer
│   ├── IEventService
│   ├── EventService
│   ├── IAttendanceService
│   ├── AttendanceService
│   ├── UserSessionService
│   └── BrowserStorageService
├── Pages/              # Blazor pages/routes
└── Components/         # Reusable Blazor components
```

### Key Components
- **EventCard**: Reusable event display component
- **AttendanceStats**: Real-time attendance statistics with auto-refresh
- **AttendanceList**: Sortable and filterable attendee list
- **RegistrationStatus**: User registration status indicator

## Technology Stack

- **.NET 10**: Latest .NET framework
- **Blazor WebAssembly**: Client-side web framework
- **Bootstrap 5**: UI framework with Bootstrap Icons
- **C# 13**: Modern C# features
- **Dependency Injection**: Built-in DI container
- **Repository Pattern**: Clean data access abstraction

## Performance Optimizations

- **StringComparison.OrdinalIgnoreCase**: Efficient case-insensitive string comparisons (no allocations)
- **Debounced Search**: 300ms debounce for search input to reduce re-renders
- **IDisposable Implementation**: Proper cleanup of timers and resources
- **Component Lifecycle**: Optimized OnInitializedAsync and OnParametersSetAsync
- **Auto-refresh Controls**: Configurable refresh intervals for real-time data

## Getting Started

### Prerequisites
- .NET 10 SDK
- Visual Studio 2026 or VS Code with C# extension

### Running the Application

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/EventEase.git
   cd EventEase
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Run the application:
   ```bash
   dotnet run --project EventEase
   ```

4. Open your browser to `https://localhost:5001` (or the URL shown in the terminal)

### Generate Test Data

Uncomment the following line in `Program.cs` to generate test events:
```csharp
repository.GenerateTestData(500); // Generates 500 test events
```

## Project Structure

```
EventEase/
├── Components/          # Reusable Blazor components
├── Layout/             # Application layout components
├── Models/             # Domain models
├── Pages/              # Routable Blazor pages
├── Repositories/       # Data access layer
├── Services/           # Business logic
├── wwwroot/            # Static assets (CSS, JS, images)
├── Program.cs          # Application entry point
└── _Imports.razor      # Global using statements
```

## Key Features Implementation

### Interface-based Dependency Injection
All services use interface-based DI for better testability and maintainability:
```csharp
builder.Services.AddSingleton<IEventRepository, EventRepository>();
builder.Services.AddSingleton<IEventService, EventService>();
builder.Services.AddSingleton<IAttendanceService, AttendanceService>();
```

### Proper Resource Disposal
Components properly implement `IDisposable` for cleanup:
```csharp
@implements IDisposable

public void Dispose()
{
	timer?.Dispose();
}
```

## Future Enhancements

- [ ] Database integration (Entity Framework Core)
- [ ] User authentication and authorization
- [ ] Email notifications for registrations
- [ ] Calendar integration
- [ ] Export attendance reports (PDF/Excel)
- [ ] QR code check-in system
- [ ] Multi-language support
- [ ] Dark mode theme

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Built with Blazor WebAssembly
- UI powered by Bootstrap 5
- Icons from Bootstrap Icons
