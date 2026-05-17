using Instrux.Domain.Models;
using Instrux.Infrastructure.Data;
using Instrux.Infrastructure.Repositories;
using Instrux.Services.ApplicationLogic;
using Instrux.Services.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Instrux.App.Tests.Data;

public class DataLoadingTests
{
    [Fact]
    public async Task GetAllFromAllServices_ReturnsSeededData()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"AllData_{Guid.NewGuid()}")
            .Options;

        using var context = new AppDbContext(options);
        var unitOfWork = new UnitOfWork(context);

        await DbInitializer.SeedAsync(context);

        var schoolClassService = new SchoolClassService(new Repository<SchoolClass>(context), unitOfWork);
        var studentService = new StudentService(new Repository<Student>(context), unitOfWork);
        var teacherService = new TeacherProfileService(new Repository<TeacherProfile>(context), unitOfWork);
        var attendanceService = new AttendanceRecordService(new Repository<AttendanceRecord>(context), unitOfWork);
        var contentService = new ContentItemService(new Repository<ContentItem>(context), unitOfWork);
        var assessmentService = new AssessmentService(new Repository<Assessment>(context), unitOfWork);
        var gradeService = new GradeService(new Repository<Grade>(context), unitOfWork);
        var calendarService = new CalendarEventService(new Repository<CalendarEvent>(context), unitOfWork);
        var todoService = new TodoItemService(new Repository<TodoItem>(context), unitOfWork);

        var classes = await schoolClassService.GetAllAsync();
        var students = await studentService.GetAllAsync();
        var teachers = await teacherService.GetAllAsync();
        var attendance = await attendanceService.GetAllAsync();
        var content = await contentService.GetAllAsync();
        var assessments = await assessmentService.GetAllAsync();
        var grades = await gradeService.GetAllAsync();
        var calendar = await calendarService.GetAllAsync();
        var todos = await todoService.GetAllAsync();

        Assert.Equal(3, classes.Data?.Count);
        Assert.Equal(3, students.Data?.Count);
        Assert.Single(teachers.Data!);
        Assert.Equal(2, attendance.Data?.Count);
        Assert.Equal(2, content.Data?.Count);
        Assert.Single(assessments.Data!);
        Assert.Equal(2, grades.Data?.Count);
        Assert.Single(calendar.Data!);
        Assert.Single(todos.Data!);
    }
}
