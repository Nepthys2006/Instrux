using Instrux.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Instrux.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.SchoolClasses.AnyAsync())
            return;

        var math101 = new SchoolClass
        {
            Name = "Mathematics 101",
            Subject = "Mathematics",
            Section = "A",
            Term = "Fall 2026",
            ColorHex = "#4F46E5"
        };

        var physics101 = new SchoolClass
        {
            Name = "Physics 101",
            Subject = "Physics",
            Section = "B",
            Term = "Fall 2026",
            ColorHex = "#DC2626"
        };

        var eng101 = new SchoolClass
        {
            Name = "English Literature",
            Subject = "English",
            Section = "A",
            Term = "Fall 2026",
            ColorHex = "#059669"
        };

        context.SchoolClasses.AddRange(math101, physics101, eng101);

        var teacher = new TeacherProfile
        {
            FullName = "Dr. Sarah Johnson",
            Nickname = "Dr. J",
            Email = "sarah.johnson@school.edu"
        };

        context.TeacherProfiles.Add(teacher);

        var alice = new Student
        {
            ClassId = math101.Id,
            Name = "Alice Smith",
            StudentIdentifier = "STU-001",
            Email = "alice.smith@school.edu"
        };

        var bob = new Student
        {
            ClassId = math101.Id,
            Name = "Bob Jones",
            StudentIdentifier = "STU-002",
            Email = "bob.jones@school.edu"
        };

        var carol = new Student
        {
            ClassId = physics101.Id,
            Name = "Carol Williams",
            StudentIdentifier = "STU-003",
            Email = "carol.williams@school.edu"
        };

        context.Students.AddRange(alice, bob, carol);

        var attendance1 = new AttendanceRecord
        {
            ClassId = math101.Id,
            StudentId = alice.Id,
            Date = new DateTime(2026, 5, 15),
            Status = "Present"
        };

        var attendance2 = new AttendanceRecord
        {
            ClassId = math101.Id,
            StudentId = bob.Id,
            Date = new DateTime(2026, 5, 15),
            Status = "Late"
        };

        context.AttendanceRecords.AddRange(attendance1, attendance2);

        var content1 = new ContentItem
        {
            ClassId = math101.Id,
            Title = "Introduction to Algebra",
            Url = "https://example.com/algebra-intro",
            Type = "Link"
        };

        var content2 = new ContentItem
        {
            ClassId = physics101.Id,
            Title = "Newton's Laws Worksheet",
            Url = "https://example.com/newton-worksheet",
            Type = "File"
        };

        context.ContentItems.AddRange(content1, content2);

        var assessment = new Assessment
        {
            ClassId = math101.Id,
            Name = "Midterm Exam",
            MaxScore = 100
        };

        context.Assessments.Add(assessment);

        var grade1 = new Grade
        {
            AssessmentId = assessment.Id,
            StudentId = alice.Id,
            Score = 88
        };

        var grade2 = new Grade
        {
            AssessmentId = assessment.Id,
            StudentId = bob.Id,
            Score = 72
        };

        context.Grades.AddRange(grade1, grade2);

        var calendarEvent = new CalendarEvent
        {
            Title = "Math Final Exam",
            Date = new DateTime(2026, 6, 10),
            TimeRange = "09:00 - 11:00",
            Category = "Exam",
            ClassId = math101.Id
        };

        context.CalendarEvents.Add(calendarEvent);

        var todo = new TodoItem
        {
            Title = "Grade homework assignments",
            Priority = "High",
            IsCompleted = false,
            DueDate = new DateTime(2026, 5, 20)
        };

        context.TodoItems.Add(todo);

        await context.SaveChangesAsync();
    }
}
