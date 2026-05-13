using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.App.ViewModels.Tabs;

public partial class TodoItemTabViewModel : ObservableObject
{
    private readonly ITodoItemService _service;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ObservableCollection<TodoItemDto> _items = [];

    [ObservableProperty]
    private TodoItemDto? _selectedItem;

    [ObservableProperty]
    private string _formTitle = string.Empty;

    [ObservableProperty]
    private string _formPriority = "Medium";

    [ObservableProperty]
    private DateTime? _formDueDate;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SaveButtonText))]
    private bool _isEditing;

    [ObservableProperty]
    private bool _isLoading;

    public string SaveButtonText => IsEditing ? "Update" : "Save";

    public TodoItemTabViewModel(ITodoItemService service, INavigationService navigationService)
    {
        _service = service;
        _navigationService = navigationService;
    }

    partial void OnSelectedItemChanged(TodoItemDto? value)
    {
        if (value is not null)
        {
            FormTitle = value.Title;
            FormPriority = value.Priority;
            FormDueDate = value.DueDate;
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
                Items = new ObservableCollection<TodoItemDto>(result.Data);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private void AddNew()
    {
        FormTitle = string.Empty;
        FormPriority = "Medium";
        FormDueDate = null;
        IsEditing = false;
        SelectedItem = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var dto = new CreateTodoItemDto
        {
            Title = FormTitle,
            Priority = FormPriority,
            DueDate = FormDueDate
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
