using EventEase.Models;

namespace EventEase.Repositories;

public class EventRepository : IEventRepository
{
    private readonly List<Event> _events;

    public EventRepository()
    {
        _events = new List<Event>
        {
            new Event
            {
                Id = 1,
                Name = "Annual Tech Summit 2025",
                Date = new DateTime(2025, 6, 15, 9, 0, 0),
                Location = "San Francisco Convention Center, CA",
                Description = "Join us for the biggest technology conference of the year featuring keynote speakers, workshops, and networking opportunities with industry leaders.",
                Capacity = 500,
                Category = "Technology"
            },
            new Event
            {
                Id = 2,
                Name = "Corporate Leadership Workshop",
                Date = new DateTime(2025, 5, 20, 10, 0, 0),
                Location = "Grand Hotel Ballroom, New York, NY",
                Description = "An intensive one-day workshop designed for executives and managers to enhance their leadership skills and strategic thinking.",
                Capacity = 100,
                Category = "Business"
            },
            new Event
            {
                Id = 3,
                Name = "Summer Networking Gala",
                Date = new DateTime(2025, 7, 10, 18, 0, 0),
                Location = "Riverside Gardens, Austin, TX",
                Description = "An elegant evening of networking, dining, and entertainment. Perfect for professionals looking to expand their network in a relaxed atmosphere.",
                Capacity = 300,
                Category = "Social"
            },
            new Event
            {
                Id = 4,
                Name = "Innovation in AI Conference",
                Date = new DateTime(2025, 8, 5, 9, 0, 0),
                Location = "Tech Hub Seattle, WA",
                Description = "Explore the latest developments in artificial intelligence with demos, presentations, and panel discussions from AI pioneers.",
                Capacity = 400,
                Category = "Technology"
            },
            new Event
            {
                Id = 5,
                Name = "Team Building Retreat",
                Date = new DateTime(2025, 9, 12, 8, 0, 0),
                Location = "Mountain View Resort, Colorado",
                Description = "A two-day retreat focused on team bonding, communication skills, and collaborative problem-solving in a beautiful mountain setting.",
                Capacity = 150,
                Category = "Corporate"
            },
            new Event
            {
                Id = 6,
                Name = "Holiday Celebration",
                Date = new DateTime(2025, 12, 15, 19, 0, 0),
                Location = "Downtown Event Center, Chicago, IL",
                Description = "Celebrate the holiday season with colleagues and clients at our annual festive gathering featuring dinner, music, and entertainment.",
                Capacity = 250,
                Category = "Social"
            }
        };
    }

    public Task<List<Event>> GetAllAsync()
    {
        return Task.FromResult(_events.ToList());
    }

    public Task<Event?> GetByIdAsync(int id)
    {
        var evt = _events.FirstOrDefault(e => e.Id == id);
        return Task.FromResult(evt);
    }

    public Task<List<Event>> GetByCategoryAsync(string category)
    {
        var filteredEvents = _events
            .Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .ToList();
        return Task.FromResult(filteredEvents);
    }

    public Task<Event> AddAsync(Event newEvent)
    {
        newEvent.Id = _events.Any() ? _events.Max(e => e.Id) + 1 : 1;
        _events.Add(newEvent);
        return Task.FromResult(newEvent);
    }

    public Task<bool> UpdateAsync(Event updatedEvent)
    {
        var existingEvent = _events.FirstOrDefault(e => e.Id == updatedEvent.Id);
        if (existingEvent == null)
            return Task.FromResult(false);

        existingEvent.Name = updatedEvent.Name;
        existingEvent.Date = updatedEvent.Date;
        existingEvent.Location = updatedEvent.Location;
        existingEvent.Description = updatedEvent.Description;
        existingEvent.Capacity = updatedEvent.Capacity;
        existingEvent.Category = updatedEvent.Category;

        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var eventToRemove = _events.FirstOrDefault(e => e.Id == id);
        if (eventToRemove == null)
            return Task.FromResult(false);

        _events.Remove(eventToRemove);
        return Task.FromResult(true);
    }

    public void GenerateTestData(int count = 100)
    {
        var categories = new[] { "Technology", "Business", "Social", "Corporate", "Education", "Networking" };
        var locations = new[]
        {
            "San Francisco, CA", "New York, NY", "Chicago, IL", "Austin, TX",
            "Seattle, WA", "Boston, MA", "Denver, CO", "Los Angeles, CA",
            "Miami, FL", "Atlanta, GA", "Dallas, TX", "Phoenix, AZ"
        };
        var eventTypes = new[]
        {
            "Conference", "Workshop", "Seminar", "Meetup", "Summit", "Gala",
            "Networking Event", "Training", "Symposium", "Forum", "Retreat", "Celebration"
        };

        var random = new Random();
        var startId = _events.Any() ? _events.Max(e => e.Id) + 1 : 1;

        for (int i = 0; i < count; i++)
        {
            var category = categories[random.Next(categories.Length)];
            var eventType = eventTypes[random.Next(eventTypes.Length)];
            var location = locations[random.Next(locations.Length)];

            _events.Add(new Event
            {
                Id = startId + i,
                Name = $"{category} {eventType} {DateTime.Now.Year + random.Next(0, 2)}",
                Date = DateTime.Now.AddDays(random.Next(1, 365)).AddHours(random.Next(8, 20)),
                Location = location,
                Description = $"Join us for an exciting {eventType.ToLower()} focused on {category.ToLower()}. " +
                             $"This event will feature industry experts, networking opportunities, and hands-on activities. " +
                             $"Perfect for professionals looking to expand their knowledge and connect with peers.",
                Capacity = random.Next(50, 1000),
                Category = category
            });
        }
    }

    public Task<(List<Event> Events, int TotalCount)> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 25,
        string? searchTerm = null,
        string? category = null,
        string sortBy = "date")
    {
        var query = _events.AsEnumerable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(e =>
                e.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.Location.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                e.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(e => e.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        // Apply sorting
        query = sortBy.ToLower() switch
        {
            "name" or "name-asc" => query.OrderBy(e => e.Name),
            "name-desc" => query.OrderByDescending(e => e.Name),
            "date" or "date-asc" => query.OrderBy(e => e.Date),
            "date-desc" => query.OrderByDescending(e => e.Date),
            "capacity" => query.OrderByDescending(e => e.Capacity),
            "capacity-desc" => query.OrderByDescending(e => e.Capacity),
            _ => query.OrderBy(e => e.Date)
        };

        var totalCount = query.Count();
        var events = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult((events, totalCount));
    }
}
