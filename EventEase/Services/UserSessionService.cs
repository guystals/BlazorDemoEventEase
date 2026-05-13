using EventEase.Models;
using System.Text.Json;

namespace EventEase.Services;

public class UserSessionService : IDisposable
{
    private const string SESSION_STORAGE_KEY = "user_session";
    private UserSession _currentSession;
    private readonly Timer _activityTimer;
    private readonly ILogger<UserSessionService> _logger;
    private Func<string, string, Task>? _saveToStorageAsync;

    public event Action? OnSessionChanged;
    public event Action<string>? OnPageChanged;

    public UserSession CurrentSession => _currentSession;

    public UserSessionService(ILogger<UserSessionService> logger)
    {
        _logger = logger;
        _currentSession = new UserSession();

        // Auto-save session every 30 seconds
        _activityTimer = new Timer(async _ => await SaveSessionAsync(), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
    }

    // Initialize session from browser storage
    public async Task InitializeSessionAsync(
        Func<string, Task<string?>> getFromStorageAsync,
        Func<string, string, Task>? saveToStorageAsync = null)
    {
        _saveToStorageAsync = saveToStorageAsync;

        try
        {
            var sessionJson = await getFromStorageAsync(SESSION_STORAGE_KEY);
            if (!string.IsNullOrEmpty(sessionJson))
            {
                _currentSession = JsonSerializer.Deserialize<UserSession>(sessionJson) ?? new UserSession();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to initialize user session from storage.");
            _currentSession = new UserSession();
        }

        UpdateLastActivity();
        NotifySessionChanged();
    }

    // Save session to browser storage
    public async Task SaveSessionAsync(Func<string, string, Task>? saveToStorageAsync = null)
    {
        try
        {
            UpdateLastActivity();
            var sessionJson = JsonSerializer.Serialize(_currentSession);
            var storageWriter = saveToStorageAsync ?? _saveToStorageAsync;
            if (storageWriter != null)
            {
                await storageWriter(SESSION_STORAGE_KEY, sessionJson);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to save user session to storage.");
        }
    }

    // Track page navigation
    public void TrackPageView(string pageName)
    {
        _currentSession.CurrentPage = pageName;
        _currentSession.PageViewCount++;
        UpdateLastActivity();
        OnPageChanged?.Invoke(pageName);
        NotifySessionChanged();
    }

    // Track event view
    public void TrackEventView(int eventId)
    {
        if (!_currentSession.ViewedEventIds.Contains(eventId))
        {
            _currentSession.ViewedEventIds.Add(eventId);
            UpdateLastActivity();
            NotifySessionChanged();
        }
    }

    // Track event registration
    public void RegisterForEvent(int eventId, string firstName, string lastName, string email)
    {
        if (!_currentSession.RegisteredEventIds.Contains(eventId))
        {
            _currentSession.RegisteredEventIds.Add(eventId);
        }

        // Update user info from registration
        if (string.IsNullOrEmpty(_currentSession.UserName))
        {
            _currentSession.UserName = $"{firstName} {lastName}";
        }
        if (string.IsNullOrEmpty(_currentSession.Email))
        {
            _currentSession.Email = email;
        }

        UpdateLastActivity();
        NotifySessionChanged();
    }

    // Check if user is registered for an event
    public bool IsRegisteredForEvent(int eventId)
    {
        return _currentSession.RegisteredEventIds.Contains(eventId);
    }

    // Check if user has viewed an event
    public bool HasViewedEvent(int eventId)
    {
        return _currentSession.ViewedEventIds.Contains(eventId);
    }

    // Set user preference
    public void SetPreference(string key, string value)
    {
        _currentSession.Preferences[key] = value;
        UpdateLastActivity();
        NotifySessionChanged();
    }

    // Get user preference
    public string? GetPreference(string key)
    {
        return _currentSession.Preferences.TryGetValue(key, out var value) ? value : null;
    }

    // Clear specific preference
    public void ClearPreference(string key)
    {
        _currentSession.Preferences.Remove(key);
        NotifySessionChanged();
    }

    // Get session duration
    public TimeSpan GetSessionDuration()
    {
        return DateTime.UtcNow - _currentSession.SessionStartTime;
    }

    // Get idle time
    public TimeSpan GetIdleTime()
    {
        return DateTime.UtcNow - _currentSession.LastActivity;
    }

    // Set authentication status
    public void SetAuthentication(bool isAuthenticated, string? userName = null, string? email = null)
    {
        _currentSession.IsAuthenticated = isAuthenticated;
        if (isAuthenticated)
        {
            _currentSession.UserName = userName ?? _currentSession.UserName;
            _currentSession.Email = email ?? _currentSession.Email;
        }
        UpdateLastActivity();
        NotifySessionChanged();
    }

    // Reset session (logout or clear)
    public void ResetSession()
    {
        var oldSessionId = _currentSession.UserId;
        _currentSession = new UserSession();
        UpdateLastActivity();
        NotifySessionChanged();
    }

    // Get session statistics
    public SessionStatistics GetStatistics()
    {
        return new SessionStatistics
        {
            SessionDuration = GetSessionDuration(),
            IdleTime = GetIdleTime(),
            TotalPageViews = _currentSession.PageViewCount,
            UniqueEventsViewed = _currentSession.ViewedEventIds.Count,
            TotalRegistrations = _currentSession.RegisteredEventIds.Count,
            IsActive = GetIdleTime().TotalMinutes < 5
        };
    }

    private void UpdateLastActivity()
    {
        _currentSession.LastActivity = DateTime.UtcNow;
    }

    private void NotifySessionChanged()
    {
        OnSessionChanged?.Invoke();
    }

    public void Dispose()
    {
        _activityTimer?.Dispose();
    }
}

public class SessionStatistics
{
    public TimeSpan SessionDuration { get; set; }
    public TimeSpan IdleTime { get; set; }
    public int TotalPageViews { get; set; }
    public int UniqueEventsViewed { get; set; }
    public int TotalRegistrations { get; set; }
    public bool IsActive { get; set; }
}
