using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;
using System;
using System.Windows;

namespace Instrux.App.ViewModels;

public partial class CalendarViewModel : ObservableObject
{
    private readonly ICalendarEventService _service;

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    [ObservableProperty]
    private ObservableCollection<CalendarEventDto> _todayEvents = [];

    [ObservableProperty]
    private ObservableCollection<CalendarEventDto> _upcomingEvents = [];

    [ObservableProperty]
    private ObservableCollection<CalendarEventDto> _filteredEvents = [];

    [ObservableProperty]
    private string _filterCategory = "All";

    [ObservableProperty]
    private string _formTitle = string.Empty;

    [ObservableProperty]
    private DateTime _formDate = DateTime.Today;

    [ObservableProperty]
    private string? _formTimeRange;

    [ObservableProperty]
    private string _formCategory = "General";

    [ObservableProperty]
    private bool _isAddDialogOpen;

    public List<string> CategoryOptions { get; } = ["All", "General", "Class", "Meeting", "Deadline", "Personal"];

    public CalendarViewModel(ICalendarEventService service)
    {
        _service = service;
        RefreshEvents();
    }

    partial void OnFilterCategoryChanged(string value)
    {
        RefreshEvents();
    }

    public void RefreshEvents()
    {
        var allEvents = AppDataStore.Instance.CalendarEvents.AsEnumerable();

        if (FilterCategory != "All")
            allEvents = allEvents.Where(e => e.Category == FilterCategory);

        var ordered = allEvents.OrderBy(e => e.Date).ToList();

        TodayEvents = new ObservableCollection<CalendarEventDto>(
            ordered.Where(e => e.Date.Date == DateTime.Today.Date));

        UpcomingEvents = new ObservableCollection<CalendarEventDto>(
            ordered.Where(e => e.Date.Date > DateTime.Today.Date));

        FilteredEvents = new ObservableCollection<CalendarEventDto>(ordered);
    }

    [RelayCommand]
    private void OpenAddDialog()
    {
        FormTitle = string.Empty;
        FormDate = DateTime.Today;
        FormTimeRange = null;
        FormCategory = "General";
        IsAddDialogOpen = true;
    }

    [RelayCommand]
    private void CloseAddDialog()
    {
        IsAddDialogOpen = false;
    }

    [RelayCommand]
    private async Task AddEventAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FormTitle)) return;

            var dto = new CreateCalendarEventDto
            {
                Title = FormTitle,
                Date = FormDate,
                TimeRange = FormTimeRange,
                Category = FormCategory
            };

            var result = await _service.CreateAsync(dto);
            if (result.IsSuccess && result.Data is not null)
            {
                AppDataStore.Instance.CalendarEvents.Add(result.Data);
                RefreshEvents();
            }

            IsAddDialogOpen = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to add event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task DeleteEventAsync(CalendarEventDto? evt)
    {
        try
        {
            if (evt is null) return;
            var result = await _service.DeleteAsync(evt.Id);
            if (result.IsSuccess)
            {
                AppDataStore.Instance.CalendarEvents.Remove(evt);
                RefreshEvents();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to delete event: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
