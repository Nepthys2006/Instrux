using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;
using System;
using System.Windows;

namespace Instrux.App.ViewModels.Tabs;

public partial class TabMaterialsViewModel : ObservableObject
{
    private readonly IContentItemService _service;

    [ObservableProperty]
    private Guid _currentClassId;

    [ObservableProperty]
    private ObservableCollection<ContentItemDto> _classItems = [];

    [ObservableProperty]
    private string _formTitle = string.Empty;

    [ObservableProperty]
    private string _formUrl = string.Empty;

    [ObservableProperty]
    private string _formType = "Link";

    [ObservableProperty]
    private bool _isAddDialogOpen;

    public List<string> TypeOptions { get; } = ["Link", "File", "Video", "Document"];

    private SchoolClassDto? _currentClass;

    public TabMaterialsViewModel(IContentItemService service)
    {
        _service = service;
    }

    public void LoadForClass(SchoolClassDto classDto)
    {
        _currentClass = classDto;
        CurrentClassId = classDto.Id;
        RefreshItems();
    }

    private void RefreshItems()
    {
        var filtered = AppDataStore.Instance.ContentItems
            .Where(c => c.ClassId == CurrentClassId)
            .OrderByDescending(c => c.CreatedAt)
            .ToList();
        ClassItems = new ObservableCollection<ContentItemDto>(filtered);
    }

    [RelayCommand]
    private void OpenAddDialog()
    {
        FormTitle = string.Empty;
        FormUrl = string.Empty;
        FormType = "Link";
        IsAddDialogOpen = true;
    }

    [RelayCommand]
    private void CloseAddDialog()
    {
        IsAddDialogOpen = false;
    }

    [RelayCommand]
    private async Task AddMaterialAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FormTitle) || CurrentClassId == Guid.Empty) return;

            var dto = new CreateContentItemDto
            {
                ClassId = CurrentClassId,
                Title = FormTitle,
                Url = FormUrl,
                Type = FormType
            };

            var result = await _service.CreateAsync(dto);
            if (result.IsSuccess && result.Data is not null)
            {
                AppDataStore.Instance.ContentItems.Add(result.Data);
                RefreshItems();
            }

            IsAddDialogOpen = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to add material: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void OpenLink(ContentItemDto? item)
    {
        if (item is null || string.IsNullOrWhiteSpace(item.Url)) return;
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = item.Url,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }
        catch { }
    }

    [RelayCommand]
    private async Task DeleteMaterialAsync(ContentItemDto? item)
    {
        try
        {
            if (item is null) return;
            var result = await _service.DeleteAsync(item.Id);
            if (result.IsSuccess)
            {
                AppDataStore.Instance.ContentItems.Remove(item);
                ClassItems.Remove(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to delete material: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
