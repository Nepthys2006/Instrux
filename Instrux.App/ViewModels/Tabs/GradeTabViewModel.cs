using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.App.ViewModels.Tabs;

public partial class GradeTabViewModel : ObservableObject
{
    private readonly IGradeService _service;
    private readonly IAssessmentService _assessmentService;
    private readonly IStudentService _studentService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<GradeDto> _items = [];

    [ObservableProperty]
    private ObservableCollection<AssessmentDto> _assessments = [];

    [ObservableProperty]
    private ObservableCollection<StudentDto> _students = [];

    [ObservableProperty]
    private GradeDto? _selectedItem;

    [ObservableProperty]
    private Guid _formAssessmentId;

    [ObservableProperty]
    private Guid _formStudentId;

    [ObservableProperty]
    private double? _formScore;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SaveButtonText))]
    private bool _isEditing;

    [ObservableProperty]
    private bool _isLoading;

    public string SaveButtonText => IsEditing ? "Update" : "Save";

    public GradeTabViewModel(
        IGradeService service,
        IAssessmentService assessmentService,
        IStudentService studentService,
        INavigationService navigationService)
    {
        _service = service;
        _assessmentService = assessmentService;
        _studentService = studentService;
        _navigationService = navigationService;
    }

    partial void OnSelectedItemChanged(GradeDto? value)
    {
        if (value is not null)
        {
            FormAssessmentId = value.AssessmentId;
            FormStudentId = value.StudentId;
            FormScore = value.Score;
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
                Items = new ObservableCollection<GradeDto>(result.Data);

            var assessmentResult = await _assessmentService.GetAllAsync();
            if (assessmentResult.IsSuccess && assessmentResult.Data is not null)
                Assessments = new ObservableCollection<AssessmentDto>(assessmentResult.Data);

            var studentResult = await _studentService.GetAllAsync();
            if (studentResult.IsSuccess && studentResult.Data is not null)
                Students = new ObservableCollection<StudentDto>(studentResult.Data);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private void AddNew()
    {
        FormAssessmentId = Guid.Empty;
        FormStudentId = Guid.Empty;
        FormScore = null;
        IsEditing = false;
        SelectedItem = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var dto = new CreateGradeDto
        {
            AssessmentId = FormAssessmentId,
            StudentId = FormStudentId,
            Score = FormScore
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
