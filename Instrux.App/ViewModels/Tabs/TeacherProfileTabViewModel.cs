using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.App.ViewModels.Tabs;

public partial class TeacherProfileTabViewModel : ObservableObject
{
    private readonly ITeacherProfileService _service;
    private readonly INavigationService _navigationService;

    public ObservableCollection<TeacherProfileDto> Items => AppDataStore.Instance.TeacherProfiles;

    [ObservableProperty]
    private TeacherProfileDto? _selectedItem;

    [ObservableProperty]
    private string _formFullName = string.Empty;

    [ObservableProperty]
    private string _formNickname = string.Empty;

    [ObservableProperty]
    private string _formEmail = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SaveButtonText))]
    private bool _isEditing;

    public string SaveButtonText => IsEditing ? "Update" : "Save";

    public TeacherProfileTabViewModel(ITeacherProfileService service, INavigationService navigationService)
    {
        _service = service;
        _navigationService = navigationService;
    }

    partial void OnSelectedItemChanged(TeacherProfileDto? value)
    {
        if (value is not null)
        {
            FormFullName = value.FullName;
            FormNickname = value.Nickname;
            FormEmail = value.Email;
            IsEditing = true;
        }
    }

    [RelayCommand]
    private void AddNew()
    {
        FormFullName = string.Empty;
        FormNickname = string.Empty;
        FormEmail = string.Empty;
        IsEditing = false;
        SelectedItem = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var dto = new CreateTeacherProfileDto
        {
            FullName = FormFullName,
            Nickname = FormNickname,
            Email = FormEmail
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
