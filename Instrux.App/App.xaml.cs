using Instrux.App.Data;
using Instrux.App.DI;
using Instrux.App.ViewModels;
using Instrux.App.Views;
using Instrux.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Syncfusion.Licensing;
using Syncfusion.SfSkinManager;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Instrux.App;

public partial class App : Application
{
    private IHost _host = null!;

    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        try
        {
            SyncfusionLicenseProvider.RegisterLicense("");
            SfSkinManager.ApplyThemeAsDefaultStyle = true;

            _host = Bootstrapper.BuildHost();
            ServiceProvider = _host.Services;

            await InitializeDatabaseAsync();

            var store = ServiceProvider.GetRequiredService<AppDataStore>();
            await store.InitializeAsync();

            // Show onboarding if no profile exists
            if (store.TeacherProfiles.Count == 0)
            {
                var onboardingVm = ServiceProvider.GetRequiredService<OnboardingViewModel>();
                var onboardingWindow = new OnboardingWindow(onboardingVm);
                onboardingVm.RequestClose = () => onboardingWindow.Close();
                onboardingWindow.ShowDialog();
            }

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Application startup failed");
            MessageBox.Show(
                $"Failed to start the application:\n\n{ex.Message}\n\nPlease check the database connection and try again.",
                "Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }
    }

    private static async Task InitializeDatabaseAsync()
    {
        using var scope = ServiceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            if (!await context.Database.CanConnectAsync())
                await context.Database.EnsureCreatedAsync();
            else
                await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Database migration failed, attempting EnsureCreated");
            await context.Database.EnsureCreatedAsync();
        }

        await DbInitializer.SeedAsync(context);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        try
        {
            _host?.Dispose();
        }
        catch { }
        base.OnExit(e);
    }
}
