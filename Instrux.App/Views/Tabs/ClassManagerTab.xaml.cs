using System.Windows;
using System.Windows.Controls;
using Instrux.App.ViewModels;
using Instrux.Services.DTOs;

namespace Instrux.App.Views.Tabs;

public partial class ClassManagerTab : UserControl
{
    public ClassManagerTab()
    {
        InitializeComponent();
    }

    private void NavigateToClassDetail(SchoolClassDto classDto)
    {
        if (DataContext is ClassManagerViewModel vm)
        {
            vm.NavigateToClassDetail(classDto);
        }
    }
}
