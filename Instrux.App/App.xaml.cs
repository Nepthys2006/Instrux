using Instrux.App.Data;
using Instrux.App.DI;
using Instrux.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Instrux.App;

public partial class App : Application
{
    private IHost _host = null!;

    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        _host = Bootstrapper.BuildHost();
        ServiceProvider = _host.Services;

        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        await DbInitializer.SeedAsync(context);

        var store = ServiceProvider.GetRequiredService<AppDataStore>();
        await store.InitializeAsync();

        var mainWindow = ServiceProvider.GetRequiredService<Views.MainWindow>();
        mainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _host?.Dispose();
        base.OnExit(e);
    }
}
