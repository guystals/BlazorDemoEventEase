namespace EventEase.Models;

public class UserSession
{
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public DateTime SessionStartTime { get; set; } = DateTime.UtcNow;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public List<int> ViewedEventIds { get; set; } = new();
    public List<int> RegisteredEventIds { get; set; } = new();
    public Dictionary<string, string> Preferences { get; set; } = new();
    public int PageViewCount { get; set; } = 0;
    public string? CurrentPage { get; set; }
    public bool IsAuthenticated { get; set; } = false;
}
