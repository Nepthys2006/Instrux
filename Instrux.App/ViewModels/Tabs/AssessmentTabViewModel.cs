using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.App.ViewModels.Tabs;

public partial class AssessmentTabViewModel : ObservableObject
{
    private readonly IAssessmentService _service;
    private readonly ISchoolClassService _classService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<AssessmentDto> _items = [];

    [ObservableProperty]
    private ObservableCollection<SchoolClassDto> _classes = [];

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

    [ObservableProperty]
    private bool _isLoading;

    public string SaveButtonText => IsEditing ? "Update" : "Save";

    public AssessmentTabViewModel(IAssessmentService service, ISchoolClassService classService, INavigationService navigationService)
    {
        _service = service;
        _classService = classService;
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
    private async Task LoadAsync()
    {
        IsLoading = true;
        try
        {
            var result = await _service.GetAllAsync();
            if (result.IsSuccess && result.Data is not null)
                Items = new ObservableCollection<AssessmentDto>(result.Data);

            var classResult = await _classService.GetAllAsync();
            if (classResult.IsSuccess && classResult.Data is not null)
                Classes = new ObservableCollection<SchoolClassDto>(classResult.Data);
        }
        finally { IsLoading = false; }
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
