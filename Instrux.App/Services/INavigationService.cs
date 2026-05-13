using System.Windows;

namespace Instrux.App.Services;

public interface INavigationService
{
    void ShowWindow<T>() where T : Window;
    bool? ShowDialog<T>() where T : Window;
}
