using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.App.ViewModels.Tabs;

public partial class StudentTabViewModel : ObservableObject
{
    private readonly IStudentService _service;
    private readonly ISchoolClassService _classService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<StudentDto> _items = [];

    [ObservableProperty]
    private ObservableCollection<SchoolClassDto> _classes = [];

    [ObservableProperty]
    private StudentDto? _selectedItem;

    [ObservableProperty]
    private Guid _formClassId;

    [ObservableProperty]
    private string _formName = string.Empty;

    [ObservableProperty]
    private string _formStudentIdentifier = string.Empty;

    [ObservableProperty]
    private string _formEmail = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SaveButtonText))]
    private bool _isEditing;

    [ObservableProperty]
    private bool _isLoading;

    public string SaveButtonText => IsEditing ? "Update" : "Save";

    public StudentTabViewModel(IStudentService service, ISchoolClassService classService, INavigationService navigationService)
    {
        _service = service;
        _classService = classService;
        _navigationService = navigationService;
    }

    partial void OnSelectedItemChanged(StudentDto? value)
    {
        if (value is not null)
        {
            FormClassId = value.ClassId;
            FormName = value.Name;
            FormStudentIdentifier = value.StudentIdentifier;
            FormEmail = value.Email;
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
                Items = new ObservableCollection<StudentDto>(result.Data);

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
        FormStudentIdentifier = string.Empty;
        FormEmail = string.Empty;
        IsEditing = false;
        SelectedItem = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var dto = new CreateStudentDto
        {
            ClassId = FormClassId,
            Name = FormName,
            StudentIdentifier = FormStudentIdentifier,
            Email = FormEmail
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
