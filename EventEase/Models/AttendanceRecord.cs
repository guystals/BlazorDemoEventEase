using System.ComponentModel.DataAnnotations;

namespace EventEase.Models;

public class AttendanceRecord
{
    public int Id { get; set; }
    public int EventId { get; set; }

    [Required(ErrorMessage = "Attendee name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Attendee name must be between 2 and 100 characters")]
    public string AttendeeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(254, ErrorMessage = "Email address cannot exceed 254 characters")]
    public string AttendeeEmail { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(30, ErrorMessage = "Phone number cannot exceed 30 characters")]
    public string? PhoneNumber { get; set; }

    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters")]
    public string? Company { get; set; }

    public DateTime RegistrationDate { get; set; }
    public bool IsCheckedIn { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public AttendanceStatus Status { get; set; }

    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? Notes { get; set; }
}

public enum AttendanceStatus
{
    Registered,
    CheckedIn,
    CheckedOut,
    NoShow,
    Cancelled
}
