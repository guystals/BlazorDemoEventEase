using EventEase.Models;

namespace EventEase.Repositories;

public interface IEventRepository
{
    Task<List<Event>> GetAllAsync();
    Task<Event?> GetByIdAsync(int id);
    Task<List<Event>> GetByCategoryAsync(string category);
    Task<Event> AddAsync(Event newEvent);
    Task<bool> UpdateAsync(Event updatedEvent);
    Task<bool> DeleteAsync(int id);
    void GenerateTestData(int count = 100);
    Task<(List<Event> Events, int TotalCount)> GetPagedAsync(
        int pageNumber = 1,
        int pageSize = 25,
        string? searchTerm = null,
        string? category = null,
        string sortBy = "date");
}
