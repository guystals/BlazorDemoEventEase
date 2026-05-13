using EventEase.Models;
using System.ComponentModel.DataAnnotations;

namespace EventEase.Tests.Models;

public class EventFormModelTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var model = new EventFormModel();

        Assert.Equal(0, model.Id);
        Assert.Equal(string.Empty, model.Name);
        Assert.True(model.Date >= DateTime.Now.AddDays(6));
        Assert.True(model.Date <= DateTime.Now.AddDays(8));
        Assert.Equal(string.Empty, model.Location);
        Assert.Equal(string.Empty, model.Description);
        Assert.Equal(100, model.Capacity);
        Assert.Equal(string.Empty, model.Category);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var date = new DateTime(2025, 6, 15);
        var model = new EventFormModel
        {
            Id = 1,
            Name = "Tech Conference",
            Date = date,
            Location = "San Francisco Convention Center",
            Description = "Annual technology conference with keynote speakers",
            Capacity = 500,
            Category = "Technology"
        };

        Assert.Equal(1, model.Id);
        Assert.Equal("Tech Conference", model.Name);
        Assert.Equal(date, model.Date);
        Assert.Equal("San Francisco Convention Center", model.Location);
        Assert.Equal("Annual technology conference with keynote speakers", model.Description);
        Assert.Equal(500, model.Capacity);
        Assert.Equal("Technology", model.Category);
    }

    #region Validation Tests

    [Fact]
    public void Name_WithValidValue_ShouldPassValidation()
    {
        var model = new EventFormModel
        {
            Name = "Tech Conference",
            Date = DateTime.Now.AddDays(7),
            Location = "San Francisco",
            Description = "A great conference",
            Capacity = 100,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains(nameof(model.Name)));
    }

    [Fact]
    public void Name_WithEmptyValue_ShouldFailValidation()
    {
        var model = new EventFormModel
        {
            Name = "",
            Date = DateTime.Now.AddDays(7),
            Location = "San Francisco",
            Description = "A great conference",
            Capacity = 100,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Name)));
    }

    [Fact]
    public void Name_WithTooShortValue_ShouldFailValidation()
    {
        var model = new EventFormModel
        {
            Name = "AB",
            Date = DateTime.Now.AddDays(7),
            Location = "San Francisco",
            Description = "A great conference",
            Capacity = 100,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Name)));
    }

    [Fact]
    public void Name_WithTooLongValue_ShouldFailValidation()
    {
        var model = new EventFormModel
        {
            Name = new string('A', 101),
            Date = DateTime.Now.AddDays(7),
            Location = "San Francisco",
            Description = "A great conference",
            Capacity = 100,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Name)));
    }

    [Fact]
    public void Location_WithValidValue_ShouldPassValidation()
    {
        var model = new EventFormModel
        {
            Name = "Tech Conference",
            Date = DateTime.Now.AddDays(7),
            Location = "San Francisco Convention Center",
            Description = "A great conference",
            Capacity = 100,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains(nameof(model.Location)));
    }

    [Fact]
    public void Location_WithEmptyValue_ShouldFailValidation()
    {
        var model = new EventFormModel
        {
            Name = "Tech Conference",
            Date = DateTime.Now.AddDays(7),
            Location = "",
            Description = "A great conference",
            Capacity = 100,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Location)));
    }

    [Fact]
    public void Location_WithTooShortValue_ShouldFailValidation()
    {
        var model = new EventFormModel
        {
            Name = "Tech Conference",
            Date = DateTime.Now.AddDays(7),
            Location = "SF",
            Description = "A great conference",
            Capacity = 100,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Location)));
    }

    [Fact]
    public void Location_WithTooLongValue_ShouldFailValidation()
    {
        var model = new EventFormModel
        {
            Name = "Tech Conference",
            Date = DateTime.Now.AddDays(7),
            Location = new string('A', 201),
            Description = "A great conference",
            Capacity = 100,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Location)));
    }

    [Fact]
    public void Description_WithValidValue_ShouldPassValidation()
    {
        var model = new EventFormModel
        {
            Name = "Tech Conference",
            Date = DateTime.Now.AddDays(7),
            Location = "San Francisco",
            Description = "A great technology conference with amazing speakers",
            Capacity = 100,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains(nameof(model.Description)));
    }

    [Fact]
    public void Description_WithEmptyValue_ShouldFailValidation()
    {
        var model = new EventFormModel
        {
            Name = "Tech Conference",
            Date = DateTime.Now.AddDays(7),
            Location = "San Francisco",
            Description = "",
            Capacity = 100,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Description)));
    }

    [Fact]
    public void Description_WithTooShortValue_ShouldFailValidation()
    {
        var model = new EventFormModel
        {
            Name = "Tech Conference",
            Date = DateTime.Now.AddDays(7),
            Location = "San Francisco",
            Description = "Too short",
            Capacity = 100,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Description)));
    }

    [Fact]
    public void Description_WithTooLongValue_ShouldFailValidation()
    {
        var model = new EventFormModel
        {
            Name = "Tech Conference",
            Date = DateTime.Now.AddDays(7),
            Location = "San Francisco",
            Description = new string('A', 1001),
            Capacity = 100,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Description)));
    }

    [Fact]
    public void Capacity_WithValidValue_ShouldPassValidation()
    {
        var model = new EventFormModel
        {
            Name = "Tech Conference",
            Date = DateTime.Now.AddDays(7),
            Location = "San Francisco",
            Description = "A great conference",
            Capacity = 500,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.DoesNotContain(validationResults, v => v.MemberNames.Contains(nameof(model.Capacity)));
    }

    [Fact]
    public void Capacity_WithZero_ShouldFailValidation()
    {
        var model = new EventFormModel
        {
            Name = "Tech Conference",
            Date = DateTime.Now.AddDays(7),
            Location = "San Francisco",
            Description = "A great conference",
            Capacity = 0,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Capacity)));
    }

    [Fact]
    public void Capacity_WithNegativeValue_ShouldFailValidation()
    {
        var model = new EventFormModel
        {
            Name = "Tech Conference",
            Date = DateTime.Now.AddDays(7),
            Location = "San Francisco",
            Description = "A great conference",
            Capacity = -1,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Capacity)));
    }

    [Fact]
    public void Capacity_WithTooLargeValue_ShouldFailValidation()
    {
        var model = new EventFormModel
        {
            Name = "Tech Conference",
            Date = DateTime.Now.AddDays(7),
            Location = "San Francisco",
            Description = "A great conference",
            Capacity = 10001,
            Category = "Technology"
        };

        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Capacity)));
    }

    [Fact]
    public void Category_WithEmptyValue_ShouldFailValidation()
    {
        var model = new EventFormModel
        {
            Name = "Tech Conference",
            Date = DateTime.Now.AddDays(7),
            Location = "San Francisco",
            Description = "A great conference",
            Capacity = 100,
            Category = ""
        };

        var validationResults = ValidateModel(model);
        Assert.Contains(validationResults, v => v.MemberNames.Contains(nameof(model.Category)));
    }

    #endregion

    #region Conversion Method Tests

    [Fact]
    public void ToEvent_ShouldConvertToEventModel()
    {
        var date = new DateTime(2025, 6, 15);
        var formModel = new EventFormModel
        {
            Id = 1,
            Name = "Tech Conference",
            Date = date,
            Location = "San Francisco",
            Description = "Annual tech conference",
            Capacity = 500,
            Category = "Technology"
        };

        var evt = formModel.ToEvent();

        Assert.NotNull(evt);
        Assert.Equal(1, evt.Id);
        Assert.Equal("Tech Conference", evt.Name);
        Assert.Equal(date, evt.Date);
        Assert.Equal("San Francisco", evt.Location);
        Assert.Equal("Annual tech conference", evt.Description);
        Assert.Equal(500, evt.Capacity);
        Assert.Equal("Technology", evt.Category);
    }

    [Fact]
    public void FromEvent_ShouldConvertFromEventModel()
    {
        var date = new DateTime(2025, 6, 15);
        var evt = new Event
        {
            Id = 1,
            Name = "Tech Conference",
            Date = date,
            Location = "San Francisco",
            Description = "Annual tech conference",
            Capacity = 500,
            Category = "Technology"
        };

        var formModel = EventFormModel.FromEvent(evt);

        Assert.NotNull(formModel);
        Assert.Equal(1, formModel.Id);
        Assert.Equal("Tech Conference", formModel.Name);
        Assert.Equal(date, formModel.Date);
        Assert.Equal("San Francisco", formModel.Location);
        Assert.Equal("Annual tech conference", formModel.Description);
        Assert.Equal(500, formModel.Capacity);
        Assert.Equal("Technology", formModel.Category);
    }

    [Fact]
    public void ToEvent_ThenFromEvent_ShouldReturnEquivalentModel()
    {
        var date = new DateTime(2025, 6, 15);
        var originalModel = new EventFormModel
        {
            Id = 1,
            Name = "Tech Conference",
            Date = date,
            Location = "San Francisco",
            Description = "Annual tech conference",
            Capacity = 500,
            Category = "Technology"
        };

        var evt = originalModel.ToEvent();
        var roundTripModel = EventFormModel.FromEvent(evt);

        Assert.Equal(originalModel.Id, roundTripModel.Id);
        Assert.Equal(originalModel.Name, roundTripModel.Name);
        Assert.Equal(originalModel.Date, roundTripModel.Date);
        Assert.Equal(originalModel.Location, roundTripModel.Location);
        Assert.Equal(originalModel.Description, roundTripModel.Description);
        Assert.Equal(originalModel.Capacity, roundTripModel.Capacity);
        Assert.Equal(originalModel.Category, roundTripModel.Category);
    }

    [Fact]
    public void FromEvent_ThenToEvent_ShouldReturnEquivalentModel()
    {
        var date = new DateTime(2025, 6, 15);
        var originalEvent = new Event
        {
            Id = 1,
            Name = "Tech Conference",
            Date = date,
            Location = "San Francisco",
            Description = "Annual tech conference",
            Capacity = 500,
            Category = "Technology"
        };

        var formModel = EventFormModel.FromEvent(originalEvent);
        var roundTripEvent = formModel.ToEvent();

        Assert.Equal(originalEvent.Id, roundTripEvent.Id);
        Assert.Equal(originalEvent.Name, roundTripEvent.Name);
        Assert.Equal(originalEvent.Date, roundTripEvent.Date);
        Assert.Equal(originalEvent.Location, roundTripEvent.Location);
        Assert.Equal(originalEvent.Description, roundTripEvent.Description);
        Assert.Equal(originalEvent.Capacity, roundTripEvent.Capacity);
        Assert.Equal(originalEvent.Category, roundTripEvent.Category);
    }

    #endregion

    private static List<ValidationResult> ValidateModel(EventFormModel model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
