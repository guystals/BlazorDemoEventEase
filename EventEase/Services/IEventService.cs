using EventEase.Models;

namespace EventEase.Services;

public interface IEventService
{
    Task<List<Event>> GetAllEventsAsync();
    Task<Event?> GetEventByIdAsync(int id);
    Task<List<Event>> GetEventsByCategoryAsync(string category);
    Task<Event> AddEventAsync(Event newEvent);
    Task<bool> UpdateEventAsync(Event updatedEvent);
    Task<bool> DeleteEventAsync(int id);
    void GenerateTestData(int count = 100);
    Task<(List<Event> Events, int TotalCount)> GetPagedEventsAsync(
        int pageNumber = 1,
        int pageSize = 25,
        string? searchTerm = null,
        string? category = null,
        string sortBy = "date");
}
