using CommunityToolkit.Mvvm.ComponentModel;
using Instrux.App.ViewModels.Tabs;

namespace Instrux.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public TeacherProfileTabViewModel TeacherProfileTab { get; }
    public SchoolClassTabViewModel SchoolClassTab { get; }
    public StudentTabViewModel StudentTab { get; }
    public AttendanceRecordTabViewModel AttendanceRecordTab { get; }
    public ContentItemTabViewModel ContentItemTab { get; }
    public AssessmentTabViewModel AssessmentTab { get; }
    public GradeTabViewModel GradeTab { get; }
    public CalendarEventTabViewModel CalendarEventTab { get; }
    public TodoItemTabViewModel TodoItemTab { get; }

    public MainViewModel(
        TeacherProfileTabViewModel teacherProfileTab,
        SchoolClassTabViewModel schoolClassTab,
        StudentTabViewModel studentTab,
        AttendanceRecordTabViewModel attendanceRecordTab,
        ContentItemTabViewModel contentItemTab,
        AssessmentTabViewModel assessmentTab,
        GradeTabViewModel gradeTab,
        CalendarEventTabViewModel calendarEventTab,
        TodoItemTabViewModel todoItemTab)
    {
        TeacherProfileTab = teacherProfileTab;
        SchoolClassTab = schoolClassTab;
        StudentTab = studentTab;
        AttendanceRecordTab = attendanceRecordTab;
        ContentItemTab = contentItemTab;
        AssessmentTab = assessmentTab;
        GradeTab = gradeTab;
        CalendarEventTab = calendarEventTab;
        TodoItemTab = todoItemTab;
    }
}
