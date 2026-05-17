using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;
using System;
using System.Windows;

namespace Instrux.App.ViewModels;

public partial class TabGradesViewModel : ObservableObject
{
    private readonly IAssessmentService _assessmentService;
    private readonly IGradeService _gradeService;

    [ObservableProperty]
    private Guid _currentClassId;

    [ObservableProperty]
    private ObservableCollection<AssessmentDto> _assessments = [];

    [ObservableProperty]
    private AssessmentDto? _selectedAssessment;

    [ObservableProperty]
    private string _formAssessmentName = string.Empty;

    [ObservableProperty]
    private double _formMaxScore = 100;

    [ObservableProperty]
    private bool _isAddAssessmentDialogOpen;

    [ObservableProperty]
    private bool _isGradeEditorOpen;

    public ObservableCollection<StudentDto> ClassStudents => [];

    private SchoolClassDto? _currentClass;

    public TabGradesViewModel(IAssessmentService assessmentService, IGradeService gradeService)
    {
        _assessmentService = assessmentService;
        _gradeService = gradeService;
    }

    public void LoadForClass(SchoolClassDto classDto)
    {
        _currentClass = classDto;
        CurrentClassId = classDto.Id;
        RefreshAssessments();
    }

    private void RefreshAssessments()
    {
        var filtered = AppDataStore.Instance.Assessments
            .Where(a => a.ClassId == CurrentClassId)
            .OrderByDescending(a => a.DateCreated)
            .ToList();
        Assessments = new ObservableCollection<AssessmentDto>(filtered);
    }

    [RelayCommand]
    private void OpenAddAssessmentDialog()
    {
        FormAssessmentName = string.Empty;
        FormMaxScore = 100;
        IsAddAssessmentDialogOpen = true;
    }

    [RelayCommand]
    private void CloseAddAssessmentDialog()
    {
        IsAddAssessmentDialogOpen = false;
    }

    [RelayCommand]
    private async Task AddAssessmentAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FormAssessmentName) || CurrentClassId == Guid.Empty) return;

            var dto = new CreateAssessmentDto
            {
                ClassId = CurrentClassId,
                Name = FormAssessmentName,
                MaxScore = FormMaxScore
            };

            var result = await _assessmentService.CreateAsync(dto);
            if (result.IsSuccess && result.Data is not null)
            {
                AppDataStore.Instance.Assessments.Add(result.Data);
                RefreshAssessments();
            }

            IsAddAssessmentDialogOpen = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to create assessment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task DeleteAssessmentAsync(AssessmentDto? assessment)
    {
        try
        {
            if (assessment is null) return;
            var result = await _assessmentService.DeleteAsync(assessment.Id);
            if (result.IsSuccess)
            {
                AppDataStore.Instance.Assessments.Remove(assessment);
                RefreshAssessments();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to delete assessment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void OpenGradeEditor(AssessmentDto? assessment)
    {
        if (assessment is null) return;
        SelectedAssessment = assessment;
        IsGradeEditorOpen = true;
    }

    [RelayCommand]
    private void CloseGradeEditor()
    {
        IsGradeEditorOpen = false;
        SelectedAssessment = null;
    }
}
