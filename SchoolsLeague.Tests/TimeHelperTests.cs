using Common;

namespace SchoolsLeague.Tests;

public class TimeHelperTests
{
    [Fact]
    public void Now_ShouldBeInRiyadhTimezone()
    {
        var riyadhNow = TimeHelper.Now;
        var utcNow = DateTime.UtcNow;

        // Riyadh is UTC+3, so Now should be 3 hours ahead
        var diff = riyadhNow - utcNow;
        Assert.InRange(diff.TotalHours, 2, 4);
    }

    [Fact]
    public void Today_ShouldHaveNoTimeComponent()
    {
        var today = TimeHelper.Today;
        Assert.Equal(0, today.Hour);
        Assert.Equal(0, today.Minute);
        Assert.Equal(0, today.Second);
    }

    [Fact]
    public void Now_ShouldBeTodayOrLater()
    {
        Assert.True(TimeHelper.Now >= TimeHelper.Today);
    }

    [Fact]
    public void Today_ShouldBeUtcPlus3()
    {
        var today = TimeHelper.Today;
        var utcToday = DateTime.UtcNow.Date;

        // In Riyadh timezone, "today" could be the same or one day ahead of UTC
        var diff = today - utcToday;
        Assert.InRange(diff.TotalHours, -1, 4);
    }
}
