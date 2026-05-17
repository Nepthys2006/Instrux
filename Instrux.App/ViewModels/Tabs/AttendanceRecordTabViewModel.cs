using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.App.ViewModels.Tabs;

public partial class AttendanceRecordTabViewModel : ObservableObject
{
    private readonly IAttendanceRecordService _service;
    private readonly INavigationService _navigationService;

    public ObservableCollection<AttendanceRecordDto> Items => AppDataStore.Instance.AttendanceRecords;
    public ObservableCollection<SchoolClassDto> Classes => AppDataStore.Instance.SchoolClasses;
    private IEnumerable<StudentDto> AllStudents => AppDataStore.Instance.Students;

    [ObservableProperty]
    private ObservableCollection<StudentDto> _students = [];

    [ObservableProperty]
    private AttendanceRecordDto? _selectedItem;

    [ObservableProperty]
    private Guid _formClassId;

    [ObservableProperty]
    private Guid _formStudentId;

    [ObservableProperty]
    private DateTime _formDate = DateTime.Today;

    [ObservableProperty]
    private string _formStatus = "Present";

    [ObservableProperty]
    private string? _formNote;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SaveButtonText))]
    private bool _isEditing;

    public List<string> StatusOptions { get; } = ["Present", "Absent", "Late", "Excused"];

    public string SaveButtonText => IsEditing ? "Update" : "Save";

    partial void OnFormClassIdChanged(Guid value)
    {
        if (FormStudentId != Guid.Empty)
        {
            var student = AllStudents.FirstOrDefault(s => s.Id == FormStudentId);
            if (student is null || student.ClassId != value)
                FormStudentId = Guid.Empty;
        }
        UpdateFilteredStudents();
    }

    partial void OnFormStudentIdChanged(Guid value)
    {
        if (value != Guid.Empty && FormClassId == Guid.Empty)
        {
            var student = AllStudents.FirstOrDefault(s => s.Id == value);
            if (student is not null)
                FormClassId = student.ClassId;
        }
        UpdateFilteredStudents();
    }

    private void UpdateFilteredStudents()
    {
        var filtered = FormClassId == Guid.Empty
            ? AllStudents
            : AllStudents.Where(s => s.ClassId == FormClassId);
        Students = new ObservableCollection<StudentDto>(filtered);
    }

    public AttendanceRecordTabViewModel(
        IAttendanceRecordService service,
        INavigationService navigationService)
    {
        _service = service;
        _navigationService = navigationService;
    }

    partial void OnSelectedItemChanged(AttendanceRecordDto? value)
    {
        if (value is not null)
        {
            FormClassId = value.ClassId;
            FormStudentId = value.StudentId;
            FormDate = value.Date;
            FormStatus = value.Status;
            FormNote = value.Note;
            IsEditing = true;
        }
    }

    [RelayCommand]
    private void AddNew()
    {
        FormClassId = Guid.Empty;
        FormStudentId = Guid.Empty;
        FormDate = DateTime.Today;
        FormStatus = "Present";
        FormNote = null;
        IsEditing = false;
        SelectedItem = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var dto = new CreateAttendanceRecordDto
        {
            ClassId = FormClassId,
            StudentId = FormStudentId,
            Date = FormDate,
            Status = FormStatus,
            Note = FormNote
        };

        if (IsEditing && SelectedItem is not null)
        {
            var result = await _service.UpdateAsync(SelectedItem.Id, dto);
            if (result.IsSuccess && result.Data is not null)
            {
                var i = Items.IndexOf(SelectedItem);
                if (i >= 0) Items[i] = result.Data;
                AddNew();
            }
        }
        else
        {
            var result = await _service.CreateAsync(dto);
            if (result.IsSuccess && result.Data is not null)
            {
                Items.Add(result.Data);
                AddNew();
            }
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (SelectedItem is null) return;
        var result = await _service.DeleteAsync(SelectedItem.Id);
        if (result.IsSuccess)
        {
            Items.Remove(SelectedItem);
            AddNew();
        }
    }
}
