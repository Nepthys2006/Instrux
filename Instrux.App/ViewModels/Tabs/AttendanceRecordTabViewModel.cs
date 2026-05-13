using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.App.ViewModels.Tabs;

public partial class AttendanceRecordTabViewModel : ObservableObject
{
    private readonly IAttendanceRecordService _service;
    private readonly ISchoolClassService _classService;
    private readonly IStudentService _studentService;
    private readonly INavigationService _navigationService;

    private List<StudentDto> _allStudents = [];

    [ObservableProperty]
    private ObservableCollection<AttendanceRecordDto> _items = [];

    [ObservableProperty]
    private ObservableCollection<SchoolClassDto> _classes = [];

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

    [ObservableProperty]
    private bool _isLoading;

    public string SaveButtonText => IsEditing ? "Update" : "Save";

    partial void OnFormClassIdChanged(Guid value)
    {
        if (FormStudentId != Guid.Empty)
        {
            var student = _allStudents.FirstOrDefault(s => s.Id == FormStudentId);
            if (student is null || student.ClassId != value)
                FormStudentId = Guid.Empty;
        }
        UpdateFilteredStudents();
    }

    partial void OnFormStudentIdChanged(Guid value)
    {
        if (value != Guid.Empty && FormClassId == Guid.Empty)
        {
            var student = _allStudents.FirstOrDefault(s => s.Id == value);
            if (student is not null)
                FormClassId = student.ClassId;
        }
        UpdateFilteredStudents();
    }

    private void UpdateFilteredStudents()
    {
        var filtered = FormClassId == Guid.Empty
            ? _allStudents
            : _allStudents.Where(s => s.ClassId == FormClassId).ToList();
        Students = new ObservableCollection<StudentDto>(filtered);
    }

    public AttendanceRecordTabViewModel(
        IAttendanceRecordService service,
        ISchoolClassService classService,
        IStudentService studentService,
        INavigationService navigationService)
    {
        _service = service;
        _classService = classService;
        _studentService = studentService;
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
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var result = await _service.GetAllAsync();
            if (result.IsSuccess && result.Data is not null)
                Items = new ObservableCollection<AttendanceRecordDto>(result.Data);

            var classResult = await _classService.GetAllAsync();
            if (classResult.IsSuccess && classResult.Data is not null)
                Classes = new ObservableCollection<SchoolClassDto>(classResult.Data);

            var studentResult = await _studentService.GetAllAsync();
            if (studentResult.IsSuccess && studentResult.Data is not null)
                _allStudents = studentResult.Data.ToList();
            UpdateFilteredStudents();
        }
        finally { IsLoading = false; }
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
            if (result.IsSuccess)
            {
                await LoadAsync();
                AddNew();
            }
        }
        else
        {
            var result = await _service.CreateAsync(dto);
            if (result.IsSuccess)
            {
                await LoadAsync();
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
            await LoadAsync();
            AddNew();
        }
    }
}
