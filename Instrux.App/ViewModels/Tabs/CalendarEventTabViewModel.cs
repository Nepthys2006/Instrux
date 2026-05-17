using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.App.ViewModels.Tabs;

public partial class CalendarEventTabViewModel : ObservableObject
{
    private readonly ICalendarEventService _service;
    private readonly INavigationService _navigationService;

    public ObservableCollection<CalendarEventDto> Items => AppDataStore.Instance.CalendarEvents;

    [ObservableProperty]
    private CalendarEventDto? _selectedItem;

    [ObservableProperty]
    private string _formTitle = string.Empty;

    [ObservableProperty]
    private DateTime _formDate = DateTime.Today;

    [ObservableProperty]
    private string? _formTimeRange;

    [ObservableProperty]
    private string _formCategory = "General";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SaveButtonText))]
    private bool _isEditing;

    public string SaveButtonText => IsEditing ? "Update" : "Save";

    public CalendarEventTabViewModel(ICalendarEventService service, INavigationService navigationService)
    {
        _service = service;
        _navigationService = navigationService;
    }

    partial void OnSelectedItemChanged(CalendarEventDto? value)
    {
        if (value is not null)
        {
            FormTitle = value.Title;
            FormDate = value.Date;
            FormTimeRange = value.TimeRange;
            FormCategory = value.Category;
            IsEditing = true;
        }
    }

    [RelayCommand]
    private void AddNew()
    {
        FormTitle = string.Empty;
        FormDate = DateTime.Today;
        FormTimeRange = null;
        FormCategory = "General";
        IsEditing = false;
        SelectedItem = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var dto = new CreateCalendarEventDto
        {
            Title = FormTitle,
            Date = FormDate,
            TimeRange = FormTimeRange,
            Category = FormCategory
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
