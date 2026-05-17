using Instrux.App.ViewModels;
using System.Windows;

namespace Instrux.App.Views;

public partial class OnboardingWindow : Window
{
    public OnboardingWindow(OnboardingViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
