using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.App.ViewModels.Tabs;

public partial class GradeTabViewModel : ObservableObject
{
    private readonly IGradeService _service;
    private readonly INavigationService _navigationService;

    public ObservableCollection<GradeDto> Items => AppDataStore.Instance.Grades;
    public ObservableCollection<AssessmentDto> Assessments => AppDataStore.Instance.Assessments;
    public ObservableCollection<StudentDto> Students => AppDataStore.Instance.Students;

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

    public string SaveButtonText => IsEditing ? "Update" : "Save";

    public GradeTabViewModel(
        IGradeService service,
        INavigationService navigationService)
    {
        _service = service;
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
