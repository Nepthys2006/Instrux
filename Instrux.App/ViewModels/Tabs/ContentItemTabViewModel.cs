using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.App.ViewModels.Tabs;

public partial class ContentItemTabViewModel : ObservableObject
{
    private readonly IContentItemService _service;
    private readonly INavigationService _navigationService;

    public ObservableCollection<ContentItemDto> Items => AppDataStore.Instance.ContentItems;
    public ObservableCollection<SchoolClassDto> Classes => AppDataStore.Instance.SchoolClasses;

    [ObservableProperty]
    private ContentItemDto? _selectedItem;

    [ObservableProperty]
    private Guid _formClassId;

    [ObservableProperty]
    private string _formTitle = string.Empty;

    [ObservableProperty]
    private string _formUrl = string.Empty;

    [ObservableProperty]
    private string _formType = "File";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SaveButtonText))]
    private bool _isEditing;

    public string SaveButtonText => IsEditing ? "Update" : "Save";

    public ContentItemTabViewModel(IContentItemService service, INavigationService navigationService)
    {
        _service = service;
        _navigationService = navigationService;
    }

    partial void OnSelectedItemChanged(ContentItemDto? value)
    {
        if (value is not null)
        {
            FormClassId = value.ClassId;
            FormTitle = value.Title;
            FormUrl = value.Url;
            FormType = value.Type;
            IsEditing = true;
        }
    }

    [RelayCommand]
    private void AddNew()
    {
        FormClassId = Guid.Empty;
        FormTitle = string.Empty;
        FormUrl = string.Empty;
        FormType = "File";
        IsEditing = false;
        SelectedItem = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var dto = new CreateContentItemDto
        {
            ClassId = FormClassId,
            Title = FormTitle,
            Url = FormUrl,
            Type = FormType
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
