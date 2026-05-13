using EventEase.Models;

namespace EventEase.Tests.Models;

public class EventTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDefaultValues()
    {
        var evt = new Event();

        Assert.Equal(0, evt.Id);
        Assert.Equal(string.Empty, evt.Name);
        Assert.Equal(default(DateTime), evt.Date);
        Assert.Equal(string.Empty, evt.Location);
        Assert.Equal(string.Empty, evt.Description);
        Assert.Equal(0, evt.Capacity);
        Assert.Equal(string.Empty, evt.Category);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var evt = new Event
        {
            Id = 1,
            Name = "Tech Conference",
            Date = new DateTime(2025, 6, 15),
            Location = "San Francisco",
            Description = "Annual tech conference",
            Capacity = 500,
            Category = "Technology"
        };

        Assert.Equal(1, evt.Id);
        Assert.Equal("Tech Conference", evt.Name);
        Assert.Equal(new DateTime(2025, 6, 15), evt.Date);
        Assert.Equal("San Francisco", evt.Location);
        Assert.Equal("Annual tech conference", evt.Description);
        Assert.Equal(500, evt.Capacity);
        Assert.Equal("Technology", evt.Category);
    }

    [Fact]
    public void Properties_ShouldAllowEmptyStrings()
    {
        var evt = new Event
        {
            Name = "",
            Location = "",
            Description = "",
            Category = ""
        };

        Assert.Equal(string.Empty, evt.Name);
        Assert.Equal(string.Empty, evt.Location);
        Assert.Equal(string.Empty, evt.Description);
        Assert.Equal(string.Empty, evt.Category);
    }

    [Fact]
    public void Date_ShouldAllowPastDates()
    {
        var pastDate = DateTime.Now.AddDays(-30);
        var evt = new Event { Date = pastDate };

        Assert.Equal(pastDate, evt.Date);
    }

    [Fact]
    public void Date_ShouldAllowFutureDates()
    {
        var futureDate = DateTime.Now.AddDays(30);
        var evt = new Event { Date = futureDate };

        Assert.Equal(futureDate, evt.Date);
    }

    [Fact]
    public void Capacity_ShouldAllowZero()
    {
        var evt = new Event { Capacity = 0 };

        Assert.Equal(0, evt.Capacity);
    }

    [Fact]
    public void Capacity_ShouldAllowLargeNumbers()
    {
        var evt = new Event { Capacity = 10000 };

        Assert.Equal(10000, evt.Capacity);
    }

    [Fact]
    public void Capacity_ShouldAllowNegativeNumbers()
    {
        var evt = new Event { Capacity = -1 };

        Assert.Equal(-1, evt.Capacity);
    }
}
