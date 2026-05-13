# Attendance Tracker - Event Participation Monitoring System

## Overview

The Attendance Tracker is a comprehensive interface-based system for monitoring and managing event participation in the EventEase Blazor WebAssembly application. It provides real-time attendance tracking, check-in/check-out management, statistics, and reporting capabilities.

## Architecture

### Interface-Based Design

The system follows **SOLID principles** with an interface-based architecture:

```
IAttendanceService (Interface)
	↓
AttendanceService (Implementation)
```

**Benefits:**
- ✅ Loose coupling - Components depend on abstractions, not concrete implementations
- ✅ Testability - Easy to create mock implementations for unit testing
- ✅ Flexibility - Swap implementations without changing dependent code
- ✅ Future-proof - Easy to add database or API-backed implementations

## Core Components

### 1. Models

#### **AttendanceRecord** (`Models/AttendanceRecord.cs`)
Core data model representing an attendee:

```csharp
public class AttendanceRecord
{
	public int Id { get; set; }
	public int EventId { get; set; }
	public string AttendeeName { get; set; }
	public string AttendeeEmail { get; set; }
	public string? PhoneNumber { get; set; }
	public string? Company { get; set; }
	public DateTime RegistrationDate { get; set; }
	public bool IsCheckedIn { get; set; }
	public DateTime? CheckInTime { get; set; }
	public DateTime? CheckOutTime { get; set; }
	public AttendanceStatus Status { get; set; }
	public string? Notes { get; set; }
}
```

#### **AttendanceStatus Enum**
```csharp
public enum AttendanceStatus
{
	Registered,   // Initially registered
	CheckedIn,    // Physically present at event
	CheckedOut,   // Left the event
	NoShow,       // Registered but didn't attend
	Cancelled     // Cancelled registration
}
```

#### **AttendanceStatistics** (`Services/IAttendanceService.cs`)
Aggregated statistics model:

```csharp
public class AttendanceStatistics
{
	public int TotalRegistered { get; set; }
	public int CheckedIn { get; set; }
	public int CheckedOut { get; set; }
	public int NoShows { get; set; }
	public int Cancelled { get; set; }
	public double AttendanceRate { get; set; }
	public DateTime? FirstCheckIn { get; set; }
	public DateTime? LastCheckIn { get; set; }
	public TimeSpan? AverageStayDuration { get; set; }
}
```

### 2. Services

#### **IAttendanceService Interface** (`Services/IAttendanceService.cs`)

Defines all attendance operations:

**CRUD Operations:**
- `AddAttendeeAsync()` - Add new attendee
- `GetAttendeeAsync()` - Get attendee by ID
- `UpdateAttendeeAsync()` - Update attendee info
- `DeleteAttendeeAsync()` - Remove attendee

**Event Queries:**
- `GetEventAttendeesAsync()` - Get all attendees for an event
- `GetCheckedInAttendeesAsync()` - Get currently checked-in attendees
- `GetAttendeesByStatusAsync()` - Filter by status

**Check-in Management:**
- `CheckInAttendeeAsync()` - Check in single attendee
- `CheckOutAttendeeAsync()` - Check out attendee
- `BulkCheckInAsync()` - Check in multiple attendees at once

**Search & Filter:**
- `SearchAttendeesAsync()` - Search by name, email, or company
- `FindAttendeeByEmailAsync()` - Lookup by email

**Statistics:**
- `GetEventStatisticsAsync()` - Comprehensive statistics
- `GetTotalAttendeesCountAsync()` - Total count
- `GetCheckedInCountAsync()` - Current check-in count
- `GetAttendanceRateAsync()` - Percentage who attended

**Status Management:**
- `UpdateAttendeeStatusAsync()` - Change status
- `MarkAsNoShowAsync()` - Mark as no-show
- `CancelAttendanceAsync()` - Cancel attendance

**Auto-Registration:**
- `RegisterAttendeeFromEventAsync()` - Auto-add from event registration

#### **AttendanceService Implementation** (`Services/AttendanceService.cs`)

In-memory implementation using `List<AttendanceRecord>`:
- Thread-safe operations
- Auto-incremented IDs
- Efficient LINQ queries
- Comprehensive statistics calculation

### 3. Components

#### **AttendanceStats** (`Components/AttendanceStats.razor`)

Visual statistics display component.

**Parameters:**
- `EventId` (required) - Event to show stats for
- `ShowExtendedStats` (bool) - Show additional metrics
- `AutoRefresh` (bool) - Auto-refresh every 10 seconds

**Features:**
- Color-coded stat cards
- Real-time updates
- Responsive grid layout
- Loading states

**Usage:**
```razor
<AttendanceStats EventId="5" ShowExtendedStats="true" />
```

#### **AttendanceList** (`Components/AttendanceList.razor`)

Attendee list table with inline actions.

**Parameters:**
- `EventId` (required) - Event to show attendees for
- `ShowPhone` (bool) - Display phone column
- `ShowCompany` (bool) - Display company column
- `ShowActions` (bool) - Show check-in/out buttons
- `ShowPagination` (bool) - Enable pagination
- `FilterByStatus` (AttendanceStatus?) - Filter by status
- `OnAttendanceChanged` (EventCallback) - Callback on changes

**Features:**
- Inline check-in/check-out
- Status badges with color coding
- Row highlighting by status
- Optional pagination
- Responsive table design

**Usage:**
```razor
<AttendanceList EventId="5" 
			   ShowPhone="true" 
			   ShowCompany="true" 
			   ShowActions="true" 
			   OnAttendanceChanged="RefreshData" />
```

### 4. Pages

#### **AttendanceTracker** (`Pages/AttendanceTracker.razor`)

Main attendance management page.

**Routes:**
- `/attendance` - Select event view
- `/attendance/{eventId}` - Direct to specific event

**Features:**
- Event dropdown selector
- Attendance statistics dashboard
- Search and filter functionality
- Add attendee modal dialog
- Quick action buttons
- Responsive layout

**Sections:**
1. **Event Selection** - Dropdown to choose event
2. **Quick Actions** - Add attendee, refresh, search toggle
3. **Statistics Dashboard** - AttendanceStats component
4. **Search & Filter** - Search box and status filter
5. **Attendee List** - Full attendee table with actions
6. **Add Modal** - Form to manually add attendees

## Integration

### Service Registration (`Program.cs`)

```csharp
builder.Services.AddSingleton<IAttendanceService, AttendanceService>();
```

**Why Singleton?**
- Maintains state across app lifetime
- Consistent with EventService pattern
- Single source of truth for attendance data

### Auto-Registration Integration

When users register for events via `Registration.razor`, they're automatically added to attendance tracker:

```csharp
await AttendanceService.RegisterAttendeeFromEventAsync(
	eventId,
	firstName,
	lastName,
	email,
	phone,
	company
);
```

### Event Details Integration

`EventDetails.razor` shows attendance summary with link to full tracker:

```razor
<AttendanceStats EventId="@currentEvent.Id" ShowExtendedStats="false" />
```

## User Workflows

### 1. Event Organizer Workflow

**Setup:**
1. Navigate to Attendance Tracker
2. Select event from dropdown
3. View initial attendee list (auto-populated from registrations)

**During Event:**
1. Check in attendees as they arrive
2. Monitor real-time attendance statistics
3. Search for specific attendees
4. Add walk-in attendees manually

**After Event:**
1. Mark no-shows
2. Review attendance statistics
3. Export/view final attendance report

### 2. Quick Check-In Workflow

1. Open Attendance Tracker for event
2. Search for attendee by name/email
3. Click check-in button
4. Statistics auto-update

### 3. Bulk Operations Workflow

1. Filter attendees by status (e.g., "Registered")
2. Use bulk check-in (via service API)
3. Verify statistics update

## API Usage Examples

### Check In Attendee

```csharp
@inject IAttendanceService AttendanceService

private async Task CheckInAttendee(int attendeeId)
{
	var success = await AttendanceService.CheckInAttendeeAsync(attendeeId);
	if (success)
	{
		// Update UI
		await LoadData();
	}
}
```

### Get Statistics

```csharp
private async Task LoadStats()
{
	var stats = await AttendanceService.GetEventStatisticsAsync(eventId);
	Console.WriteLine($"Attendance Rate: {stats.AttendanceRate:F1}%");
	Console.WriteLine($"Checked In: {stats.CheckedIn} / {stats.TotalRegistered}");
}
```

### Search Attendees

```csharp
private async Task SearchAttendees(string searchTerm)
{
	var results = await AttendanceService.SearchAttendeesAsync(eventId, searchTerm);
	// Display results
}
```

### Add Manual Attendee

```csharp
private async Task AddWalkIn()
{
	var record = new AttendanceRecord
	{
		EventId = eventId,
		AttendeeName = "John Doe",
		AttendeeEmail = "john@example.com",
		PhoneNumber = "555-1234"
	};

	await AttendanceService.AddAttendeeAsync(record);
}
```

## Styling & UI

### Color Coding

**Status Badges:**
- 🔵 Registered - Blue (`bg-primary`)
- 🟢 Checked In - Green (`bg-success`)
- 🔵 Checked Out - Cyan (`bg-info`)
- 🔴 No Show - Red (`bg-danger`)
- ⚫ Cancelled - Gray (`bg-secondary`)

**Table Row Highlighting:**
- Green tint for checked-in attendees
- Cyan tint for checked-out attendees
- Red tint for no-shows
- Gray tint for cancelled

### Responsive Design

- Mobile-friendly grid layout
- Stacked cards on small screens
- Horizontal scrolling tables
- Touch-optimized buttons

## Statistics Calculation

### Attendance Rate Formula

```
Attendance Rate = (CheckedIn + CheckedOut) / (TotalRegistered + NoShows) × 100
```

### Average Stay Duration

Calculated from attendees who have both check-in and check-out times:

```csharp
var avgTicks = completedStays.Average(a => 
	(a.CheckOutTime!.Value - a.CheckInTime!.Value).Ticks
);
stats.AverageStayDuration = TimeSpan.FromTicks((long)avgTicks);
```

## Future Enhancements

### Planned Features

- [ ] **Database Integration** - Replace in-memory storage with EF Core
- [ ] **Real-time Updates** - SignalR for live multi-user updates
- [ ] **QR Code Check-In** - Generate QR codes for attendees
- [ ] **Barcode Scanning** - Mobile camera check-in
- [ ] **Export to Excel** - Download attendance reports
- [ ] **Email Notifications** - Auto-send check-in confirmations
- [ ] **Badge Printing** - Print name badges for attendees
- [ ] **Analytics Dashboard** - Trend analysis across events
- [ ] **Capacity Warnings** - Alert when approaching capacity
- [ ] **Wait List Management** - Handle over-registration

### Alternative Implementations

The interface-based design allows easy swapping of implementations:

**API-Based Implementation:**
```csharp
public class ApiAttendanceService : IAttendanceService
{
	private readonly HttpClient _http;

	public async Task<AttendanceRecord> AddAttendeeAsync(AttendanceRecord record)
	{
		var response = await _http.PostAsJsonAsync("/api/attendance", record);
		return await response.Content.ReadFromJsonAsync<AttendanceRecord>();
	}
	// ... other methods
}
```

**Database Implementation:**
```csharp
public class DbAttendanceService : IAttendanceService
{
	private readonly AppDbContext _context;

	public async Task<AttendanceRecord> AddAttendeeAsync(AttendanceRecord record)
	{
		_context.Attendance.Add(record);
		await _context.SaveChangesAsync();
		return record;
	}
	// ... other methods
}
```

## Performance Considerations

### Current Implementation

- **In-Memory Storage** - Fast read/write, limited by memory
- **LINQ Queries** - Efficient for small-to-medium datasets (<10,000 records)
- **No Database Overhead** - Zero latency for queries

### Optimization Tips

For large events (1000+ attendees):
1. Implement pagination in all lists
2. Add debouncing to search inputs
3. Cache statistics calculations
4. Consider indexed database storage

## Testing Guide

### Unit Testing with Interface

```csharp
public class MockAttendanceService : IAttendanceService
{
	private List<AttendanceRecord> _records = new();

	public Task<AttendanceRecord> AddAttendeeAsync(AttendanceRecord record)
	{
		record.Id = _records.Count + 1;
		_records.Add(record);
		return Task.FromResult(record);
	}
	// ... implement other methods
}

[Fact]
public async Task CheckIn_ShouldUpdateStatus()
{
	// Arrange
	var service = new MockAttendanceService();
	var attendee = await service.AddAttendeeAsync(new AttendanceRecord
	{
		EventId = 1,
		AttendeeName = "Test User",
		AttendeeEmail = "test@example.com"
	});

	// Act
	await service.CheckInAttendeeAsync(attendee.Id);
	var updated = await service.GetAttendeeAsync(attendee.Id);

	// Assert
	Assert.True(updated.IsCheckedIn);
	Assert.Equal(AttendanceStatus.CheckedIn, updated.Status);
}
```

## Troubleshooting

### Issue: Statistics Not Updating

**Solution:** Force component refresh with `@key` directive:
```razor
<AttendanceStats EventId="5" @key="statsKey" />
```

### Issue: Attendees Not Auto-Registering

**Check:**
1. IAttendanceService is injected in Registration.razor
2. RegisterAttendeeFromEventAsync is called in HandleRegistration
3. Service is registered in Program.cs

### Issue: Interface Dependency Injection Fails

**Error:** `Unable to resolve service for type 'IAttendanceService'`

**Solution:** Ensure Program.cs contains:
```csharp
builder.Services.AddSingleton<IAttendanceService, AttendanceService>();
```

## Security Considerations

⚠️ **Current Implementation:**
- Client-side only - no server validation
- Anyone can modify attendance data
- No authentication/authorization

🔒 **Production Requirements:**
- Add role-based access control
- Validate check-in requests server-side
- Implement audit logging
- Encrypt sensitive attendee data

## License

Part of the EventEase application.

## Support

For issues or questions about the Attendance Tracker:
1. Check this documentation
2. Review the source code comments
3. Test with sample data first
4. Check browser console for errors

---

**Version:** 1.0.0  
**Last Updated:** 2025  
**Author:** EventEase Development Team
