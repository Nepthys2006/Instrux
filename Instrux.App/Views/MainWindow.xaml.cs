using Instrux.App.ViewModels;
using Syncfusion.SfSkinManager;
using System;
using System.Windows;

namespace Instrux.App.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        try
        {
            SfSkinManager.SetTheme(this, new Theme("Windows11Light"));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Theme initialization failed: {ex.Message}");
        }
    }
}
