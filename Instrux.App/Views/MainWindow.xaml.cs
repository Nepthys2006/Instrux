using Instrux.App.ViewModels;
using System.Windows;

namespace Instrux.App.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
