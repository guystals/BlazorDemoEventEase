using EventEase.Models;
using EventEase.Services;

namespace EventEase.Tests.Services;

public class AttendanceServiceTests
{
    private readonly AttendanceService _attendanceService;

    public AttendanceServiceTests()
    {
        _attendanceService = new AttendanceService();
    }

    #region Basic CRUD Operations Tests

    [Fact]
    public async Task AddAttendeeAsync_ShouldAddAttendeeWithGeneratedId()
    {
        var record = new AttendanceRecord
        {
            EventId = 1,
            AttendeeName = "John Doe",
            AttendeeEmail = "john@example.com",
            PhoneNumber = "123-456-7890",
            Company = "Tech Corp"
        };

        var result = await _attendanceService.AddAttendeeAsync(record);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John Doe", result.AttendeeName);
        Assert.Equal("john@example.com", result.AttendeeEmail);
        Assert.Equal(AttendanceStatus.Registered, result.Status);
        Assert.True(result.RegistrationDate <= DateTime.UtcNow);
    }

    [Fact]
    public async Task AddAttendeeAsync_MultipleAttendees_ShouldIncrementIds()
    {
        var record1 = new AttendanceRecord { EventId = 1, AttendeeName = "John Doe", AttendeeEmail = "john@example.com" };
        var record2 = new AttendanceRecord { EventId = 1, AttendeeName = "Jane Smith", AttendeeEmail = "jane@example.com" };

        var result1 = await _attendanceService.AddAttendeeAsync(record1);
        var result2 = await _attendanceService.AddAttendeeAsync(record2);

        Assert.Equal(1, result1.Id);
        Assert.Equal(2, result2.Id);
    }

    [Fact]
    public async Task GetAttendeeAsync_WithValidId_ShouldReturnAttendee()
    {
        var record = new AttendanceRecord { EventId = 1, AttendeeName = "John Doe", AttendeeEmail = "john@example.com" };
        var added = await _attendanceService.AddAttendeeAsync(record);

        var result = await _attendanceService.GetAttendeeAsync(added.Id);

        Assert.NotNull(result);
        Assert.Equal(added.Id, result.Id);
        Assert.Equal("John Doe", result.AttendeeName);
    }

    [Fact]
    public async Task GetAttendeeAsync_WithInvalidId_ShouldReturnNull()
    {
        var result = await _attendanceService.GetAttendeeAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAttendeeAsync_WithValidAttendee_ShouldUpdateAndReturnTrue()
    {
        var record = new AttendanceRecord { EventId = 1, AttendeeName = "John Doe", AttendeeEmail = "john@example.com" };
        var added = await _attendanceService.AddAttendeeAsync(record);

        added.AttendeeName = "John Updated";
        added.AttendeeEmail = "john.updated@example.com";
        added.Company = "New Company";

        var result = await _attendanceService.UpdateAttendeeAsync(added);

        Assert.True(result);
        var updated = await _attendanceService.GetAttendeeAsync(added.Id);
        Assert.Equal("John Updated", updated!.AttendeeName);
        Assert.Equal("john.updated@example.com", updated.AttendeeEmail);
        Assert.Equal("New Company", updated.Company);
    }

    [Fact]
    public async Task UpdateAttendeeAsync_WithInvalidAttendee_ShouldReturnFalse()
    {
        var record = new AttendanceRecord { Id = 999, EventId = 1, AttendeeName = "Non-existent", AttendeeEmail = "none@example.com" };

        var result = await _attendanceService.UpdateAttendeeAsync(record);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAttendeeAsync_WithValidId_ShouldDeleteAndReturnTrue()
    {
        var record = new AttendanceRecord { EventId = 1, AttendeeName = "John Doe", AttendeeEmail = "john@example.com" };
        var added = await _attendanceService.AddAttendeeAsync(record);

        var result = await _attendanceService.DeleteAttendeeAsync(added.Id);

        Assert.True(result);
        var deleted = await _attendanceService.GetAttendeeAsync(added.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteAttendeeAsync_WithInvalidId_ShouldReturnFalse()
    {
        var result = await _attendanceService.DeleteAttendeeAsync(999);

        Assert.False(result);
    }

    #endregion

    #region Event-specific Queries Tests

    [Fact]
    public async Task GetEventAttendeesAsync_ShouldReturnAttendeesForSpecificEvent()
    {
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Alice", AttendeeEmail = "alice@example.com" });
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com" });
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 2, AttendeeName = "Charlie", AttendeeEmail = "charlie@example.com" });

        var result = await _attendanceService.GetEventAttendeesAsync(1);

        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.Equal(1, a.EventId));
    }

    [Fact]
    public async Task GetEventAttendeesAsync_ShouldReturnAttendeesOrderedByName()
    {
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Zoe", AttendeeEmail = "zoe@example.com" });
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Alice", AttendeeEmail = "alice@example.com" });
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com" });

        var result = await _attendanceService.GetEventAttendeesAsync(1);

        Assert.Equal("Alice", result[0].AttendeeName);
        Assert.Equal("Bob", result[1].AttendeeName);
        Assert.Equal("Zoe", result[2].AttendeeName);
    }

    [Fact]
    public async Task GetEventAttendeesAsync_WithNoAttendees_ShouldReturnEmptyList()
    {
        var result = await _attendanceService.GetEventAttendeesAsync(999);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCheckedInAttendeesAsync_ShouldReturnOnlyCheckedInAttendees()
    {
        var attendee1 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Alice", AttendeeEmail = "alice@example.com" });
        var attendee2 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com" });
        var attendee3 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Charlie", AttendeeEmail = "charlie@example.com" });

        await _attendanceService.CheckInAttendeeAsync(attendee1.Id);
        await _attendanceService.CheckInAttendeeAsync(attendee3.Id);

        var result = await _attendanceService.GetCheckedInAttendeesAsync(1);

        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.True(a.IsCheckedIn));
    }

    [Fact]
    public async Task GetAttendeesByStatusAsync_ShouldReturnAttendeesWithSpecificStatus()
    {
        var attendee1 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Alice", AttendeeEmail = "alice@example.com" });
        var attendee2 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com" });
        var attendee3 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Charlie", AttendeeEmail = "charlie@example.com" });

        await _attendanceService.CheckInAttendeeAsync(attendee1.Id);
        await _attendanceService.MarkAsNoShowAsync(attendee3.Id);

        var registered = await _attendanceService.GetAttendeesByStatusAsync(1, AttendanceStatus.Registered);
        var checkedIn = await _attendanceService.GetAttendeesByStatusAsync(1, AttendanceStatus.CheckedIn);
        var noShows = await _attendanceService.GetAttendeesByStatusAsync(1, AttendanceStatus.NoShow);

        Assert.Single(registered);
        Assert.Single(checkedIn);
        Assert.Single(noShows);
    }

    #endregion

    #region Check-in/Check-out Operations Tests

    [Fact]
    public async Task CheckInAttendeeAsync_WithValidId_ShouldCheckInAttendee()
    {
        var attendee = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John Doe", AttendeeEmail = "john@example.com" });

        var result = await _attendanceService.CheckInAttendeeAsync(attendee.Id);

        Assert.True(result);
        var checkedIn = await _attendanceService.GetAttendeeAsync(attendee.Id);
        Assert.True(checkedIn!.IsCheckedIn);
        Assert.NotNull(checkedIn.CheckInTime);
        Assert.Equal(AttendanceStatus.CheckedIn, checkedIn.Status);
    }

    [Fact]
    public async Task CheckInAttendeeAsync_WithInvalidId_ShouldReturnFalse()
    {
        var result = await _attendanceService.CheckInAttendeeAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task CheckOutAttendeeAsync_WithCheckedInAttendee_ShouldCheckOut()
    {
        var attendee = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John Doe", AttendeeEmail = "john@example.com" });
        await _attendanceService.CheckInAttendeeAsync(attendee.Id);

        var result = await _attendanceService.CheckOutAttendeeAsync(attendee.Id);

        Assert.True(result);
        var checkedOut = await _attendanceService.GetAttendeeAsync(attendee.Id);
        Assert.NotNull(checkedOut!.CheckOutTime);
        Assert.Equal(AttendanceStatus.CheckedOut, checkedOut.Status);
    }

    [Fact]
    public async Task CheckOutAttendeeAsync_WithNotCheckedInAttendee_ShouldReturnFalse()
    {
        var attendee = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John Doe", AttendeeEmail = "john@example.com" });

        var result = await _attendanceService.CheckOutAttendeeAsync(attendee.Id);

        Assert.False(result);
    }

    [Fact]
    public async Task CheckOutAttendeeAsync_WithInvalidId_ShouldReturnFalse()
    {
        var result = await _attendanceService.CheckOutAttendeeAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task BulkCheckInAsync_ShouldCheckInMultipleAttendees()
    {
        var attendee1 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Alice", AttendeeEmail = "alice@example.com" });
        var attendee2 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com" });
        var attendee3 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Charlie", AttendeeEmail = "charlie@example.com" });

        var result = await _attendanceService.BulkCheckInAsync(new List<int> { attendee1.Id, attendee2.Id, attendee3.Id });

        Assert.True(result);
        var checkedIn = await _attendanceService.GetCheckedInAttendeesAsync(1);
        Assert.Equal(3, checkedIn.Count);
    }

    [Fact]
    public async Task BulkCheckInAsync_WithSomeInvalidIds_ShouldCheckInOnlyValidOnes()
    {
        var attendee1 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Alice", AttendeeEmail = "alice@example.com" });
        var attendee2 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com" });

        var result = await _attendanceService.BulkCheckInAsync(new List<int> { attendee1.Id, 999, attendee2.Id });

        Assert.True(result);
        var checkedIn = await _attendanceService.GetCheckedInAttendeesAsync(1);
        Assert.Equal(2, checkedIn.Count);
    }

    [Fact]
    public async Task BulkCheckInAsync_WithEmptyList_ShouldReturnTrue()
    {
        var result = await _attendanceService.BulkCheckInAsync(new List<int>());

        Assert.True(result);
    }

    #endregion

    #region Search and Filter Tests

    [Fact]
    public async Task SearchAttendeesAsync_ByName_ShouldReturnMatchingAttendees()
    {
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John Smith", AttendeeEmail = "john@example.com" });
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Jane Doe", AttendeeEmail = "jane@example.com" });
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John Doe", AttendeeEmail = "johndoe@example.com" });

        var result = await _attendanceService.SearchAttendeesAsync(1, "John");

        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.Contains("John", a.AttendeeName));
    }

    [Fact]
    public async Task SearchAttendeesAsync_ByEmail_ShouldReturnMatchingAttendees()
    {
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John Smith", AttendeeEmail = "john.smith@techcorp.com" });
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Jane Doe", AttendeeEmail = "jane@example.com" });
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@techcorp.com" });

        var result = await _attendanceService.SearchAttendeesAsync(1, "techcorp");

        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.Contains("techcorp", a.AttendeeEmail, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchAttendeesAsync_ByCompany_ShouldReturnMatchingAttendees()
    {
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John", AttendeeEmail = "john@example.com", Company = "Tech Corp" });
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Jane", AttendeeEmail = "jane@example.com", Company = "Data Inc" });
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com", Company = "Tech Solutions" });

        var result = await _attendanceService.SearchAttendeesAsync(1, "Tech");

        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.Contains("Tech", a.Company!));
    }

    [Fact]
    public async Task SearchAttendeesAsync_CaseInsensitive_ShouldReturnMatchingAttendees()
    {
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "JOHN SMITH", AttendeeEmail = "john@example.com" });

        var result = await _attendanceService.SearchAttendeesAsync(1, "john smith");

        Assert.Single(result);
    }

    [Fact]
    public async Task SearchAttendeesAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John", AttendeeEmail = "john@example.com" });

        var result = await _attendanceService.SearchAttendeesAsync(1, "NonExistent");

        Assert.Empty(result);
    }

    [Fact]
    public async Task FindAttendeeByEmailAsync_WithExistingEmail_ShouldReturnAttendee()
    {
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John", AttendeeEmail = "john@example.com" });

        var result = await _attendanceService.FindAttendeeByEmailAsync(1, "john@example.com");

        Assert.NotNull(result);
        Assert.Equal("john@example.com", result.AttendeeEmail);
    }

    [Fact]
    public async Task FindAttendeeByEmailAsync_CaseInsensitive_ShouldReturnAttendee()
    {
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John", AttendeeEmail = "john@EXAMPLE.com" });

        var result = await _attendanceService.FindAttendeeByEmailAsync(1, "JOHN@example.COM");

        Assert.NotNull(result);
    }

    [Fact]
    public async Task FindAttendeeByEmailAsync_WithNonExistingEmail_ShouldReturnNull()
    {
        var result = await _attendanceService.FindAttendeeByEmailAsync(1, "nonexistent@example.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task FindAttendeeByEmailAsync_WithDifferentEvent_ShouldReturnNull()
    {
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John", AttendeeEmail = "john@example.com" });

        var result = await _attendanceService.FindAttendeeByEmailAsync(2, "john@example.com");

        Assert.Null(result);
    }

    #endregion

    #region Statistics Tests

    [Fact]
    public async Task GetEventStatisticsAsync_WithNoAttendees_ShouldReturnZeroStats()
    {
        var result = await _attendanceService.GetEventStatisticsAsync(1);

        Assert.Equal(0, result.TotalRegistered);
        Assert.Equal(0, result.CheckedIn);
        Assert.Equal(0, result.CheckedOut);
        Assert.Equal(0, result.NoShows);
        Assert.Equal(0, result.Cancelled);
        Assert.Equal(0, result.AttendanceRate);
    }

    [Fact]
    public async Task GetEventStatisticsAsync_WithMixedStatuses_ShouldCalculateCorrectly()
    {
        var attendee1 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Alice", AttendeeEmail = "alice@example.com" });
        var attendee2 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com" });
        var attendee3 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Charlie", AttendeeEmail = "charlie@example.com" });
        var attendee4 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "David", AttendeeEmail = "david@example.com" });
        var attendee5 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Eve", AttendeeEmail = "eve@example.com" });

        await _attendanceService.CheckInAttendeeAsync(attendee1.Id);
        await _attendanceService.CheckInAttendeeAsync(attendee2.Id);
        await _attendanceService.CheckOutAttendeeAsync(attendee2.Id);
        await _attendanceService.MarkAsNoShowAsync(attendee4.Id);
        await _attendanceService.CancelAttendanceAsync(attendee5.Id);

        var result = await _attendanceService.GetEventStatisticsAsync(1);

        Assert.Equal(3, result.TotalRegistered);
        Assert.Equal(1, result.CheckedIn);
        Assert.Equal(1, result.CheckedOut);
        Assert.Equal(1, result.NoShows);
        Assert.Equal(1, result.Cancelled);
        Assert.Equal(50, result.AttendanceRate);
    }

    [Fact]
    public async Task GetEventStatisticsAsync_ShouldCalculateCheckInTimes()
    {
        var attendee1 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Alice", AttendeeEmail = "alice@example.com" });
        await Task.Delay(10);
        await _attendanceService.CheckInAttendeeAsync(attendee1.Id);
        await Task.Delay(10);
        var attendee2 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com" });
        await _attendanceService.CheckInAttendeeAsync(attendee2.Id);

        var result = await _attendanceService.GetEventStatisticsAsync(1);

        Assert.NotNull(result.FirstCheckIn);
        Assert.NotNull(result.LastCheckIn);
        Assert.True(result.LastCheckIn >= result.FirstCheckIn);
    }

    [Fact]
    public async Task GetEventStatisticsAsync_ShouldCalculateAverageStayDuration()
    {
        var attendee1 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Alice", AttendeeEmail = "alice@example.com" });
        var attendee2 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com" });

        await _attendanceService.CheckInAttendeeAsync(attendee1.Id);
        await _attendanceService.CheckInAttendeeAsync(attendee2.Id);
        await Task.Delay(10);
        await _attendanceService.CheckOutAttendeeAsync(attendee1.Id);
        await _attendanceService.CheckOutAttendeeAsync(attendee2.Id);

        var result = await _attendanceService.GetEventStatisticsAsync(1);

        Assert.NotNull(result.AverageStayDuration);
        Assert.True(result.AverageStayDuration > TimeSpan.Zero);
    }

    [Fact]
    public async Task GetTotalAttendeesCountAsync_ShouldReturnCorrectCount()
    {
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Alice", AttendeeEmail = "alice@example.com" });
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com" });
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 2, AttendeeName = "Charlie", AttendeeEmail = "charlie@example.com" });

        var result = await _attendanceService.GetTotalAttendeesCountAsync(1);

        Assert.Equal(2, result);
    }

    [Fact]
    public async Task GetCheckedInCountAsync_ShouldReturnCorrectCount()
    {
        var attendee1 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Alice", AttendeeEmail = "alice@example.com" });
        var attendee2 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com" });
        var attendee3 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Charlie", AttendeeEmail = "charlie@example.com" });

        await _attendanceService.CheckInAttendeeAsync(attendee1.Id);
        await _attendanceService.CheckInAttendeeAsync(attendee2.Id);

        var result = await _attendanceService.GetCheckedInCountAsync(1);

        Assert.Equal(2, result);
    }

    [Fact]
    public async Task GetAttendanceRateAsync_WithNoAttendees_ShouldReturnZero()
    {
        var result = await _attendanceService.GetAttendanceRateAsync(1);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetAttendanceRateAsync_ShouldCalculateCorrectPercentage()
    {
        var attendee1 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Alice", AttendeeEmail = "alice@example.com" });
        var attendee2 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com" });
        var attendee3 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Charlie", AttendeeEmail = "charlie@example.com" });
        var attendee4 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "David", AttendeeEmail = "david@example.com" });

        await _attendanceService.CheckInAttendeeAsync(attendee1.Id);
        await _attendanceService.CheckInAttendeeAsync(attendee2.Id);
        await _attendanceService.CheckOutAttendeeAsync(attendee2.Id);

        var result = await _attendanceService.GetAttendanceRateAsync(1);

        Assert.Equal(50, result);
    }

    [Fact]
    public async Task GetAttendanceRateAsync_ShouldExcludeCancelledAttendees()
    {
        var attendee1 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Alice", AttendeeEmail = "alice@example.com" });
        var attendee2 = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com" });

        await _attendanceService.CheckInAttendeeAsync(attendee1.Id);
        await _attendanceService.CancelAttendanceAsync(attendee2.Id);

        var result = await _attendanceService.GetAttendanceRateAsync(1);

        Assert.Equal(100, result);
    }

    #endregion

    #region Status Management Tests

    [Fact]
    public async Task UpdateAttendeeStatusAsync_WithValidId_ShouldUpdateStatus()
    {
        var attendee = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John", AttendeeEmail = "john@example.com" });

        var result = await _attendanceService.UpdateAttendeeStatusAsync(attendee.Id, AttendanceStatus.CheckedIn);

        Assert.True(result);
        var updated = await _attendanceService.GetAttendeeAsync(attendee.Id);
        Assert.Equal(AttendanceStatus.CheckedIn, updated!.Status);
    }

    [Fact]
    public async Task UpdateAttendeeStatusAsync_WithInvalidId_ShouldReturnFalse()
    {
        var result = await _attendanceService.UpdateAttendeeStatusAsync(999, AttendanceStatus.CheckedIn);

        Assert.False(result);
    }

    [Fact]
    public async Task MarkAsNoShowAsync_WithValidId_ShouldMarkAsNoShow()
    {
        var attendee = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John", AttendeeEmail = "john@example.com" });

        var result = await _attendanceService.MarkAsNoShowAsync(attendee.Id);

        Assert.True(result);
        var updated = await _attendanceService.GetAttendeeAsync(attendee.Id);
        Assert.Equal(AttendanceStatus.NoShow, updated!.Status);
        Assert.False(updated.IsCheckedIn);
    }

    [Fact]
    public async Task MarkAsNoShowAsync_ShouldUncheckAttendee()
    {
        var attendee = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John", AttendeeEmail = "john@example.com" });
        await _attendanceService.CheckInAttendeeAsync(attendee.Id);

        var result = await _attendanceService.MarkAsNoShowAsync(attendee.Id);

        Assert.True(result);
        var updated = await _attendanceService.GetAttendeeAsync(attendee.Id);
        Assert.False(updated!.IsCheckedIn);
    }

    [Fact]
    public async Task MarkAsNoShowAsync_WithInvalidId_ShouldReturnFalse()
    {
        var result = await _attendanceService.MarkAsNoShowAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task CancelAttendanceAsync_WithValidId_ShouldCancelAttendance()
    {
        var attendee = await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "John", AttendeeEmail = "john@example.com" });

        var result = await _attendanceService.CancelAttendanceAsync(attendee.Id);

        Assert.True(result);
        var updated = await _attendanceService.GetAttendeeAsync(attendee.Id);
        Assert.Equal(AttendanceStatus.Cancelled, updated!.Status);
        Assert.False(updated.IsCheckedIn);
    }

    [Fact]
    public async Task CancelAttendanceAsync_WithInvalidId_ShouldReturnFalse()
    {
        var result = await _attendanceService.CancelAttendanceAsync(999);

        Assert.False(result);
    }

    #endregion

    #region Auto-registration Tests

    [Fact]
    public async Task RegisterAttendeeFromEventAsync_ShouldCreateNewAttendee()
    {
        var result = await _attendanceService.RegisterAttendeeFromEventAsync(1, "John", "Doe", "john@example.com", "123-456-7890", "Tech Corp");

        Assert.NotNull(result);
        Assert.Equal(1, result.EventId);
        Assert.Equal("John Doe", result.AttendeeName);
        Assert.Equal("john@example.com", result.AttendeeEmail);
        Assert.Equal("123-456-7890", result.PhoneNumber);
        Assert.Equal("Tech Corp", result.Company);
        Assert.Equal(AttendanceStatus.Registered, result.Status);
    }

    [Fact]
    public async Task RegisterAttendeeFromEventAsync_WithOptionalParameters_ShouldCreateAttendee()
    {
        var result = await _attendanceService.RegisterAttendeeFromEventAsync(1, "Jane", "Smith", "jane@example.com");

        Assert.NotNull(result);
        Assert.Equal("Jane Smith", result.AttendeeName);
        Assert.Null(result.PhoneNumber);
        Assert.Null(result.Company);
    }

    [Fact]
    public async Task RegisterAttendeeFromEventAsync_WithExistingEmail_ShouldReturnExistingRecord()
    {
        var first = await _attendanceService.RegisterAttendeeFromEventAsync(1, "John", "Doe", "john@example.com");
        var second = await _attendanceService.RegisterAttendeeFromEventAsync(1, "John", "Smith", "john@example.com");

        Assert.Equal(first.Id, second.Id);
        Assert.Equal("John Doe", second.AttendeeName);
    }

    [Fact]
    public async Task RegisterAttendeeFromEventAsync_WithExistingEmailDifferentEvent_ShouldCreateNewRecord()
    {
        var first = await _attendanceService.RegisterAttendeeFromEventAsync(1, "John", "Doe", "john@example.com");
        var second = await _attendanceService.RegisterAttendeeFromEventAsync(2, "John", "Doe", "john@example.com");

        Assert.NotEqual(first.Id, second.Id);
    }

    #endregion

    #region Export/Reporting Tests

    [Fact]
    public async Task GetAllAttendanceRecordsAsync_ShouldReturnAllRecords()
    {
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Alice", AttendeeEmail = "alice@example.com" });
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 1, AttendeeName = "Bob", AttendeeEmail = "bob@example.com" });
        await _attendanceService.AddAttendeeAsync(new AttendanceRecord { EventId = 2, AttendeeName = "Charlie", AttendeeEmail = "charlie@example.com" });

        var result = await _attendanceService.GetAllAttendanceRecordsAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetAllAttendanceRecordsAsync_WithNoRecords_ShouldReturnEmptyList()
    {
        var result = await _attendanceService.GetAllAttendanceRecordsAsync();

        Assert.Empty(result);
    }

    #endregion
}
