using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.App.ViewModels.Tabs;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;
using System;
using System.Windows;

namespace Instrux.App.ViewModels;

public partial class ClassManagerViewModel : ObservableObject
{
    private readonly ISchoolClassService _classService;
    private readonly IStudentService _studentService;
    private readonly IAttendanceRecordService _attendanceService;

    public ClassDetailViewModel ClassDetailView { get; }
    public TabRosterViewModel RosterView { get; }
    public TabAttendanceViewModel AttendanceView { get; }
    public TabMaterialsViewModel MaterialsView { get; }
    public TabGradesViewModel GradesView { get; }

    public ObservableCollection<SchoolClassDto> Classes => AppDataStore.Instance.SchoolClasses;

    [ObservableProperty]
    private object? _classSubView;

    [ObservableProperty]
    private SchoolClassDto? _selectedClass;

    [ObservableProperty]
    private bool _isShowingDetail;

    [ObservableProperty]
    private string _formName = string.Empty;

    [ObservableProperty]
    private string _formSubject = string.Empty;

    [ObservableProperty]
    private string _formSection = string.Empty;

    [ObservableProperty]
    private string _formTerm = string.Empty;

    [ObservableProperty]
    private string _formColorHex = "#4F46E5";

    [ObservableProperty]
    private bool _isCreateDialogOpen;

    public static readonly List<string> ColorOptions =
    [
        "#4F46E5", "#6366F1", "#8B5CF6", "#EC4899",
        "#F43F5E", "#F97316", "#10B981", "#14B8A6"
    ];

    public ClassManagerViewModel(
        ISchoolClassService classService,
        IStudentService studentService,
        IAttendanceRecordService attendanceService,
        ClassDetailViewModel classDetailView,
        TabRosterViewModel rosterView,
        TabAttendanceViewModel attendanceView,
        TabMaterialsViewModel materialsView,
        TabGradesViewModel gradesView)
    {
        _classService = classService;
        _studentService = studentService;
        _attendanceService = attendanceService;
        ClassDetailView = classDetailView;
        RosterView = rosterView;
        AttendanceView = attendanceView;
        MaterialsView = materialsView;
        GradesView = gradesView;

        ClassDetailView.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ClassDetailViewModel.ActiveSubTab))
                UpdateClassSubView();
        };
    }

    private void UpdateClassSubView()
    {
        ClassSubView = ClassDetailView.ActiveSubTab switch
        {
            ClassSubTab.Roster => RosterView,
            ClassSubTab.Attendance => AttendanceView,
            ClassSubTab.Materials => MaterialsView,
            ClassSubTab.Grades => GradesView,
            _ => null
        };
    }

    public void NavigateToClassDetail(SchoolClassDto classDto)
    {
        SelectedClass = classDto;
        IsShowingDetail = true;
        ClassDetailView.LoadClass(classDto);
        RosterView.LoadForClass(classDto);
        AttendanceView.LoadForClass(classDto);
        MaterialsView.LoadForClass(classDto);
        GradesView.LoadForClass(classDto);
        UpdateClassSubView();
    }

    [RelayCommand]
    private void NavigateToClassDetailFromCard(SchoolClassDto? classDto)
    {
        if (classDto is not null)
            NavigateToClassDetail(classDto);
    }

    [RelayCommand]
    private void BackToClasses()
    {
        IsShowingDetail = false;
        SelectedClass = null;
    }

    [RelayCommand]
    private void OpenCreateDialog()
    {
        FormName = string.Empty;
        FormSubject = string.Empty;
        FormSection = string.Empty;
        FormTerm = string.Empty;
        FormColorHex = "#4F46E5";
        IsCreateDialogOpen = true;
    }

    [RelayCommand]
    private void CloseCreateDialog()
    {
        IsCreateDialogOpen = false;
    }

    [RelayCommand]
    private async Task CreateClassAsync()
    {
        try
        {
            var dto = new CreateSchoolClassDto
            {
                Name = FormName,
                Subject = FormSubject,
                Section = FormSection,
                Term = FormTerm,
                ColorHex = FormColorHex
            };

            var result = await _classService.CreateAsync(dto);
            if (result.IsSuccess && result.Data is not null)
            {
                AppDataStore.Instance.SchoolClasses.Add(result.Data);
            }

            IsCreateDialogOpen = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to create class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task DeleteClassAsync(SchoolClassDto? classDto)
    {
        try
        {
            if (classDto is null) return;
            var result = await _classService.DeleteAsync(classDto.Id);
            if (result.IsSuccess)
            {
                AppDataStore.Instance.SchoolClasses.Remove(classDto);
                if (SelectedClass?.Id == classDto.Id)
                    BackToClasses();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to delete class: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
