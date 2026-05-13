using EventEase.Models;

namespace EventEase.Tests.Models;

public class UserSessionTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var session = new UserSession();

        Assert.NotNull(session.UserId);
        Assert.NotEqual(string.Empty, session.UserId);
        Assert.Null(session.UserName);
        Assert.Null(session.Email);
        Assert.NotEqual(default(DateTime), session.SessionStartTime);
        Assert.NotEqual(default(DateTime), session.LastActivity);
        Assert.NotNull(session.ViewedEventIds);
        Assert.Empty(session.ViewedEventIds);
        Assert.NotNull(session.RegisteredEventIds);
        Assert.Empty(session.RegisteredEventIds);
        Assert.NotNull(session.Preferences);
        Assert.Empty(session.Preferences);
        Assert.Equal(0, session.PageViewCount);
        Assert.Null(session.CurrentPage);
        Assert.False(session.IsAuthenticated);
    }

    [Fact]
    public void UserId_ShouldBeUniqueForEachInstance()
    {
        var session1 = new UserSession();
        var session2 = new UserSession();

        Assert.NotEqual(session1.UserId, session2.UserId);
    }

    [Fact]
    public void UserId_ShouldBeValidGuid()
    {
        var session = new UserSession();

        Assert.True(Guid.TryParse(session.UserId, out _));
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var sessionStart = DateTime.UtcNow;
        var lastActivity = DateTime.UtcNow.AddMinutes(5);

        var session = new UserSession
        {
            UserId = "custom-user-id",
            UserName = "John Doe",
            Email = "john@example.com",
            SessionStartTime = sessionStart,
            LastActivity = lastActivity,
            PageViewCount = 10,
            CurrentPage = "Events",
            IsAuthenticated = true
        };

        Assert.Equal("custom-user-id", session.UserId);
        Assert.Equal("John Doe", session.UserName);
        Assert.Equal("john@example.com", session.Email);
        Assert.Equal(sessionStart, session.SessionStartTime);
        Assert.Equal(lastActivity, session.LastActivity);
        Assert.Equal(10, session.PageViewCount);
        Assert.Equal("Events", session.CurrentPage);
        Assert.True(session.IsAuthenticated);
    }

    [Fact]
    public void ViewedEventIds_ShouldBeModifiable()
    {
        var session = new UserSession();

        session.ViewedEventIds.Add(1);
        session.ViewedEventIds.Add(2);
        session.ViewedEventIds.Add(3);

        Assert.Equal(3, session.ViewedEventIds.Count);
        Assert.Contains(1, session.ViewedEventIds);
        Assert.Contains(2, session.ViewedEventIds);
        Assert.Contains(3, session.ViewedEventIds);
    }

    [Fact]
    public void RegisteredEventIds_ShouldBeModifiable()
    {
        var session = new UserSession();

        session.RegisteredEventIds.Add(10);
        session.RegisteredEventIds.Add(20);

        Assert.Equal(2, session.RegisteredEventIds.Count);
        Assert.Contains(10, session.RegisteredEventIds);
        Assert.Contains(20, session.RegisteredEventIds);
    }

    [Fact]
    public void Preferences_ShouldBeModifiable()
    {
        var session = new UserSession();

        session.Preferences["theme"] = "dark";
        session.Preferences["language"] = "en-US";
        session.Preferences["pageSize"] = "25";

        Assert.Equal(3, session.Preferences.Count);
        Assert.Equal("dark", session.Preferences["theme"]);
        Assert.Equal("en-US", session.Preferences["language"]);
        Assert.Equal("25", session.Preferences["pageSize"]);
    }

    [Fact]
    public void Preferences_ShouldAllowKeyUpdate()
    {
        var session = new UserSession();

        session.Preferences["theme"] = "dark";
        Assert.Equal("dark", session.Preferences["theme"]);

        session.Preferences["theme"] = "light";
        Assert.Equal("light", session.Preferences["theme"]);
    }

    [Fact]
    public void Preferences_ShouldAllowKeyRemoval()
    {
        var session = new UserSession();

        session.Preferences["theme"] = "dark";
        Assert.Single(session.Preferences);

        session.Preferences.Remove("theme");
        Assert.Empty(session.Preferences);
    }

    [Fact]
    public void PageViewCount_ShouldAllowIncrement()
    {
        var session = new UserSession { PageViewCount = 0 };

        session.PageViewCount++;
        Assert.Equal(1, session.PageViewCount);

        session.PageViewCount++;
        Assert.Equal(2, session.PageViewCount);
    }

    [Fact]
    public void OptionalProperties_ShouldAllowNull()
    {
        var session = new UserSession
        {
            UserName = null,
            Email = null,
            CurrentPage = null
        };

        Assert.Null(session.UserName);
        Assert.Null(session.Email);
        Assert.Null(session.CurrentPage);
    }

    [Fact]
    public void OptionalProperties_ShouldAllowEmptyStrings()
    {
        var session = new UserSession
        {
            UserName = "",
            Email = "",
            CurrentPage = ""
        };

        Assert.Equal(string.Empty, session.UserName);
        Assert.Equal(string.Empty, session.Email);
        Assert.Equal(string.Empty, session.CurrentPage);
    }

    [Fact]
    public void IsAuthenticated_ShouldToggleBetweenTrueAndFalse()
    {
        var session = new UserSession { IsAuthenticated = false };
        Assert.False(session.IsAuthenticated);

        session.IsAuthenticated = true;
        Assert.True(session.IsAuthenticated);

        session.IsAuthenticated = false;
        Assert.False(session.IsAuthenticated);
    }

    [Fact]
    public void SessionStartTime_ShouldDefaultToUtcTime()
    {
        var beforeCreation = DateTime.UtcNow;
        var session = new UserSession();
        var afterCreation = DateTime.UtcNow;

        Assert.True(session.SessionStartTime >= beforeCreation);
        Assert.True(session.SessionStartTime <= afterCreation);
    }

    [Fact]
    public void LastActivity_ShouldDefaultToUtcTime()
    {
        var beforeCreation = DateTime.UtcNow;
        var session = new UserSession();
        var afterCreation = DateTime.UtcNow;

        Assert.True(session.LastActivity >= beforeCreation);
        Assert.True(session.LastActivity <= afterCreation);
    }

    [Fact]
    public void Collections_ShouldBeIndependent()
    {
        var session1 = new UserSession();
        var session2 = new UserSession();

        session1.ViewedEventIds.Add(1);
        session1.RegisteredEventIds.Add(10);
        session1.Preferences["theme"] = "dark";

        Assert.Empty(session2.ViewedEventIds);
        Assert.Empty(session2.RegisteredEventIds);
        Assert.Empty(session2.Preferences);
    }
}
