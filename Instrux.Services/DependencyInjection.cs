using System.Reflection;
using FluentValidation;
using Instrux.Services.ApplicationLogic;
using Instrux.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Instrux.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<ISchoolClassService, SchoolClassService>();
        services.AddScoped<ITeacherProfileService, TeacherProfileService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IAttendanceRecordService, AttendanceRecordService>();
        services.AddScoped<IContentItemService, ContentItemService>();
        services.AddScoped<IAssessmentService, AssessmentService>();
        services.AddScoped<IGradeService, GradeService>();
        services.AddScoped<ICalendarEventService, CalendarEventService>();
        services.AddScoped<ITodoItemService, TodoItemService>();

        return services;
    }
}
