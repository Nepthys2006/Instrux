using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Instrux.App.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void ShowWindow<T>() where T : Window
    {
        var window = _serviceProvider.GetRequiredService<T>();
        window.Show();
    }

    public bool? ShowDialog<T>() where T : Window
    {
        var window = _serviceProvider.GetRequiredService<T>();
        return window.ShowDialog();
    }
}
