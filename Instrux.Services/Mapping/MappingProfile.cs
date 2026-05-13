using AutoMapper;
using Instrux.Domain.Models;
using Instrux.Services.DTOs;

namespace Instrux.Services.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // SchoolClass
        CreateMap<SchoolClass, SchoolClassDto>()
            .ForMember(d => d.StudentCount, opt => opt.MapFrom(s => s.Students.Count));
        CreateMap<CreateSchoolClassDto, SchoolClass>();

        // TeacherProfile
        CreateMap<TeacherProfile, TeacherProfileDto>();
        CreateMap<CreateTeacherProfileDto, TeacherProfile>();

        // Student
        CreateMap<Student, StudentDto>()
            .ForMember(d => d.ClassName, opt => opt.MapFrom(s => s.Class != null ? s.Class.Name : string.Empty));
        CreateMap<CreateStudentDto, Student>();

        // AttendanceRecord
        CreateMap<AttendanceRecord, AttendanceRecordDto>()
            .ForMember(d => d.StudentName, opt => opt.MapFrom(s => s.Student != null ? s.Student.Name : string.Empty));
        CreateMap<CreateAttendanceRecordDto, AttendanceRecord>();

        // ContentItem
        CreateMap<ContentItem, ContentItemDto>()
            .ForMember(d => d.ClassName, opt => opt.MapFrom(s => s.Class != null ? s.Class.Name : string.Empty));
        CreateMap<CreateContentItemDto, ContentItem>();

        // Assessment
        CreateMap<Assessment, AssessmentDto>()
            .ForMember(d => d.ClassName, opt => opt.MapFrom(s => s.Class != null ? s.Class.Name : string.Empty));
        CreateMap<CreateAssessmentDto, Assessment>();

        // Grade
        CreateMap<Grade, GradeDto>()
            .ForMember(d => d.AssessmentName, opt => opt.MapFrom(s => s.Assessment != null ? s.Assessment.Name : string.Empty))
            .ForMember(d => d.StudentName, opt => opt.MapFrom(s => s.Student != null ? s.Student.Name : string.Empty));
        CreateMap<CreateGradeDto, Grade>();

        // CalendarEvent
        CreateMap<CalendarEvent, CalendarEventDto>();
        CreateMap<CreateCalendarEventDto, CalendarEvent>();

        // TodoItem
        CreateMap<TodoItem, TodoItemDto>();
        CreateMap<CreateTodoItemDto, TodoItem>();
    }
}
