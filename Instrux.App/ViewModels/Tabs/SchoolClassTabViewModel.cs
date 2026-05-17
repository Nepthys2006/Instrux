using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.App.ViewModels.Tabs;

public partial class SchoolClassTabViewModel : ObservableObject
{
    private readonly ISchoolClassService _service;
    private readonly INavigationService _navigationService;

    public ObservableCollection<SchoolClassDto> Items => AppDataStore.Instance.SchoolClasses;

    [ObservableProperty]
    private SchoolClassDto? _selectedItem;

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
    [NotifyPropertyChangedFor(nameof(SaveButtonText))]
    private bool _isEditing;

    public string SaveButtonText => IsEditing ? "Update" : "Save";

    public SchoolClassTabViewModel(ISchoolClassService service, INavigationService navigationService)
    {
        _service = service;
        _navigationService = navigationService;
    }

    partial void OnSelectedItemChanged(SchoolClassDto? value)
    {
        if (value is not null)
        {
            FormName = value.Name;
            FormSubject = value.Subject;
            FormSection = value.Section;
            FormTerm = value.Term;
            FormColorHex = value.ColorHex;
            IsEditing = true;
        }
    }

    [RelayCommand]
    private void AddNew()
    {
        FormName = string.Empty;
        FormSubject = string.Empty;
        FormSection = string.Empty;
        FormTerm = string.Empty;
        FormColorHex = "#4F46E5";
        IsEditing = false;
        SelectedItem = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var dto = new CreateSchoolClassDto
        {
            Name = FormName,
            Subject = FormSubject,
            Section = FormSection,
            Term = FormTerm,
            ColorHex = FormColorHex
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
