using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;
using System;
using System.Windows;

namespace Instrux.App.ViewModels.Tabs;

public partial class AttendanceStudentItem : ObservableObject
{
    public Guid StudentId { get; init; }
    public string StudentName { get; init; } = string.Empty;

    [ObservableProperty]
    private string _status = "Present";
}

public partial class TabAttendanceViewModel : ObservableObject
{
    private readonly IAttendanceRecordService _service;

    [ObservableProperty]
    private ObservableCollection<AttendanceStudentItem> _students = [];

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    [ObservableProperty]
    private Guid _currentClassId;

    public List<string> StatusOptions { get; } = ["Present", "Absent", "Late", "Excused"];

    private SchoolClassDto? _currentClass;

    public TabAttendanceViewModel(IAttendanceRecordService service)
    {
        _service = service;
    }

    public void LoadForClass(SchoolClassDto classDto)
    {
        _currentClass = classDto;
        CurrentClassId = classDto.Id;
        LoadStudents();
    }

    private void LoadStudents()
    {
        var classStudents = AppDataStore.Instance.Students
            .Where(s => s.ClassId == CurrentClassId)
            .Select(s => new AttendanceStudentItem
            {
                StudentId = s.Id,
                StudentName = s.Name,
                Status = "Present"
            });

        Students = new ObservableCollection<AttendanceStudentItem>(classStudents);
    }

    [RelayCommand]
    private void MarkAll(string status)
    {
        foreach (var student in Students)
            student.Status = status;
    }

    [RelayCommand]
    private async Task SaveAttendanceAsync()
    {
        try
        {
            foreach (var student in Students)
            {
                var existing = AppDataStore.Instance.AttendanceRecords
                    .FirstOrDefault(a => a.StudentId == student.StudentId && a.Date.Date == SelectedDate.Date);

                if (existing is not null)
                {
                    var updateDto = new CreateAttendanceRecordDto
                    {
                        ClassId = CurrentClassId,
                        StudentId = student.StudentId,
                        Date = SelectedDate,
                        Status = student.Status
                    };
                    await _service.UpdateAsync(existing.Id, updateDto);
                }
                else
                {
                    var createDto = new CreateAttendanceRecordDto
                    {
                        ClassId = CurrentClassId,
                        StudentId = student.StudentId,
                        Date = SelectedDate,
                        Status = student.Status
                    };
                    await _service.CreateAsync(createDto);
                }
            }

            await RefreshAttendanceAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save attendance: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task RefreshAttendanceAsync()
    {
        try
        {
            var result = await _service.GetAllAsync(default);
            if (result.IsSuccess && result.Data is not null)
            {
                AppDataStore.Instance.AttendanceRecords.Clear();
                foreach (var item in result.Data)
                    AppDataStore.Instance.AttendanceRecords.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to refresh attendance: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
