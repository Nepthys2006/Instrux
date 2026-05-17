using System.Collections.ObjectModel;
using Instrux.Services.Common;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.App.Data;

public class AppDataStore
{
    public static AppDataStore Instance { get; private set; } = null!;

    private readonly ITeacherProfileService _teacherProfileService;
    private readonly ISchoolClassService _schoolClassService;
    private readonly IStudentService _studentService;
    private readonly IAttendanceRecordService _attendanceRecordService;
    private readonly IContentItemService _contentItemService;
    private readonly IAssessmentService _assessmentService;
    private readonly IGradeService _gradeService;
    private readonly ICalendarEventService _calendarEventService;
    private readonly ITodoItemService _todoItemService;

    public ObservableCollection<TeacherProfileDto> TeacherProfiles { get; } = [];
    public ObservableCollection<SchoolClassDto> SchoolClasses { get; } = [];
    public ObservableCollection<StudentDto> Students { get; } = [];
    public ObservableCollection<AttendanceRecordDto> AttendanceRecords { get; } = [];
    public ObservableCollection<ContentItemDto> ContentItems { get; } = [];
    public ObservableCollection<AssessmentDto> Assessments { get; } = [];
    public ObservableCollection<GradeDto> Grades { get; } = [];
    public ObservableCollection<CalendarEventDto> CalendarEvents { get; } = [];
    public ObservableCollection<TodoItemDto> TodoItems { get; } = [];

    public AppDataStore(
        ITeacherProfileService teacherProfileService,
        ISchoolClassService schoolClassService,
        IStudentService studentService,
        IAttendanceRecordService attendanceRecordService,
        IContentItemService contentItemService,
        IAssessmentService assessmentService,
        IGradeService gradeService,
        ICalendarEventService calendarEventService,
        ITodoItemService todoItemService)
    {
        Instance = this;

        _teacherProfileService = teacherProfileService;
        _schoolClassService = schoolClassService;
        _studentService = studentService;
        _attendanceRecordService = attendanceRecordService;
        _contentItemService = contentItemService;
        _assessmentService = assessmentService;
        _gradeService = gradeService;
        _calendarEventService = calendarEventService;
        _todoItemService = todoItemService;
    }

    public async Task InitializeAsync()
    {
        await LoadCollection(TeacherProfiles, _teacherProfileService.GetAllAsync);
        await LoadCollection(SchoolClasses, _schoolClassService.GetAllAsync);
        await LoadCollection(Students, _studentService.GetAllAsync);
        await LoadCollection(AttendanceRecords, _attendanceRecordService.GetAllAsync);
        await LoadCollection(ContentItems, _contentItemService.GetAllAsync);
        await LoadCollection(Assessments, _assessmentService.GetAllAsync);
        await LoadCollection(Grades, _gradeService.GetAllAsync);
        await LoadCollection(CalendarEvents, _calendarEventService.GetAllAsync);
        await LoadCollection(TodoItems, _todoItemService.GetAllAsync);
    }

    private static async Task LoadCollection<T>(
        ObservableCollection<T> collection,
        Func<CancellationToken, Task<Result<IReadOnlyList<T>>>> loadFunc)
    {
        var result = await loadFunc(default);
        if (result.IsSuccess && result.Data is not null)
        {
            collection.Clear();
            foreach (var item in result.Data)
                collection.Add(item);
        }
    }
}
