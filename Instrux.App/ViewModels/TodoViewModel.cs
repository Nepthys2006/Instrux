using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;
using System;
using System.Windows;

namespace Instrux.App.ViewModels;

public partial class TodoViewModel : ObservableObject
{
    private readonly ITodoItemService _service;

    [ObservableProperty]
    private ObservableCollection<TodoItemDto> _pendingItems = [];

    [ObservableProperty]
    private ObservableCollection<TodoItemDto> _completedItems = [];

    [ObservableProperty]
    private string _filterPriority = "All";

    [ObservableProperty]
    private string _formTitle = string.Empty;

    [ObservableProperty]
    private string _formPriority = "Medium";

    [ObservableProperty]
    private DateTime? _formDueDate;

    [ObservableProperty]
    private bool _isAddDialogOpen;

    public List<string> PriorityOptions { get; } = ["All", "High", "Medium", "Low"];
    public List<string> FormPriorityOptions { get; } = ["High", "Medium", "Low"];

    public TodoViewModel(ITodoItemService service)
    {
        _service = service;
        RefreshItems();
    }

    partial void OnFilterPriorityChanged(string value)
    {
        RefreshItems();
    }

    public void RefreshItems()
    {
        var all = AppDataStore.Instance.TodoItems.AsEnumerable();

        if (FilterPriority != "All")
            all = all.Where(t => t.Priority == FilterPriority);

        PendingItems = new ObservableCollection<TodoItemDto>(
            all.Where(t => !t.IsCompleted)
               .OrderByDescending(t => t.Priority == "High")
               .ThenByDescending(t => t.Priority == "Medium")
               .ThenBy(t => t.DueDate));

        CompletedItems = new ObservableCollection<TodoItemDto>(
            all.Where(t => t.IsCompleted)
               .OrderByDescending(t => t.DueDate));
    }

    [RelayCommand]
    private void OpenAddDialog()
    {
        FormTitle = string.Empty;
        FormPriority = "Medium";
        FormDueDate = null;
        IsAddDialogOpen = true;
    }

    [RelayCommand]
    private void CloseAddDialog()
    {
        IsAddDialogOpen = false;
    }

    [RelayCommand]
    private async Task AddItemAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FormTitle)) return;

            var dto = new CreateTodoItemDto
            {
                Title = FormTitle,
                Priority = FormPriority,
                DueDate = FormDueDate
            };

            var result = await _service.CreateAsync(dto);
            if (result.IsSuccess && result.Data is not null)
            {
                AppDataStore.Instance.TodoItems.Add(result.Data);
                RefreshItems();
            }

            IsAddDialogOpen = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to add task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task ToggleCompleteAsync(TodoItemDto? item)
    {
        try
        {
            if (item is null) return;

            var dto = new CreateTodoItemDto
            {
                Title = item.Title,
                Priority = item.Priority,
                DueDate = item.DueDate
            };

            item.IsCompleted = !item.IsCompleted;
            var result = await _service.UpdateAsync(item.Id, dto);
            if (result.IsSuccess && result.Data is not null)
            {
                var i = AppDataStore.Instance.TodoItems.IndexOf(item);
                if (i >= 0) AppDataStore.Instance.TodoItems[i] = result.Data;
                RefreshItems();
            }
            else
            {
                RefreshItems();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task DeleteItemAsync(TodoItemDto? item)
    {
        try
        {
            if (item is null) return;
            var result = await _service.DeleteAsync(item.Id);
            if (result.IsSuccess)
            {
                AppDataStore.Instance.TodoItems.Remove(item);
                RefreshItems();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to delete task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
