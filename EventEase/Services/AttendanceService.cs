using EventEase.Models;

namespace EventEase.Services;

public class AttendanceService : IAttendanceService
{
    private readonly List<AttendanceRecord> _attendanceRecords;
    private int _nextId = 1;

    public AttendanceService()
    {
        _attendanceRecords = new List<AttendanceRecord>();
    }

    #region Basic CRUD Operations

    public Task<AttendanceRecord> AddAttendeeAsync(AttendanceRecord record)
    {
        record.Id = _nextId++;
        record.RegistrationDate = DateTime.UtcNow;
        record.Status = AttendanceStatus.Registered;
        _attendanceRecords.Add(record);
        return Task.FromResult(record);
    }

    public Task<AttendanceRecord?> GetAttendeeAsync(int attendeeId)
    {
        var attendee = _attendanceRecords.FirstOrDefault(a => a.Id == attendeeId);
        return Task.FromResult(attendee);
    }

    public Task<bool> UpdateAttendeeAsync(AttendanceRecord record)
    {
        var existing = _attendanceRecords.FirstOrDefault(a => a.Id == record.Id);
        if (existing == null)
            return Task.FromResult(false);

        existing.AttendeeName = record.AttendeeName;
        existing.AttendeeEmail = record.AttendeeEmail;
        existing.PhoneNumber = record.PhoneNumber;
        existing.Company = record.Company;
        existing.Notes = record.Notes;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAttendeeAsync(int attendeeId)
    {
        var attendee = _attendanceRecords.FirstOrDefault(a => a.Id == attendeeId);
        if (attendee == null)
            return Task.FromResult(false);

        _attendanceRecords.Remove(attendee);
        return Task.FromResult(true);
    }

    #endregion

    #region Event-specific Queries

    public Task<List<AttendanceRecord>> GetEventAttendeesAsync(int eventId)
    {
        var attendees = _attendanceRecords
            .Where(a => a.EventId == eventId)
            .OrderBy(a => a.AttendeeName)
            .ToList();
        return Task.FromResult(attendees);
    }

    public Task<List<AttendanceRecord>> GetCheckedInAttendeesAsync(int eventId)
    {
        var attendees = _attendanceRecords
            .Where(a => a.EventId == eventId && a.IsCheckedIn)
            .OrderByDescending(a => a.CheckInTime)
            .ToList();
        return Task.FromResult(attendees);
    }

    public Task<List<AttendanceRecord>> GetAttendeesByStatusAsync(int eventId, AttendanceStatus status)
    {
        var attendees = _attendanceRecords
            .Where(a => a.EventId == eventId && a.Status == status)
            .OrderBy(a => a.AttendeeName)
            .ToList();
        return Task.FromResult(attendees);
    }

    #endregion

    #region Check-in/Check-out Operations

    public Task<bool> CheckInAttendeeAsync(int attendeeId)
    {
        var attendee = _attendanceRecords.FirstOrDefault(a => a.Id == attendeeId);
        if (attendee == null)
            return Task.FromResult(false);

        attendee.IsCheckedIn = true;
        attendee.CheckInTime = DateTime.UtcNow;
        attendee.Status = AttendanceStatus.CheckedIn;

        return Task.FromResult(true);
    }

    public Task<bool> CheckOutAttendeeAsync(int attendeeId)
    {
        var attendee = _attendanceRecords.FirstOrDefault(a => a.Id == attendeeId);
        if (attendee == null || !attendee.IsCheckedIn)
            return Task.FromResult(false);

        attendee.IsCheckedIn = false;
        attendee.CheckOutTime = DateTime.UtcNow;
        attendee.Status = AttendanceStatus.CheckedOut;

        return Task.FromResult(true);
    }

    public Task<bool> BulkCheckInAsync(List<int> attendeeIds)
    {
        foreach (var id in attendeeIds)
        {
            var attendee = _attendanceRecords.FirstOrDefault(a => a.Id == id);
            if (attendee != null && !attendee.IsCheckedIn)
            {
                attendee.IsCheckedIn = true;
                attendee.CheckInTime = DateTime.UtcNow;
                attendee.Status = AttendanceStatus.CheckedIn;
            }
        }
        return Task.FromResult(true);
    }

    #endregion

    #region Search and Filter

    public Task<List<AttendanceRecord>> SearchAttendeesAsync(int eventId, string searchTerm)
    {
        var results = _attendanceRecords
            .Where(a => a.EventId == eventId &&
                       (a.AttendeeName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        a.AttendeeEmail.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (a.Company != null && a.Company.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))))
            .OrderBy(a => a.AttendeeName)
            .ToList();
        return Task.FromResult(results);
    }

    public Task<AttendanceRecord?> FindAttendeeByEmailAsync(int eventId, string email)
    {
        var attendee = _attendanceRecords
            .FirstOrDefault(a => a.EventId == eventId && 
                               a.AttendeeEmail.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(attendee);
    }

    #endregion

    #region Statistics

    public Task<AttendanceStatistics> GetEventStatisticsAsync(int eventId)
    {
        var attendees = _attendanceRecords.Where(a => a.EventId == eventId).ToList();

        var stats = new AttendanceStatistics
        {
            TotalRegistered = attendees.Count(a => a.Status == AttendanceStatus.Registered || 
                                                   a.Status == AttendanceStatus.CheckedIn || 
                                                   a.Status == AttendanceStatus.CheckedOut),
            CheckedIn = attendees.Count(a => a.Status == AttendanceStatus.CheckedIn),
            CheckedOut = attendees.Count(a => a.Status == AttendanceStatus.CheckedOut),
            NoShows = attendees.Count(a => a.Status == AttendanceStatus.NoShow),
            Cancelled = attendees.Count(a => a.Status == AttendanceStatus.Cancelled)
        };

        var totalPotentialAttendees = stats.TotalRegistered + stats.NoShows;
        stats.AttendanceRate = totalPotentialAttendees > 0 
            ? (double)(stats.CheckedIn + stats.CheckedOut) / totalPotentialAttendees * 100 
            : 0;

        var checkedInAttendees = attendees.Where(a => a.CheckInTime.HasValue).ToList();
        if (checkedInAttendees.Any())
        {
            stats.FirstCheckIn = checkedInAttendees.Min(a => a.CheckInTime);
            stats.LastCheckIn = checkedInAttendees.Max(a => a.CheckInTime);
        }

        var completedStays = attendees
            .Where(a => a.CheckInTime.HasValue && a.CheckOutTime.HasValue)
            .ToList();

        if (completedStays.Any())
        {
            var avgTicks = completedStays
                .Average(a => (a.CheckOutTime!.Value - a.CheckInTime!.Value).Ticks);
            stats.AverageStayDuration = TimeSpan.FromTicks((long)avgTicks);
        }

        return Task.FromResult(stats);
    }

    public Task<int> GetTotalAttendeesCountAsync(int eventId)
    {
        var count = _attendanceRecords.Count(a => a.EventId == eventId);
        return Task.FromResult(count);
    }

    public Task<int> GetCheckedInCountAsync(int eventId)
    {
        var count = _attendanceRecords.Count(a => a.EventId == eventId && a.IsCheckedIn);
        return Task.FromResult(count);
    }

    public Task<double> GetAttendanceRateAsync(int eventId)
    {
        var total = _attendanceRecords.Count(a => a.EventId == eventId && 
            a.Status != AttendanceStatus.Cancelled);

        if (total == 0)
            return Task.FromResult(0.0);

        var attended = _attendanceRecords.Count(a => a.EventId == eventId && 
            (a.Status == AttendanceStatus.CheckedIn || a.Status == AttendanceStatus.CheckedOut));

        var rate = (double)attended / total * 100;
        return Task.FromResult(rate);
    }

    #endregion

    #region Status Management

    public Task<bool> UpdateAttendeeStatusAsync(int attendeeId, AttendanceStatus status)
    {
        var attendee = _attendanceRecords.FirstOrDefault(a => a.Id == attendeeId);
        if (attendee == null)
            return Task.FromResult(false);

        attendee.Status = status;
        return Task.FromResult(true);
    }

    public Task<bool> MarkAsNoShowAsync(int attendeeId)
    {
        var attendee = _attendanceRecords.FirstOrDefault(a => a.Id == attendeeId);
        if (attendee == null)
            return Task.FromResult(false);

        attendee.Status = AttendanceStatus.NoShow;
        attendee.IsCheckedIn = false;
        return Task.FromResult(true);
    }

    public Task<bool> CancelAttendanceAsync(int attendeeId)
    {
        var attendee = _attendanceRecords.FirstOrDefault(a => a.Id == attendeeId);
        if (attendee == null)
            return Task.FromResult(false);

        attendee.Status = AttendanceStatus.Cancelled;
        attendee.IsCheckedIn = false;
        return Task.FromResult(true);
    }

    #endregion

    #region Auto-registration

    public Task<AttendanceRecord> RegisterAttendeeFromEventAsync(
        int eventId, 
        string firstName, 
        string lastName, 
        string email, 
        string? phone = null, 
        string? company = null)
    {
        // Check if already registered
        var existing = _attendanceRecords.FirstOrDefault(a => 
            a.EventId == eventId && a.AttendeeEmail.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
            return Task.FromResult(existing);

        var record = new AttendanceRecord
        {
            EventId = eventId,
            AttendeeName = $"{firstName} {lastName}",
            AttendeeEmail = email,
            PhoneNumber = phone,
            Company = company,
            RegistrationDate = DateTime.UtcNow,
            Status = AttendanceStatus.Registered
        };

        return AddAttendeeAsync(record);
    }

    #endregion

    #region Export/Reporting

    public Task<List<AttendanceRecord>> GetAllAttendanceRecordsAsync()
    {
        return Task.FromResult(_attendanceRecords.ToList());
    }

    #endregion
}
