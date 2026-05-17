using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;
using System;
using System.Windows;

namespace Instrux.App.ViewModels.Tabs;

public partial class TabRosterViewModel : ObservableObject
{
    private readonly IStudentService _service;
    private readonly INavigationService _navigationService;

    public ObservableCollection<StudentDto> Items => AppDataStore.Instance.Students;

    [ObservableProperty]
    private StudentDto? _selectedItem;

    [ObservableProperty]
    private string _formName = string.Empty;

    [ObservableProperty]
    private string _formStudentIdentifier = string.Empty;

    [ObservableProperty]
    private string _formEmail = string.Empty;

    [ObservableProperty]
    private bool _isAddDialogOpen;

    [ObservableProperty]
    private Guid _currentClassId;

    private SchoolClassDto? _currentClass;

    public TabRosterViewModel(IStudentService service, INavigationService navigationService)
    {
        _service = service;
        _navigationService = navigationService;
    }

    public void LoadForClass(SchoolClassDto classDto)
    {
        _currentClass = classDto;
        CurrentClassId = classDto.Id;
    }

    [RelayCommand]
    private void OpenAddDialog()
    {
        FormName = string.Empty;
        FormStudentIdentifier = string.Empty;
        FormEmail = string.Empty;
        IsAddDialogOpen = true;
    }

    [RelayCommand]
    private void CloseAddDialog()
    {
        IsAddDialogOpen = false;
    }

    [RelayCommand]
    private async Task AddStudentAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(FormName) || CurrentClassId == Guid.Empty) return;

            var dto = new CreateStudentDto
            {
                ClassId = CurrentClassId,
                Name = FormName,
                StudentIdentifier = FormStudentIdentifier,
                Email = FormEmail
            };

            var result = await _service.CreateAsync(dto);
            if (result.IsSuccess && result.Data is not null)
            {
                AppDataStore.Instance.Students.Add(result.Data);
            }

            IsAddDialogOpen = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to add student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task DeleteStudentAsync(StudentDto? student)
    {
        try
        {
            if (student is null) return;
            var result = await _service.DeleteAsync(student.Id);
            if (result.IsSuccess)
            {
                AppDataStore.Instance.Students.Remove(student);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to delete student: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
