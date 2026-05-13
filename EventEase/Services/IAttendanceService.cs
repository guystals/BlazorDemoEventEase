using EventEase.Models;

namespace EventEase.Services;

public interface IAttendanceService
{
    // Basic CRUD operations
    Task<AttendanceRecord> AddAttendeeAsync(AttendanceRecord record);
    Task<AttendanceRecord?> GetAttendeeAsync(int attendeeId);
    Task<bool> UpdateAttendeeAsync(AttendanceRecord record);
    Task<bool> DeleteAttendeeAsync(int attendeeId);

    // Event-specific queries
    Task<List<AttendanceRecord>> GetEventAttendeesAsync(int eventId);
    Task<List<AttendanceRecord>> GetCheckedInAttendeesAsync(int eventId);
    Task<List<AttendanceRecord>> GetAttendeesByStatusAsync(int eventId, AttendanceStatus status);

    // Check-in/Check-out operations
    Task<bool> CheckInAttendeeAsync(int attendeeId);
    Task<bool> CheckOutAttendeeAsync(int attendeeId);
    Task<bool> BulkCheckInAsync(List<int> attendeeIds);

    // Search and filter
    Task<List<AttendanceRecord>> SearchAttendeesAsync(int eventId, string searchTerm);
    Task<AttendanceRecord?> FindAttendeeByEmailAsync(int eventId, string email);

    // Statistics
    Task<AttendanceStatistics> GetEventStatisticsAsync(int eventId);
    Task<int> GetTotalAttendeesCountAsync(int eventId);
    Task<int> GetCheckedInCountAsync(int eventId);
    Task<double> GetAttendanceRateAsync(int eventId);

    // Status management
    Task<bool> UpdateAttendeeStatusAsync(int attendeeId, AttendanceStatus status);
    Task<bool> MarkAsNoShowAsync(int attendeeId);
    Task<bool> CancelAttendanceAsync(int attendeeId);

    // Auto-registration from event registrations
    Task<AttendanceRecord> RegisterAttendeeFromEventAsync(int eventId, string firstName, string lastName, string email, string? phone = null, string? company = null);

    // Export/reporting
    Task<List<AttendanceRecord>> GetAllAttendanceRecordsAsync();
}

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
