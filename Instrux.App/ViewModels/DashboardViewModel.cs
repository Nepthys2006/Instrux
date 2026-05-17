using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.Services.DTOs;

namespace Instrux.App.ViewModels;

public class StatCard
{
    public string Label { get; init; } = string.Empty;
    public string Value { get; init; } = "0";
    public string Color { get; init; } = "#4F46E5";
}

public partial class DashboardViewModel : ObservableObject
{
    public ObservableCollection<SchoolClassDto> Classes => AppDataStore.Instance.SchoolClasses;
    public ObservableCollection<CalendarEventDto> CalendarEvents => AppDataStore.Instance.CalendarEvents;
    public ObservableCollection<TodoItemDto> TodoItems => AppDataStore.Instance.TodoItems;

    [ObservableProperty]
    private ObservableCollection<StatCard> _statCards = [];

    [ObservableProperty]
    private string _greeting = "Good morning, Teacher";

    [ObservableProperty]
    private string _todayDate = DateTime.Now.ToString("dddd, MMMM d");

    [ObservableProperty]
    private List<CalendarEventDto> _todayEvents = [];

    [ObservableProperty]
    private List<TodoItemDto> _priorityTodos = [];

    public DashboardViewModel()
    {
        SubscribeToChanges();
        RefreshAll();
    }

    private void SubscribeToChanges()
    {
        AppDataStore.Instance.SchoolClasses.CollectionChanged += OnDataChanged;
        AppDataStore.Instance.Students.CollectionChanged += OnDataChanged;
        AppDataStore.Instance.Assessments.CollectionChanged += OnDataChanged;
        AppDataStore.Instance.CalendarEvents.CollectionChanged += OnDataChanged;
        AppDataStore.Instance.TodoItems.CollectionChanged += OnDataChanged;
    }

    private void OnDataChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        var classes = AppDataStore.Instance.SchoolClasses;
        var students = AppDataStore.Instance.Students;
        var assessments = AppDataStore.Instance.Assessments;
        var todos = AppDataStore.Instance.TodoItems;
        var events = AppDataStore.Instance.CalendarEvents;
        var profile = AppDataStore.Instance.TeacherProfiles.FirstOrDefault();

        var hour = DateTime.Now.Hour;
        var timeGreeting = hour < 12 ? "Good morning" : hour < 18 ? "Good afternoon" : "Good evening";
        Greeting = $"{timeGreeting}, {profile?.Nickname ?? "Teacher"}";
        TodayDate = DateTime.Now.ToString("dddd, MMMM d");

        var totalStudents = students.Count;
        var pendingTasks = todos.Count(t => !t.IsCompleted);

        StatCards =
        [
            new StatCard { Label = "Total Classes", Value = classes.Count.ToString(), Color = "#4F46E5" },
            new StatCard { Label = "Total Students", Value = totalStudents.ToString(), Color = "#10B981" },
            new StatCard { Label = "Assessments", Value = assessments.Count.ToString(), Color = "#8B5CF6" },
            new StatCard { Label = "Tasks Pending", Value = pendingTasks.ToString(), Color = "#EF4444" },
        ];

        var today = DateTime.Today;
        TodayEvents = events
            .Where(e => e.Date.Date == today)
            .OrderBy(e => e.TimeRange)
            .Take(5)
            .ToList();

        PriorityTodos = todos
            .Where(t => !t.IsCompleted)
            .OrderByDescending(t => t.Priority == "High")
            .ThenBy(t => t.DueDate)
            .Take(4)
            .ToList();
    }

    [RelayCommand]
    private void NavigateToClass(SchoolClassDto? classDto)
    {
    }
}
