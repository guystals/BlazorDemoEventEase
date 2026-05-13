# User Session Tracker - State Management System

## Overview

The User Session Tracker is a comprehensive state management solution for the EventEase Blazor WebAssembly application. It provides real-time session tracking, user activity monitoring, and persistent state management across browser sessions.

## Features

### Core Functionality
- ✅ **Unique Session ID** - Automatically generated GUID for each user session
- ✅ **User Profile Tracking** - Stores username, email, and authentication status
- ✅ **Activity Monitoring** - Tracks session start time, last activity, and idle time
- ✅ **Page View Tracking** - Monitors navigation and counts page views
- ✅ **Event Tracking** - Records viewed events and registrations
- ✅ **User Preferences** - Key-value store for custom user settings
- ✅ **Browser Storage Integration** - Persists session data in localStorage
- ✅ **Real-time Updates** - Event-driven notifications for state changes

### Session Statistics
- Session duration calculation
- Idle time monitoring
- Total page views counter
- Unique events viewed
- Registration tracking
- Active/inactive status

## Architecture

### Components

#### 1. **UserSession Model** (`Models/UserSession.cs`)
Core data model containing:
- User identification (UserId, UserName, Email)
- Session timing (SessionStartTime, LastActivity)
- Activity tracking (ViewedEventIds, RegisteredEventIds)
- User preferences (Dictionary)
- Navigation state (CurrentPage, PageViewCount)

#### 2. **UserSessionService** (`Services/UserSessionService.cs`)
Main service providing:
- Session initialization and management
- Activity tracking methods
- Event view/registration tracking
- Preference management
- Statistics generation
- Auto-save functionality (every 30 seconds)

#### 3. **BrowserStorageService** (`Services/BrowserStorageService.cs`)
JavaScript interop service for:
- localStorage operations (persistent)
- sessionStorage operations (temporary)
- Safe error handling for storage unavailability

#### 4. **SessionInfo Component** (`Components/SessionInfo.razor`)
Visual component displaying:
- Session duration
- Events viewed count
- Registration count
- User authentication status
- Auto-refreshing every 10 seconds

## Implementation

### Service Registration

Services are registered in `Program.cs`:

```csharp
// Scoped services for session management
builder.Services.AddScoped<BrowserStorageService>();
builder.Services.AddScoped<UserSessionService>();
```

### Layout Integration

The `MainLayout.razor` initializes and tracks sessions:
- Loads session from localStorage on startup
- Tracks page navigation automatically
- Auto-saves session on navigation changes
- Displays SessionInfo component in header

### Usage Examples

#### Track Event View
```csharp
@inject UserSessionService SessionService

protected override async Task OnInitializedAsync()
{
	// Track that user viewed this event
	SessionService.TrackEventView(eventId);
}
```

#### Track Registration
```csharp
private void HandleRegistration()
{
	SessionService.RegisterForEvent(
		eventId,
		firstName,
		lastName,
		email
	);
}
```

#### Set User Preference
```csharp
SessionService.SetPreference("theme", "dark");
var theme = SessionService.GetPreference("theme");
```

#### Check Registration Status
```csharp
bool isRegistered = SessionService.IsRegisteredForEvent(eventId);
```

#### Get Session Statistics
```csharp
var stats = SessionService.GetStatistics();
Console.WriteLine($"Session Duration: {stats.SessionDuration}");
Console.WriteLine($"Events Viewed: {stats.UniqueEventsViewed}");
```

## API Reference

### UserSessionService Methods

#### Session Management
- `InitializeSessionAsync(Func<string, Task<string?>>)` - Load session from storage
- `SaveSessionAsync(Func<string, string, Task>)` - Persist session to storage
- `ResetSession()` - Clear and start new session

#### Activity Tracking
- `TrackPageView(string pageName)` - Record page navigation
- `TrackEventView(int eventId)` - Record event view
- `RegisterForEvent(int eventId, string firstName, string lastName, string email)` - Record registration

#### User Preferences
- `SetPreference(string key, string value)` - Store preference
- `GetPreference(string key)` - Retrieve preference
- `ClearPreference(string key)` - Remove preference

#### Status Queries
- `IsRegisteredForEvent(int eventId)` - Check registration status
- `HasViewedEvent(int eventId)` - Check if event was viewed
- `GetSessionDuration()` - Calculate session length
- `GetIdleTime()` - Calculate time since last activity
- `GetStatistics()` - Get comprehensive session statistics

#### Authentication
- `SetAuthentication(bool isAuthenticated, string? userName, string? email)` - Update auth status

### Events
- `OnSessionChanged` - Triggered when session state changes
- `OnPageChanged` - Triggered when user navigates to new page

## Browser Storage

### localStorage (Persistent)
The session is automatically saved to localStorage with key `user_session`:
- Survives browser restarts
- Shared across tabs in same domain
- Auto-saved every 30 seconds
- Saved on navigation changes

### sessionStorage (Temporary)
Available through BrowserStorageService but not used by default:
- Cleared when tab/browser closes
- Isolated per tab
- Use for temporary data

## Demo Page

Visit `/session-demo` to see the session tracker in action:
- Real-time session information
- Activity statistics
- Event tracking visualization
- Preference management
- Simulation tools (login/logout)
- Manual save/reset controls

## Performance Considerations

### Auto-Save Timer
- Runs every 30 seconds
- Minimal performance impact
- Prevents data loss

### Component Updates
- SessionInfo updates every 10 seconds
- SessionDemo updates every 2 seconds (for demonstration)
- Event-driven updates on state changes

### Storage Optimization
- JSON serialization for compact storage
- Graceful degradation if storage unavailable
- Silent failures prevent app crashes

## Security Considerations

⚠️ **Important Security Notes:**

1. **Client-Side Storage**: All data is stored in browser localStorage
   - Not encrypted by default
   - Accessible via JavaScript
   - Should NOT store sensitive data (passwords, tokens, etc.)

2. **User Identification**: UserId is a client-generated GUID
   - Not a secure authentication mechanism
   - Should be replaced with server-side auth for production

3. **Email Storage**: User emails are stored in plain text
   - Consider encryption for sensitive implementations
   - Comply with privacy regulations (GDPR, CCPA)

## Future Enhancements

Potential improvements:
- [ ] Server-side session synchronization
- [ ] Session analytics dashboard
- [ ] Activity heatmaps
- [ ] User behavior predictions
- [ ] A/B testing support
- [ ] Multi-device session sync
- [ ] Session replay functionality
- [ ] Anonymous vs authenticated user tracking
- [ ] GDPR compliance tools
- [ ] Session expiration policies

## Troubleshooting

### Session Not Persisting
1. Check browser console for localStorage errors
2. Verify browser supports localStorage
3. Check if private/incognito mode (some browsers restrict storage)
4. Ensure storage quota not exceeded

### Statistics Not Updating
1. Verify timer is running
2. Check component is subscribed to OnSessionChanged
3. Ensure StateHasChanged() is called

### Navigation Not Tracked
1. Verify NavigationManager.LocationChanged is subscribed
2. Check MainLayout is properly initialized
3. Ensure TrackPageView is called

## License

Part of the EventEase application.

## Author

Developed for EventEase Blazor WebAssembly application demonstrating modern state management patterns in Blazor.
