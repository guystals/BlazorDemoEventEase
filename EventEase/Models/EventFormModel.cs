using System.ComponentModel.DataAnnotations;

namespace EventEase.Models;

public class EventFormModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Event name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Event name must be between 3 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Event date is required")]
    public DateTime Date { get; set; } = DateTime.Now.AddDays(7);

    [Required(ErrorMessage = "Location is required")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Location must be between 3 and 200 characters")]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Capacity is required")]
    [Range(1, 10000, ErrorMessage = "Capacity must be between 1 and 10,000")]
    public int Capacity { get; set; } = 100;

    [Required(ErrorMessage = "Category is required")]
    public string Category { get; set; } = string.Empty;

    public Event ToEvent()
    {
        return new Event
        {
            Id = Id,
            Name = Name,
            Date = Date,
            Location = Location,
            Description = Description,
            Capacity = Capacity,
            Category = Category
        };
    }

    public static EventFormModel FromEvent(Event evt)
    {
        return new EventFormModel
        {
            Id = evt.Id,
            Name = evt.Name,
            Date = evt.Date,
            Location = evt.Location,
            Description = evt.Description,
            Capacity = evt.Capacity,
            Category = evt.Category
        };
    }
}
