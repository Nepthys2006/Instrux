using Instrux.Domain.Models;
using Instrux.Infrastructure.Data;
using Instrux.Infrastructure.Repositories;
using Instrux.Services.ApplicationLogic;
using Instrux.Services.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Instrux.App.Tests.Services;

public class ServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UnitOfWork _unitOfWork;

    public ServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"InstruxTest_{Guid.NewGuid()}")
            .Options;
        _context = new AppDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SchoolClass_Create_AddsEntity()
    {
        var repo = new Repository<SchoolClass>(_context);
        var service = new SchoolClassService(repo, _unitOfWork);

        var dto = new CreateSchoolClassDto
        {
            Name = "Test Class",
            Subject = "Testing",
            Section = "A",
            Term = "Spring 2026",
            ColorHex = "#FF0000"
        };

        var result = await service.CreateAsync(dto);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Test Class", result.Data.Name);
        Assert.Equal("Testing", result.Data.Subject);
    }

    [Fact]
    public async Task SchoolClass_CreateAndGetAll_ReturnsItems()
    {
        var repo = new Repository<SchoolClass>(_context);
        var service = new SchoolClassService(repo, _unitOfWork);

        await service.CreateAsync(new CreateSchoolClassDto
        {
            Name = "Class 1",
            Subject = "Math",
            Section = "A",
            Term = "Spring 2026"
        });

        await service.CreateAsync(new CreateSchoolClassDto
        {
            Name = "Class 2",
            Subject = "Science",
            Section = "B",
            Term = "Spring 2026"
        });

        var allResult = await service.GetAllAsync();

        Assert.True(allResult.IsSuccess);
        Assert.Equal(2, allResult.Data?.Count);
    }

    [Fact]
    public async Task SchoolClass_Update_ModifiesEntity()
    {
        var repo = new Repository<SchoolClass>(_context);
        var service = new SchoolClassService(repo, _unitOfWork);

        var createResult = await service.CreateAsync(new CreateSchoolClassDto
        {
            Name = "Original",
            Subject = "Math",
            Section = "A",
            Term = "Spring 2026"
        });

        var updateResult = await service.UpdateAsync(createResult.Data!.Id, new CreateSchoolClassDto
        {
            Name = "Updated",
            Subject = "Math",
            Section = "B",
            Term = "Spring 2026"
        });

        Assert.True(updateResult.IsSuccess);
        Assert.Equal("Updated", updateResult.Data?.Name);
        Assert.Equal("B", updateResult.Data?.Section);
    }

    [Fact]
    public async Task SchoolClass_Delete_RemovesEntity()
    {
        var repo = new Repository<SchoolClass>(_context);
        var service = new SchoolClassService(repo, _unitOfWork);

        var createResult = await service.CreateAsync(new CreateSchoolClassDto
        {
            Name = "To Delete",
            Subject = "Math",
            Section = "A",
            Term = "Spring 2026"
        });

        var deleteResult = await service.DeleteAsync(createResult.Data!.Id);
        Assert.True(deleteResult.IsSuccess);

        var allResult = await service.GetAllAsync();
        Assert.Empty(allResult.Data!);
    }

    [Fact]
    public async Task TeacherProfile_Create_AddsEntity()
    {
        var repo = new Repository<TeacherProfile>(_context);
        var service = new TeacherProfileService(repo, _unitOfWork);

        var dto = new CreateTeacherProfileDto
        {
            FullName = "Dr. Test",
            Nickname = "Tester",
            Email = "test@school.edu"
        };

        var result = await service.CreateAsync(dto);
        Assert.True(result.IsSuccess);
        Assert.Equal("Dr. Test", result.Data?.FullName);
        Assert.Equal("test@school.edu", result.Data?.Email);
    }

    [Fact]
    public async Task Student_Create_AddsEntity()
    {
        var classRepo = new Repository<SchoolClass>(_context);
        var classService = new SchoolClassService(classRepo, _unitOfWork);
        var classResult = await classService.CreateAsync(new CreateSchoolClassDto
        {
            Name = "Test Class",
            Subject = "Math",
            Section = "A",
            Term = "Spring 2026"
        });

        var studentRepo = new Repository<Student>(_context);
        var studentService = new StudentService(studentRepo, _unitOfWork);

        var dto = new CreateStudentDto
        {
            ClassId = classResult.Data!.Id,
            Name = "Test Student",
            StudentIdentifier = "STU-TEST",
            Email = "student@school.edu"
        };

        var result = await studentService.CreateAsync(dto);
        Assert.True(result.IsSuccess);
        Assert.Equal("Test Student", result.Data?.Name);
        Assert.Equal(classResult.Data.Id, result.Data?.ClassId);
    }

    [Fact]
    public async Task Grade_Create_AddsEntity()
    {
        var classRepo = new Repository<SchoolClass>(_context);
        var classService = new SchoolClassService(classRepo, _unitOfWork);
        var classResult = await classService.CreateAsync(new CreateSchoolClassDto
        {
            Name = "Math",
            Subject = "Math",
            Section = "A",
            Term = "Spring 2026"
        });

        var studentRepo = new Repository<Student>(_context);
        var studentService = new StudentService(studentRepo, _unitOfWork);
        var studentResult = await studentService.CreateAsync(new CreateStudentDto
        {
            ClassId = classResult.Data!.Id,
            Name = "Student",
            StudentIdentifier = "S1",
            Email = "s@school.edu"
        });

        var assessmentRepo = new Repository<Assessment>(_context);
        var assessmentService = new AssessmentService(assessmentRepo, _unitOfWork);
        var assessmentResult = await assessmentService.CreateAsync(new CreateAssessmentDto
        {
            ClassId = classResult.Data.Id,
            Name = "Quiz 1",
            MaxScore = 100
        });

        var gradeRepo = new Repository<Grade>(_context);
        var gradeService = new GradeService(gradeRepo, _unitOfWork);

        var dto = new CreateGradeDto
        {
            AssessmentId = assessmentResult.Data!.Id,
            StudentId = studentResult.Data!.Id,
            Score = 85
        };

        var result = await gradeService.CreateAsync(dto);
        Assert.True(result.IsSuccess);
        Assert.Equal(85, result.Data?.Score);
    }

    [Fact]
    public async Task Service_GetById_NotFound_ReturnsFailure()
    {
        var repo = new Repository<SchoolClass>(_context);
        var service = new SchoolClassService(repo, _unitOfWork);

        var result = await service.GetByIdAsync(Guid.NewGuid());
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }
}
