using EventEase.Models;

namespace EventEase.Tests.Models;

public class AttendanceRecordTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var record = new AttendanceRecord();

        Assert.Equal(0, record.Id);
        Assert.Equal(0, record.EventId);
        Assert.Equal(string.Empty, record.AttendeeName);
        Assert.Equal(string.Empty, record.AttendeeEmail);
        Assert.Null(record.PhoneNumber);
        Assert.Null(record.Company);
        Assert.Equal(default(DateTime), record.RegistrationDate);
        Assert.False(record.IsCheckedIn);
        Assert.Null(record.CheckInTime);
        Assert.Null(record.CheckOutTime);
        Assert.Equal(AttendanceStatus.Registered, record.Status);
        Assert.Null(record.Notes);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var registrationDate = DateTime.UtcNow;
        var checkInTime = DateTime.UtcNow.AddHours(1);
        var checkOutTime = DateTime.UtcNow.AddHours(3);

        var record = new AttendanceRecord
        {
            Id = 1,
            EventId = 10,
            AttendeeName = "John Doe",
            AttendeeEmail = "john@example.com",
            PhoneNumber = "123-456-7890",
            Company = "Tech Corp",
            RegistrationDate = registrationDate,
            IsCheckedIn = true,
            CheckInTime = checkInTime,
            CheckOutTime = checkOutTime,
            Status = AttendanceStatus.CheckedOut,
            Notes = "VIP attendee"
        };

        Assert.Equal(1, record.Id);
        Assert.Equal(10, record.EventId);
        Assert.Equal("John Doe", record.AttendeeName);
        Assert.Equal("john@example.com", record.AttendeeEmail);
        Assert.Equal("123-456-7890", record.PhoneNumber);
        Assert.Equal("Tech Corp", record.Company);
        Assert.Equal(registrationDate, record.RegistrationDate);
        Assert.True(record.IsCheckedIn);
        Assert.Equal(checkInTime, record.CheckInTime);
        Assert.Equal(checkOutTime, record.CheckOutTime);
        Assert.Equal(AttendanceStatus.CheckedOut, record.Status);
        Assert.Equal("VIP attendee", record.Notes);
    }

    [Fact]
    public void OptionalProperties_ShouldAllowNull()
    {
        var record = new AttendanceRecord
        {
            PhoneNumber = null,
            Company = null,
            CheckInTime = null,
            CheckOutTime = null,
            Notes = null
        };

        Assert.Null(record.PhoneNumber);
        Assert.Null(record.Company);
        Assert.Null(record.CheckInTime);
        Assert.Null(record.CheckOutTime);
        Assert.Null(record.Notes);
    }

    [Fact]
    public void OptionalProperties_ShouldAllowEmptyStrings()
    {
        var record = new AttendanceRecord
        {
            PhoneNumber = "",
            Company = "",
            Notes = ""
        };

        Assert.Equal(string.Empty, record.PhoneNumber);
        Assert.Equal(string.Empty, record.Company);
        Assert.Equal(string.Empty, record.Notes);
    }

    [Fact]
    public void Status_ShouldAcceptAllEnumValues()
    {
        var record = new AttendanceRecord();

        record.Status = AttendanceStatus.Registered;
        Assert.Equal(AttendanceStatus.Registered, record.Status);

        record.Status = AttendanceStatus.CheckedIn;
        Assert.Equal(AttendanceStatus.CheckedIn, record.Status);

        record.Status = AttendanceStatus.CheckedOut;
        Assert.Equal(AttendanceStatus.CheckedOut, record.Status);

        record.Status = AttendanceStatus.NoShow;
        Assert.Equal(AttendanceStatus.NoShow, record.Status);

        record.Status = AttendanceStatus.Cancelled;
        Assert.Equal(AttendanceStatus.Cancelled, record.Status);
    }

    [Fact]
    public void IsCheckedIn_ShouldToggleBetweenTrueAndFalse()
    {
        var record = new AttendanceRecord { IsCheckedIn = false };
        Assert.False(record.IsCheckedIn);

        record.IsCheckedIn = true;
        Assert.True(record.IsCheckedIn);

        record.IsCheckedIn = false;
        Assert.False(record.IsCheckedIn);
    }

    [Fact]
    public void CheckInAndCheckOutTimes_ShouldAllowFutureAndPastDates()
    {
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var futureDate = DateTime.UtcNow.AddDays(1);

        var record = new AttendanceRecord
        {
            CheckInTime = pastDate,
            CheckOutTime = futureDate
        };

        Assert.Equal(pastDate, record.CheckInTime);
        Assert.Equal(futureDate, record.CheckOutTime);
    }
}

public class AttendanceStatusTests
{
    [Fact]
    public void AttendanceStatus_ShouldHaveCorrectValues()
    {
        Assert.Equal(0, (int)AttendanceStatus.Registered);
        Assert.Equal(1, (int)AttendanceStatus.CheckedIn);
        Assert.Equal(2, (int)AttendanceStatus.CheckedOut);
        Assert.Equal(3, (int)AttendanceStatus.NoShow);
        Assert.Equal(4, (int)AttendanceStatus.Cancelled);
    }

    [Fact]
    public void AttendanceStatus_ShouldHaveCorrectNames()
    {
        Assert.Equal("Registered", AttendanceStatus.Registered.ToString());
        Assert.Equal("CheckedIn", AttendanceStatus.CheckedIn.ToString());
        Assert.Equal("CheckedOut", AttendanceStatus.CheckedOut.ToString());
        Assert.Equal("NoShow", AttendanceStatus.NoShow.ToString());
        Assert.Equal("Cancelled", AttendanceStatus.Cancelled.ToString());
    }
}
