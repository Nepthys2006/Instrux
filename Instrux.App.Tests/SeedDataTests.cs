using Instrux.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Instrux.App.Tests;

public class SeedDataTests
{
    [Fact]
    public async Task SeedAsync_SeedsAllEntities()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"SeedTest_{Guid.NewGuid()}")
            .Options;

        using var context = new AppDbContext(options);

        await DbInitializer.SeedAsync(context);

        Assert.Equal(3, await context.SchoolClasses.CountAsync());
        Assert.Equal(1, await context.TeacherProfiles.CountAsync());
        Assert.Equal(3, await context.Students.CountAsync());
        Assert.Equal(2, await context.AttendanceRecords.CountAsync());
        Assert.Equal(2, await context.ContentItems.CountAsync());
        Assert.Equal(1, await context.Assessments.CountAsync());
        Assert.Equal(2, await context.Grades.CountAsync());
        Assert.Equal(1, await context.CalendarEvents.CountAsync());
        Assert.Equal(1, await context.TodoItems.CountAsync());
    }

    [Fact]
    public async Task SeedAsync_IsIdempotent()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"SeedIdempotent_{Guid.NewGuid()}")
            .Options;

        using var context = new AppDbContext(options);

        await DbInitializer.SeedAsync(context);
        await DbInitializer.SeedAsync(context);

        Assert.Equal(3, await context.SchoolClasses.CountAsync());
    }

    [Fact]
    public async Task SeedAsync_SchoolClasses_HaveCorrectSubject()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"SeedCheck_{Guid.NewGuid()}")
            .Options;

        using var context = new AppDbContext(options);

        await DbInitializer.SeedAsync(context);

        var classes = await context.SchoolClasses.ToListAsync();
        Assert.Contains(classes, c => c.Subject == "Mathematics");
        Assert.Contains(classes, c => c.Subject == "Physics");
        Assert.Contains(classes, c => c.Subject == "English");
    }
}
