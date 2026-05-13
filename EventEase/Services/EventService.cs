using EventEase.Models;
using EventEase.Repositories;

namespace EventEase.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _repository;

    public EventService(IEventRepository repository)
    {
        _repository = repository;
    }

    public Task<List<Event>> GetAllEventsAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<Event?> GetEventByIdAsync(int id)
    {
        return _repository.GetByIdAsync(id);
    }

    public Task<List<Event>> GetEventsByCategoryAsync(string category)
    {
        return _repository.GetByCategoryAsync(category);
    }

    public Task<Event> AddEventAsync(Event newEvent)
    {
        ArgumentNullException.ThrowIfNull(newEvent);
        return _repository.AddAsync(newEvent);
    }

    public Task<bool> UpdateEventAsync(Event updatedEvent)
    {
        ArgumentNullException.ThrowIfNull(updatedEvent);
        return _repository.UpdateAsync(updatedEvent);
    }

    public Task<bool> DeleteEventAsync(int id)
    {
        return _repository.DeleteAsync(id);
    }

    public void GenerateTestData(int count = 100)
    {
        _repository.GenerateTestData(count);
    }

    public Task<(List<Event> Events, int TotalCount)> GetPagedEventsAsync(
        int pageNumber = 1,
        int pageSize = 25,
        string? searchTerm = null,
        string? category = null,
        string sortBy = "date")
    {
        return _repository.GetPagedAsync(pageNumber, pageSize, searchTerm, category, sortBy);
    }
}
