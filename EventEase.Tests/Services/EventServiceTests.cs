using EventEase.Models;
using EventEase.Repositories;
using EventEase.Services;
using Moq;

namespace EventEase.Tests.Services;

public class EventServiceTests
{
    private readonly Mock<IEventRepository> _mockRepository;
    private readonly EventService _eventService;

    public EventServiceTests()
    {
        _mockRepository = new Mock<IEventRepository>();
        _eventService = new EventService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllEventsAsync_ShouldReturnAllEvents()
    {
        var expectedEvents = new List<Event>
        {
            new Event { Id = 1, Name = "Event 1", Date = DateTime.Now, Location = "Location 1", Description = "Description 1", Capacity = 100, Category = "Conference" },
            new Event { Id = 2, Name = "Event 2", Date = DateTime.Now.AddDays(1), Location = "Location 2", Description = "Description 2", Capacity = 50, Category = "Workshop" }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedEvents);

        var result = await _eventService.GetAllEventsAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(expectedEvents, result);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetEventByIdAsync_WithValidId_ShouldReturnEvent()
    {
        var expectedEvent = new Event { Id = 1, Name = "Event 1", Date = DateTime.Now, Location = "Location 1", Description = "Description 1", Capacity = 100, Category = "Conference" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedEvent);

        var result = await _eventService.GetEventByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(expectedEvent.Id, result.Id);
        Assert.Equal(expectedEvent.Name, result.Name);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetEventByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Event?)null);

        var result = await _eventService.GetEventByIdAsync(999);

        Assert.Null(result);
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task GetEventsByCategoryAsync_ShouldReturnFilteredEvents()
    {
        var conferenceEvents = new List<Event>
        {
            new Event { Id = 1, Name = "Event 1", Date = DateTime.Now, Location = "Location 1", Description = "Description 1", Capacity = 100, Category = "Conference" },
            new Event { Id = 2, Name = "Event 2", Date = DateTime.Now.AddDays(1), Location = "Location 2", Description = "Description 2", Capacity = 150, Category = "Conference" }
        };
        _mockRepository.Setup(r => r.GetByCategoryAsync("Conference")).ReturnsAsync(conferenceEvents);

        var result = await _eventService.GetEventsByCategoryAsync("Conference");

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.Equal("Conference", e.Category));
        _mockRepository.Verify(r => r.GetByCategoryAsync("Conference"), Times.Once);
    }

    [Fact]
    public async Task GetEventsByCategoryAsync_WithNoMatchingCategory_ShouldReturnEmptyList()
    {
        _mockRepository.Setup(r => r.GetByCategoryAsync("NonExistent")).ReturnsAsync(new List<Event>());

        var result = await _eventService.GetEventsByCategoryAsync("NonExistent");

        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetByCategoryAsync("NonExistent"), Times.Once);
    }

    [Fact]
    public async Task AddEventAsync_ShouldReturnAddedEvent()
    {
        var newEvent = new Event { Name = "New Event", Date = DateTime.Now.AddDays(5), Location = "New Location", Description = "New Description", Capacity = 200, Category = "Seminar" };
        var addedEvent = new Event { Id = 3, Name = "New Event", Date = DateTime.Now.AddDays(5), Location = "New Location", Description = "New Description", Capacity = 200, Category = "Seminar" };
        _mockRepository.Setup(r => r.AddAsync(newEvent)).ReturnsAsync(addedEvent);

        var result = await _eventService.AddEventAsync(newEvent);

        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal(newEvent.Name, result.Name);
        _mockRepository.Verify(r => r.AddAsync(newEvent), Times.Once);
    }

    [Fact]
    public async Task UpdateEventAsync_WithValidEvent_ShouldReturnTrue()
    {
        var updatedEvent = new Event { Id = 1, Name = "Updated Event", Date = DateTime.Now, Location = "Updated Location", Description = "Updated Description", Capacity = 120, Category = "Conference" };
        _mockRepository.Setup(r => r.UpdateAsync(updatedEvent)).ReturnsAsync(true);

        var result = await _eventService.UpdateEventAsync(updatedEvent);

        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateAsync(updatedEvent), Times.Once);
    }

    [Fact]
    public async Task UpdateEventAsync_WithInvalidEvent_ShouldReturnFalse()
    {
        var invalidEvent = new Event { Id = 999, Name = "Non-existent Event", Date = DateTime.Now, Location = "Location", Description = "Description", Capacity = 100, Category = "Conference" };
        _mockRepository.Setup(r => r.UpdateAsync(invalidEvent)).ReturnsAsync(false);

        var result = await _eventService.UpdateEventAsync(invalidEvent);

        Assert.False(result);
        _mockRepository.Verify(r => r.UpdateAsync(invalidEvent), Times.Once);
    }

    [Fact]
    public async Task DeleteEventAsync_WithValidId_ShouldReturnTrue()
    {
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _eventService.DeleteEventAsync(1);

        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteEventAsync_WithInvalidId_ShouldReturnFalse()
    {
        _mockRepository.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

        var result = await _eventService.DeleteEventAsync(999);

        Assert.False(result);
        _mockRepository.Verify(r => r.DeleteAsync(999), Times.Once);
    }

    [Fact]
    public void GenerateTestData_ShouldCallRepositoryMethod()
    {
        _eventService.GenerateTestData(50);

        _mockRepository.Verify(r => r.GenerateTestData(50), Times.Once);
    }

    [Fact]
    public void GenerateTestData_WithDefaultCount_ShouldCallRepositoryMethodWithDefault()
    {
        _eventService.GenerateTestData();

        _mockRepository.Verify(r => r.GenerateTestData(100), Times.Once);
    }

    [Fact]
    public async Task GetPagedEventsAsync_WithDefaultParameters_ShouldReturnPagedResults()
    {
        var events = new List<Event>
        {
            new Event { Id = 1, Name = "Event 1", Date = DateTime.Now, Location = "Location 1", Description = "Description 1", Capacity = 100, Category = "Conference" },
            new Event { Id = 2, Name = "Event 2", Date = DateTime.Now.AddDays(1), Location = "Location 2", Description = "Description 2", Capacity = 50, Category = "Workshop" }
        };
        var expectedResult = (events, 100);
        _mockRepository.Setup(r => r.GetPagedAsync(1, 25, null, null, "date")).ReturnsAsync(expectedResult);

        var result = await _eventService.GetPagedEventsAsync();

        Assert.NotNull(result.Events);
        Assert.Equal(2, result.Events.Count);
        Assert.Equal(100, result.TotalCount);
        _mockRepository.Verify(r => r.GetPagedAsync(1, 25, null, null, "date"), Times.Once);
    }

    [Fact]
    public async Task GetPagedEventsAsync_WithCustomParameters_ShouldReturnFilteredPagedResults()
    {
        var events = new List<Event>
        {
            new Event { Id = 1, Name = "Conference Event", Date = DateTime.Now, Location = "Location 1", Description = "Description 1", Capacity = 100, Category = "Conference" }
        };
        var expectedResult = (events, 1);
        _mockRepository.Setup(r => r.GetPagedAsync(2, 10, "Conference", "Conference", "name")).ReturnsAsync(expectedResult);

        var result = await _eventService.GetPagedEventsAsync(2, 10, "Conference", "Conference", "name");

        Assert.NotNull(result.Events);
        Assert.Single(result.Events);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Conference Event", result.Events[0].Name);
        _mockRepository.Verify(r => r.GetPagedAsync(2, 10, "Conference", "Conference", "name"), Times.Once);
    }

    [Fact]
    public async Task GetPagedEventsAsync_WithSearchTerm_ShouldReturnSearchResults()
    {
        var events = new List<Event>
        {
            new Event { Id = 1, Name = "Tech Conference", Date = DateTime.Now, Location = "Location 1", Description = "Description 1", Capacity = 100, Category = "Conference" }
        };
        var expectedResult = (events, 1);
        _mockRepository.Setup(r => r.GetPagedAsync(1, 25, "Tech", null, "date")).ReturnsAsync(expectedResult);

        var result = await _eventService.GetPagedEventsAsync(searchTerm: "Tech");

        Assert.NotNull(result.Events);
        Assert.Single(result.Events);
        Assert.Contains("Tech", result.Events[0].Name);
        _mockRepository.Verify(r => r.GetPagedAsync(1, 25, "Tech", null, "date"), Times.Once);
    }

    [Fact]
    public async Task GetPagedEventsAsync_WithNoResults_ShouldReturnEmptyList()
    {
        var expectedResult = (new List<Event>(), 0);
        _mockRepository.Setup(r => r.GetPagedAsync(1, 25, null, null, "date")).ReturnsAsync(expectedResult);

        var result = await _eventService.GetPagedEventsAsync();

        Assert.NotNull(result.Events);
        Assert.Empty(result.Events);
        Assert.Equal(0, result.TotalCount);
        _mockRepository.Verify(r => r.GetPagedAsync(1, 25, null, null, "date"), Times.Once);
    }

    [Fact]
    public async Task AddEventAsync_WithNullEvent_ShouldThrowArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _eventService.AddEventAsync(null!));
    }

    [Fact]
    public async Task UpdateEventAsync_WithNullEvent_ShouldThrowArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _eventService.UpdateEventAsync(null!));
    }

    [Fact]
    public async Task GetEventsByCategoryAsync_WithNullCategory_ShouldCallRepository()
    {
        var events = new List<Event>();
        _mockRepository.Setup(r => r.GetByCategoryAsync(null!)).ReturnsAsync(events);

        var result = await _eventService.GetEventsByCategoryAsync(null!);

        Assert.NotNull(result);
        _mockRepository.Verify(r => r.GetByCategoryAsync(null!), Times.Once);
    }

    [Fact]
    public async Task GetPagedEventsAsync_WithNullSearchTerm_ShouldWork()
    {
        var events = new List<Event>
        {
            new Event { Id = 1, Name = "Event 1", Date = DateTime.Now, Location = "Location 1", Description = "Description 1", Capacity = 100, Category = "Conference" }
        };
        var expectedResult = (events, 1);
        _mockRepository.Setup(r => r.GetPagedAsync(1, 25, null, null, "date")).ReturnsAsync(expectedResult);

        var result = await _eventService.GetPagedEventsAsync(searchTerm: null);

        Assert.NotNull(result.Events);
        Assert.Single(result.Events);
        _mockRepository.Verify(r => r.GetPagedAsync(1, 25, null, null, "date"), Times.Once);
    }

    [Fact]
    public async Task GetPagedEventsAsync_WithNullCategory_ShouldWork()
    {
        var events = new List<Event>
        {
            new Event { Id = 1, Name = "Event 1", Date = DateTime.Now, Location = "Location 1", Description = "Description 1", Capacity = 100, Category = "Conference" }
        };
        var expectedResult = (events, 1);
        _mockRepository.Setup(r => r.GetPagedAsync(1, 25, null, null, "date")).ReturnsAsync(expectedResult);

        var result = await _eventService.GetPagedEventsAsync(category: null);

        Assert.NotNull(result.Events);
        Assert.Single(result.Events);
        _mockRepository.Verify(r => r.GetPagedAsync(1, 25, null, null, "date"), Times.Once);
    }

    [Fact]
    public async Task GetPagedEventsAsync_WithNullSortBy_ShouldUseDefault()
    {
        var events = new List<Event>
        {
            new Event { Id = 1, Name = "Event 1", Date = DateTime.Now, Location = "Location 1", Description = "Description 1", Capacity = 100, Category = "Conference" }
        };
        var expectedResult = (events, 1);
        _mockRepository.Setup(r => r.GetPagedAsync(1, 25, null, null, null!)).ReturnsAsync(expectedResult);

        var result = await _eventService.GetPagedEventsAsync(sortBy: null!);

        Assert.NotNull(result.Events);
        Assert.Single(result.Events);
        _mockRepository.Verify(r => r.GetPagedAsync(1, 25, null, null, null!), Times.Once);
    }
}
