using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.App.ViewModels;

public partial class OnboardingViewModel : ObservableObject
{
    private readonly ITeacherProfileService _service;

    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private string _nickname = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private int _currentStep;

    [ObservableProperty]
    private bool _isComplete;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public bool IsLastStep => CurrentStep >= 1;

    public Action? RequestClose { get; set; }

    public OnboardingViewModel(ITeacherProfileService service)
    {
        _service = service;
    }

    [RelayCommand]
    private void NextStep()
    {
        ErrorMessage = string.Empty;

        if (CurrentStep == 0 && string.IsNullOrWhiteSpace(FullName))
        {
            ErrorMessage = "Please enter your full name.";
            return;
        }

        CurrentStep++;
        OnPropertyChanged(nameof(IsLastStep));
    }

    [RelayCommand]
    private void PreviousStep()
    {
        ErrorMessage = string.Empty;
        if (CurrentStep > 0)
            CurrentStep--;
        OnPropertyChanged(nameof(IsLastStep));
    }

    [RelayCommand]
    private async Task FinishAsync()
    {
        try
        {
            var dto = new CreateTeacherProfileDto
            {
                FullName = FullName,
                Nickname = Nickname,
                Email = Email
            };

            var result = await _service.CreateAsync(dto);
            if (result.IsSuccess)
            {
                IsComplete = true;
            }
            else
            {
                ErrorMessage = "Failed to save profile. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private void CloseWindow()
    {
        RequestClose?.Invoke();
    }
}
