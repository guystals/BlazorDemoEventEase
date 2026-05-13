using EventEase.Models;
using EventEase.Services;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventEase.Tests.Services;

public class UserSessionServiceTests : IDisposable
{
    private readonly UserSessionService _sessionService;
    private readonly Mock<ILogger<UserSessionService>> _loggerMock;

    public UserSessionServiceTests()
    {
        _loggerMock = new Mock<ILogger<UserSessionService>>();
        _sessionService = new UserSessionService(_loggerMock.Object);
    }

    public void Dispose()
    {
        _sessionService.Dispose();
    }

    #region Initialization Tests

    [Fact]
    public void Constructor_ShouldInitializeWithNewSession()
    {
        var loggerMock = new Mock<ILogger<UserSessionService>>();
        var service = new UserSessionService(loggerMock.Object);

        Assert.NotNull(service.CurrentSession);
        Assert.NotNull(service.CurrentSession.UserId);
        Assert.Null(service.CurrentSession.UserName);
        Assert.Null(service.CurrentSession.Email);
        Assert.False(service.CurrentSession.IsAuthenticated);
        Assert.Empty(service.CurrentSession.ViewedEventIds);
        Assert.Empty(service.CurrentSession.RegisteredEventIds);
        Assert.Empty(service.CurrentSession.Preferences);
        Assert.Equal(0, service.CurrentSession.PageViewCount);

        service.Dispose();
    }

    [Fact]
    public async Task InitializeSessionAsync_WithValidStorageData_ShouldLoadSession()
    {
        var existingSession = new UserSession
        {
            UserId = "test-user-123",
            UserName = "John Doe",
            Email = "john@example.com",
            IsAuthenticated = true,
            PageViewCount = 5
        };
        existingSession.ViewedEventIds.Add(1);
        existingSession.ViewedEventIds.Add(2);

        var sessionJson = JsonSerializer.Serialize(existingSession);
        Func<string, Task<string?>> mockStorage = _ => Task.FromResult<string?>(sessionJson);

        await _sessionService.InitializeSessionAsync(mockStorage);

        Assert.Equal("test-user-123", _sessionService.CurrentSession.UserId);
        Assert.Equal("John Doe", _sessionService.CurrentSession.UserName);
        Assert.Equal("john@example.com", _sessionService.CurrentSession.Email);
        Assert.True(_sessionService.CurrentSession.IsAuthenticated);
        Assert.Equal(5, _sessionService.CurrentSession.PageViewCount);
        Assert.Equal(2, _sessionService.CurrentSession.ViewedEventIds.Count);
    }

    [Fact]
    public async Task InitializeSessionAsync_WithEmptyStorage_ShouldCreateNewSession()
    {
        Func<string, Task<string?>> mockStorage = _ => Task.FromResult<string?>(null);

        await _sessionService.InitializeSessionAsync(mockStorage);

        Assert.NotNull(_sessionService.CurrentSession);
        Assert.NotNull(_sessionService.CurrentSession.UserId);
    }

    [Fact]
    public async Task InitializeSessionAsync_WithInvalidJson_ShouldCreateNewSession()
    {
        Func<string, Task<string?>> mockStorage = _ => Task.FromResult<string?>("invalid json");

        await _sessionService.InitializeSessionAsync(mockStorage);

        Assert.NotNull(_sessionService.CurrentSession);
        Assert.NotNull(_sessionService.CurrentSession.UserId);
    }

    [Fact]
    public async Task InitializeSessionAsync_ShouldUpdateLastActivity()
    {
        var beforeInit = DateTime.UtcNow;
        Func<string, Task<string?>> mockStorage = _ => Task.FromResult<string?>(null);

        await _sessionService.InitializeSessionAsync(mockStorage);

        Assert.True(_sessionService.CurrentSession.LastActivity >= beforeInit);
    }

    [Fact]
    public async Task InitializeSessionAsync_ShouldTriggerSessionChangedEvent()
    {
        var eventTriggered = false;
        _sessionService.OnSessionChanged += () => eventTriggered = true;
        Func<string, Task<string?>> mockStorage = _ => Task.FromResult<string?>(null);

        await _sessionService.InitializeSessionAsync(mockStorage);

        Assert.True(eventTriggered);
    }

    #endregion

    #region Save Session Tests

    [Fact]
    public async Task SaveSessionAsync_WithStorageFunction_ShouldSerializeAndSave()
    {
        string? savedJson = null;
        string? savedKey = null;
        Func<string, string, Task> mockStorage = (key, value) =>
        {
            savedKey = key;
            savedJson = value;
            return Task.CompletedTask;
        };

        _sessionService.TrackPageView("Home");
        await _sessionService.SaveSessionAsync(mockStorage);

        Assert.NotNull(savedJson);
        Assert.Equal("user_session", savedKey);
        var deserializedSession = JsonSerializer.Deserialize<UserSession>(savedJson);
        Assert.NotNull(deserializedSession);
        Assert.Equal("Home", deserializedSession.CurrentPage);
    }

    [Fact]
    public async Task SaveSessionAsync_WithoutStorageFunction_ShouldNotThrow()
    {
        var exception = await Record.ExceptionAsync(async () => await _sessionService.SaveSessionAsync());

        Assert.Null(exception);
    }

    [Fact]
    public async Task SaveSessionAsync_ShouldUpdateLastActivity()
    {
        var beforeSave = DateTime.UtcNow;

        await _sessionService.SaveSessionAsync();

        Assert.True(_sessionService.CurrentSession.LastActivity >= beforeSave);
    }

    #endregion

    #region Page Tracking Tests

    [Fact]
    public void TrackPageView_ShouldUpdateCurrentPage()
    {
        _sessionService.TrackPageView("Events");

        Assert.Equal("Events", _sessionService.CurrentSession.CurrentPage);
    }

    [Fact]
    public void TrackPageView_ShouldIncrementPageViewCount()
    {
        _sessionService.TrackPageView("Home");
        _sessionService.TrackPageView("Events");
        _sessionService.TrackPageView("About");

        Assert.Equal(3, _sessionService.CurrentSession.PageViewCount);
    }

    [Fact]
    public void TrackPageView_ShouldUpdateLastActivity()
    {
        var beforeTracking = DateTime.UtcNow;

        _sessionService.TrackPageView("Home");

        Assert.True(_sessionService.CurrentSession.LastActivity >= beforeTracking);
    }

    [Fact]
    public void TrackPageView_ShouldTriggerPageChangedEvent()
    {
        string? changedPage = null;
        _sessionService.OnPageChanged += (page) => changedPage = page;

        _sessionService.TrackPageView("Events");

        Assert.Equal("Events", changedPage);
    }

    [Fact]
    public void TrackPageView_ShouldTriggerSessionChangedEvent()
    {
        var eventTriggered = false;
        _sessionService.OnSessionChanged += () => eventTriggered = true;

        _sessionService.TrackPageView("Home");

        Assert.True(eventTriggered);
    }

    #endregion

    #region Event Tracking Tests

    [Fact]
    public void TrackEventView_ShouldAddEventToViewedList()
    {
        _sessionService.TrackEventView(1);

        Assert.Contains(1, _sessionService.CurrentSession.ViewedEventIds);
    }

    [Fact]
    public void TrackEventView_WithSameEventMultipleTimes_ShouldAddOnlyOnce()
    {
        _sessionService.TrackEventView(1);
        _sessionService.TrackEventView(1);
        _sessionService.TrackEventView(1);

        Assert.Single(_sessionService.CurrentSession.ViewedEventIds);
    }

    [Fact]
    public void TrackEventView_WithMultipleEvents_ShouldAddAll()
    {
        _sessionService.TrackEventView(1);
        _sessionService.TrackEventView(2);
        _sessionService.TrackEventView(3);

        Assert.Equal(3, _sessionService.CurrentSession.ViewedEventIds.Count);
        Assert.Contains(1, _sessionService.CurrentSession.ViewedEventIds);
        Assert.Contains(2, _sessionService.CurrentSession.ViewedEventIds);
        Assert.Contains(3, _sessionService.CurrentSession.ViewedEventIds);
    }

    [Fact]
    public void TrackEventView_ShouldUpdateLastActivity()
    {
        var beforeTracking = DateTime.UtcNow;

        _sessionService.TrackEventView(1);

        Assert.True(_sessionService.CurrentSession.LastActivity >= beforeTracking);
    }

    [Fact]
    public void TrackEventView_ShouldTriggerSessionChangedEvent()
    {
        var eventTriggered = false;
        _sessionService.OnSessionChanged += () => eventTriggered = true;

        _sessionService.TrackEventView(1);

        Assert.True(eventTriggered);
    }

    [Fact]
    public void HasViewedEvent_WithViewedEvent_ShouldReturnTrue()
    {
        _sessionService.TrackEventView(1);

        Assert.True(_sessionService.HasViewedEvent(1));
    }

    [Fact]
    public void HasViewedEvent_WithNotViewedEvent_ShouldReturnFalse()
    {
        Assert.False(_sessionService.HasViewedEvent(1));
    }

    #endregion

    #region Event Registration Tests

    [Fact]
    public void RegisterForEvent_ShouldAddEventToRegisteredList()
    {
        _sessionService.RegisterForEvent(1, "John", "Doe", "john@example.com");

        Assert.Contains(1, _sessionService.CurrentSession.RegisteredEventIds);
    }

    [Fact]
    public void RegisterForEvent_ShouldSetUserName()
    {
        _sessionService.RegisterForEvent(1, "John", "Doe", "john@example.com");

        Assert.Equal("John Doe", _sessionService.CurrentSession.UserName);
    }

    [Fact]
    public void RegisterForEvent_ShouldSetEmail()
    {
        _sessionService.RegisterForEvent(1, "John", "Doe", "john@example.com");

        Assert.Equal("john@example.com", _sessionService.CurrentSession.Email);
    }

    [Fact]
    public void RegisterForEvent_WithExistingUserName_ShouldNotOverwrite()
    {
        _sessionService.RegisterForEvent(1, "John", "Doe", "john@example.com");
        _sessionService.RegisterForEvent(2, "Jane", "Smith", "jane@example.com");

        Assert.Equal("John Doe", _sessionService.CurrentSession.UserName);
    }

    [Fact]
    public void RegisterForEvent_WithExistingEmail_ShouldNotOverwrite()
    {
        _sessionService.RegisterForEvent(1, "John", "Doe", "john@example.com");
        _sessionService.RegisterForEvent(2, "Jane", "Smith", "jane@example.com");

        Assert.Equal("john@example.com", _sessionService.CurrentSession.Email);
    }

    [Fact]
    public void RegisterForEvent_WithSameEventMultipleTimes_ShouldNotDuplicate()
    {
        _sessionService.RegisterForEvent(1, "John", "Doe", "john@example.com");
        _sessionService.RegisterForEvent(1, "John", "Doe", "john@example.com");

        Assert.Single(_sessionService.CurrentSession.RegisteredEventIds);
    }

    [Fact]
    public void RegisterForEvent_ShouldUpdateLastActivity()
    {
        var beforeRegistration = DateTime.UtcNow;

        _sessionService.RegisterForEvent(1, "John", "Doe", "john@example.com");

        Assert.True(_sessionService.CurrentSession.LastActivity >= beforeRegistration);
    }

    [Fact]
    public void RegisterForEvent_ShouldTriggerSessionChangedEvent()
    {
        var eventTriggered = false;
        _sessionService.OnSessionChanged += () => eventTriggered = true;

        _sessionService.RegisterForEvent(1, "John", "Doe", "john@example.com");

        Assert.True(eventTriggered);
    }

    [Fact]
    public void IsRegisteredForEvent_WithRegisteredEvent_ShouldReturnTrue()
    {
        _sessionService.RegisterForEvent(1, "John", "Doe", "john@example.com");

        Assert.True(_sessionService.IsRegisteredForEvent(1));
    }

    [Fact]
    public void IsRegisteredForEvent_WithNotRegisteredEvent_ShouldReturnFalse()
    {
        Assert.False(_sessionService.IsRegisteredForEvent(1));
    }

    #endregion

    #region Preferences Tests

    [Fact]
    public void SetPreference_ShouldStorePreference()
    {
        _sessionService.SetPreference("theme", "dark");

        var value = _sessionService.GetPreference("theme");
        Assert.Equal("dark", value);
    }

    [Fact]
    public void SetPreference_WithExistingKey_ShouldOverwrite()
    {
        _sessionService.SetPreference("theme", "dark");
        _sessionService.SetPreference("theme", "light");

        var value = _sessionService.GetPreference("theme");
        Assert.Equal("light", value);
    }

    [Fact]
    public void SetPreference_ShouldUpdateLastActivity()
    {
        var beforeSetting = DateTime.UtcNow;

        _sessionService.SetPreference("theme", "dark");

        Assert.True(_sessionService.CurrentSession.LastActivity >= beforeSetting);
    }

    [Fact]
    public void SetPreference_ShouldTriggerSessionChangedEvent()
    {
        var eventTriggered = false;
        _sessionService.OnSessionChanged += () => eventTriggered = true;

        _sessionService.SetPreference("theme", "dark");

        Assert.True(eventTriggered);
    }

    [Fact]
    public void GetPreference_WithExistingKey_ShouldReturnValue()
    {
        _sessionService.SetPreference("language", "en-US");

        var value = _sessionService.GetPreference("language");

        Assert.Equal("en-US", value);
    }

    [Fact]
    public void GetPreference_WithNonExistingKey_ShouldReturnNull()
    {
        var value = _sessionService.GetPreference("nonexistent");

        Assert.Null(value);
    }

    [Fact]
    public void ClearPreference_ShouldRemovePreference()
    {
        _sessionService.SetPreference("theme", "dark");
        _sessionService.ClearPreference("theme");

        var value = _sessionService.GetPreference("theme");
        Assert.Null(value);
    }

    [Fact]
    public void ClearPreference_WithNonExistingKey_ShouldNotThrow()
    {
        var exception = Record.Exception(() => _sessionService.ClearPreference("nonexistent"));

        Assert.Null(exception);
    }

    [Fact]
    public void ClearPreference_ShouldTriggerSessionChangedEvent()
    {
        _sessionService.SetPreference("theme", "dark");
        var eventTriggered = false;
        _sessionService.OnSessionChanged += () => eventTriggered = true;

        _sessionService.ClearPreference("theme");

        Assert.True(eventTriggered);
    }

    #endregion

    #region Session Duration and Idle Time Tests

    [Fact]
    public async Task GetSessionDuration_ShouldReturnTimeSinceSessionStart()
    {
        await Task.Delay(100);

        var duration = _sessionService.GetSessionDuration();

        Assert.True(duration.TotalMilliseconds >= 50); // Allow some tolerance for timing
    }

    [Fact]
    public async Task GetIdleTime_ShouldReturnTimeSinceLastActivity()
    {
        _sessionService.TrackPageView("Home");
        await Task.Delay(100);

        var idleTime = _sessionService.GetIdleTime();

        Assert.True(idleTime.TotalMilliseconds >= 50); // Allow some tolerance for timing
    }

    [Fact]
    public void GetIdleTime_AfterActivity_ShouldResetToNearZero()
    {
        _sessionService.TrackPageView("Home");

        var idleTime = _sessionService.GetIdleTime();

        Assert.True(idleTime.TotalSeconds < 1);
    }

    #endregion

    #region Authentication Tests

    [Fact]
    public void SetAuthentication_WithTrue_ShouldSetAuthenticatedStatus()
    {
        _sessionService.SetAuthentication(true, "John Doe", "john@example.com");

        Assert.True(_sessionService.CurrentSession.IsAuthenticated);
        Assert.Equal("John Doe", _sessionService.CurrentSession.UserName);
        Assert.Equal("john@example.com", _sessionService.CurrentSession.Email);
    }

    [Fact]
    public void SetAuthentication_WithFalse_ShouldSetUnauthenticatedStatus()
    {
        _sessionService.SetAuthentication(true, "John Doe", "john@example.com");
        _sessionService.SetAuthentication(false);

        Assert.False(_sessionService.CurrentSession.IsAuthenticated);
    }

    [Fact]
    public void SetAuthentication_WithoutUserName_ShouldNotOverwriteExisting()
    {
        _sessionService.RegisterForEvent(1, "Jane", "Smith", "jane@example.com");
        _sessionService.SetAuthentication(true);

        Assert.Equal("Jane Smith", _sessionService.CurrentSession.UserName);
    }

    [Fact]
    public void SetAuthentication_WithUserName_ShouldOverwriteExisting()
    {
        _sessionService.RegisterForEvent(1, "Jane", "Smith", "jane@example.com");
        _sessionService.SetAuthentication(true, "John Doe", "john@example.com");

        Assert.Equal("John Doe", _sessionService.CurrentSession.UserName);
    }

    [Fact]
    public void SetAuthentication_ShouldUpdateLastActivity()
    {
        var beforeAuth = DateTime.UtcNow;

        _sessionService.SetAuthentication(true, "John Doe", "john@example.com");

        Assert.True(_sessionService.CurrentSession.LastActivity >= beforeAuth);
    }

    [Fact]
    public void SetAuthentication_ShouldTriggerSessionChangedEvent()
    {
        var eventTriggered = false;
        _sessionService.OnSessionChanged += () => eventTriggered = true;

        _sessionService.SetAuthentication(true, "John Doe", "john@example.com");

        Assert.True(eventTriggered);
    }

    #endregion

    #region Reset Session Tests

    [Fact]
    public void ResetSession_ShouldClearAllSessionData()
    {
        _sessionService.TrackPageView("Home");
        _sessionService.TrackEventView(1);
        _sessionService.RegisterForEvent(1, "John", "Doe", "john@example.com");
        _sessionService.SetPreference("theme", "dark");
        _sessionService.SetAuthentication(true, "John Doe", "john@example.com");

        _sessionService.ResetSession();

        Assert.Null(_sessionService.CurrentSession.UserName);
        Assert.Null(_sessionService.CurrentSession.Email);
        Assert.False(_sessionService.CurrentSession.IsAuthenticated);
        Assert.Empty(_sessionService.CurrentSession.ViewedEventIds);
        Assert.Empty(_sessionService.CurrentSession.RegisteredEventIds);
        Assert.Empty(_sessionService.CurrentSession.Preferences);
        Assert.Equal(0, _sessionService.CurrentSession.PageViewCount);
    }

    [Fact]
    public void ResetSession_ShouldGenerateNewUserId()
    {
        var oldUserId = _sessionService.CurrentSession.UserId;

        _sessionService.ResetSession();

        Assert.NotEqual(oldUserId, _sessionService.CurrentSession.UserId);
    }

    [Fact]
    public void ResetSession_ShouldUpdateLastActivity()
    {
        var beforeReset = DateTime.UtcNow;

        _sessionService.ResetSession();

        Assert.True(_sessionService.CurrentSession.LastActivity >= beforeReset);
    }

    [Fact]
    public void ResetSession_ShouldTriggerSessionChangedEvent()
    {
        var eventTriggered = false;
        _sessionService.OnSessionChanged += () => eventTriggered = true;

        _sessionService.ResetSession();

        Assert.True(eventTriggered);
    }

    #endregion

    #region Statistics Tests

    [Fact]
    public void GetStatistics_ShouldReturnCorrectSessionDuration()
    {
        var stats = _sessionService.GetStatistics();

        Assert.True(stats.SessionDuration.TotalSeconds >= 0);
    }

    [Fact]
    public void GetStatistics_ShouldReturnCorrectIdleTime()
    {
        var stats = _sessionService.GetStatistics();

        Assert.True(stats.IdleTime.TotalSeconds >= 0);
    }

    [Fact]
    public void GetStatistics_ShouldReturnCorrectPageViewCount()
    {
        _sessionService.TrackPageView("Home");
        _sessionService.TrackPageView("Events");
        _sessionService.TrackPageView("About");

        var stats = _sessionService.GetStatistics();

        Assert.Equal(3, stats.TotalPageViews);
    }

    [Fact]
    public void GetStatistics_ShouldReturnCorrectUniqueEventsViewed()
    {
        _sessionService.TrackEventView(1);
        _sessionService.TrackEventView(2);
        _sessionService.TrackEventView(3);

        var stats = _sessionService.GetStatistics();

        Assert.Equal(3, stats.UniqueEventsViewed);
    }

    [Fact]
    public void GetStatistics_ShouldReturnCorrectTotalRegistrations()
    {
        _sessionService.RegisterForEvent(1, "John", "Doe", "john@example.com");
        _sessionService.RegisterForEvent(2, "John", "Doe", "john@example.com");

        var stats = _sessionService.GetStatistics();

        Assert.Equal(2, stats.TotalRegistrations);
    }

    [Fact]
    public void GetStatistics_WithRecentActivity_ShouldShowActive()
    {
        _sessionService.TrackPageView("Home");

        var stats = _sessionService.GetStatistics();

        Assert.True(stats.IsActive);
    }

    [Fact]
    public async Task GetStatistics_WithoutRecentActivity_ShouldShowInactive()
    {
        _sessionService.TrackPageView("Home");
        var session = _sessionService.CurrentSession;
        session.LastActivity = DateTime.UtcNow.AddMinutes(-6);

        var stats = _sessionService.GetStatistics();

        Assert.False(stats.IsActive);
    }

    #endregion

    #region Event Notification Tests

    [Fact]
    public void OnSessionChanged_ShouldTriggerOnMultipleOperations()
    {
        var triggerCount = 0;
        _sessionService.OnSessionChanged += () => triggerCount++;

        _sessionService.TrackPageView("Home");
        _sessionService.TrackEventView(1);
        _sessionService.RegisterForEvent(1, "John", "Doe", "john@example.com");
        _sessionService.SetPreference("theme", "dark");

        Assert.Equal(4, triggerCount);
    }

    [Fact]
    public void OnPageChanged_ShouldTriggerOnlyForPageViews()
    {
        var triggerCount = 0;
        _sessionService.OnPageChanged += _ => triggerCount++;

        _sessionService.TrackPageView("Home");
        _sessionService.TrackEventView(1);
        _sessionService.TrackPageView("Events");
        _sessionService.RegisterForEvent(1, "John", "Doe", "john@example.com");

        Assert.Equal(2, triggerCount);
    }

    #endregion
}
