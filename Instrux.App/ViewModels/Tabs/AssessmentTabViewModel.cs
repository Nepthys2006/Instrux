using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.App.ViewModels.Tabs;

public partial class AssessmentTabViewModel : ObservableObject
{
    private readonly IAssessmentService _service;
    private readonly INavigationService _navigationService;

    public ObservableCollection<AssessmentDto> Items => AppDataStore.Instance.Assessments;
    public ObservableCollection<SchoolClassDto> Classes => AppDataStore.Instance.SchoolClasses;

    [ObservableProperty]
    private AssessmentDto? _selectedItem;

    [ObservableProperty]
    private Guid _formClassId;

    [ObservableProperty]
    private string _formName = string.Empty;

    [ObservableProperty]
    private double _formMaxScore;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SaveButtonText))]
    private bool _isEditing;

    public string SaveButtonText => IsEditing ? "Update" : "Save";

    public AssessmentTabViewModel(IAssessmentService service, INavigationService navigationService)
    {
        _service = service;
        _navigationService = navigationService;
    }

    partial void OnSelectedItemChanged(AssessmentDto? value)
    {
        if (value is not null)
        {
            FormClassId = value.ClassId;
            FormName = value.Name;
            FormMaxScore = value.MaxScore;
            IsEditing = true;
        }
    }

    [RelayCommand]
    private void AddNew()
    {
        FormClassId = Guid.Empty;
        FormName = string.Empty;
        FormMaxScore = 0;
        IsEditing = false;
        SelectedItem = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var dto = new CreateAssessmentDto
        {
            ClassId = FormClassId,
            Name = FormName,
            MaxScore = FormMaxScore
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
