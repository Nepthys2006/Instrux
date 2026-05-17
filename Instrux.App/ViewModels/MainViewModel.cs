using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.App.ViewModels.Tabs;
using Instrux.App.Views.Tabs;
using Instrux.Services.DTOs;

namespace Instrux.App.ViewModels;

public enum AppView
{
    Dashboard,
    Classes,
    ClassDetail,
    Calendar,
    Todos,
    Settings
}

public partial class MainViewModel : ObservableObject
{
    public DashboardViewModel DashboardView { get; }
    public ClassManagerViewModel ClassManagerView { get; }
    public CalendarViewModel CalendarView { get; }
    public TodoViewModel TodoView { get; }
    public SettingsViewModel SettingsView { get; }

    [ObservableProperty]
    private AppView _currentAppView = AppView.Dashboard;

    [ObservableProperty]
    private object? _currentView;

    [ObservableProperty]
    private bool _isDashboardSelected = true;

    [ObservableProperty]
    private bool _isClassesSelected;

    [ObservableProperty]
    private bool _isCalendarSelected;

    [ObservableProperty]
    private bool _isTodosSelected;

    [ObservableProperty]
    private SchoolClassDto? _selectedClass;

    public ObservableCollection<SchoolClassDto> QuickAccessClasses => AppDataStore.Instance.SchoolClasses;

    public MainViewModel(
        DashboardViewModel dashboardViewModel,
        ClassManagerViewModel classManagerViewModel,
        CalendarViewModel calendarView,
        TodoViewModel todoView,
        SettingsViewModel settingsView)
    {
        DashboardView = dashboardViewModel;
        ClassManagerView = classManagerViewModel;
        CalendarView = calendarView;
        TodoView = todoView;
        SettingsView = settingsView;

        NavigateToDashboard();
    }

    partial void OnCurrentAppViewChanged(AppView value)
    {
        IsDashboardSelected = value == AppView.Dashboard;
        IsClassesSelected = value == AppView.Classes;
        IsCalendarSelected = value == AppView.Calendar;
        IsTodosSelected = value == AppView.Todos;

        if (value == AppView.Calendar) CalendarView.RefreshEvents();

        CurrentView = value switch
        {
            AppView.Dashboard => DashboardView,
            AppView.Classes => ClassManagerView,
            AppView.ClassDetail => ClassManagerView,
            AppView.Calendar => CalendarView,
            AppView.Todos => TodoView,
            AppView.Settings => SettingsView,
            _ => DashboardView
        };
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        CurrentAppView = AppView.Dashboard;
    }

    [RelayCommand]
    private void NavigateToClasses()
    {
        CurrentAppView = AppView.Classes;
    }

    [RelayCommand]
    private void NavigateToCalendar()
    {
        CurrentAppView = AppView.Calendar;
    }

    [RelayCommand]
    private void NavigateToTodos()
    {
        TodoView.RefreshItems();
        CurrentAppView = AppView.Todos;
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentAppView = AppView.Settings;
    }

    [RelayCommand]
    private void NavigateToClass(SchoolClassDto? classDto)
    {
        if (classDto is null) return;
        SelectedClass = classDto;
        ClassManagerView.NavigateToClassDetail(classDto);
        CurrentAppView = AppView.ClassDetail;
    }
}
