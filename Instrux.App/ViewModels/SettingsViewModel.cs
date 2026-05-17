using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.App.Services;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;
using System;
using System.Windows;

namespace Instrux.App.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ITeacherProfileService _service;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private string _nickname = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private TeacherProfileDto? _profile;

    public SettingsViewModel(ITeacherProfileService service, INavigationService navigationService)
    {
        _service = service;
        _navigationService = navigationService;
        LoadProfile();
    }

    private void LoadProfile()
    {
        Profile = AppDataStore.Instance.TeacherProfiles.FirstOrDefault();
        if (Profile is not null)
        {
            FullName = Profile.FullName;
            Nickname = Profile.Nickname;
            Email = Profile.Email;
        }
    }

    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        try
        {
            var dto = new CreateTeacherProfileDto
            {
                FullName = FullName,
                Nickname = Nickname,
                Email = Email
            };

            if (Profile is not null)
            {
                var result = await _service.UpdateAsync(Profile.Id, dto);
                if (result.IsSuccess && result.Data is not null)
                {
                    var i = AppDataStore.Instance.TeacherProfiles.IndexOf(Profile);
                    if (i >= 0) AppDataStore.Instance.TeacherProfiles[i] = result.Data;
                    Profile = result.Data;
                    StatusMessage = "Profile saved!";
                }
                else
                {
                    StatusMessage = "Failed to save profile.";
                }
            }
            else
            {
                var result = await _service.CreateAsync(dto);
                if (result.IsSuccess && result.Data is not null)
                {
                    AppDataStore.Instance.TeacherProfiles.Add(result.Data);
                    Profile = result.Data;
                    StatusMessage = "Profile created!";
                }
                else
                {
                    StatusMessage = "Failed to create profile.";
                }
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ClearStatus()
    {
        StatusMessage = string.Empty;
    }
}
