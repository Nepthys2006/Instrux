using Instrux.App.Data;
using Instrux.App.Services;
using Instrux.App.ViewModels;
using Instrux.App.ViewModels.Tabs;
using Instrux.App.Views;
using Instrux.App.Views.Tabs;
using Instrux.Infrastructure;
using Instrux.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;

namespace Instrux.App.DI;

public static class Bootstrapper
{
    public static IHost BuildHost()
    {
        return Host.CreateDefaultBuilder()
            .UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
            .UseSerilog((context, config) =>
            {
                config.WriteTo.Console();
                config.WriteTo.File(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "instrux-.log"),
                    rollingInterval: RollingInterval.Day);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddApplication();
                services.AddInfrastructure(context.Configuration);

                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<AppDataStore>();

                // Existing tab VMs (needed for admin views)
                services.AddTransient<TeacherProfileTabViewModel>();
                services.AddTransient<SchoolClassTabViewModel>();
                services.AddTransient<StudentTabViewModel>();
                services.AddTransient<AttendanceRecordTabViewModel>();
                services.AddTransient<ContentItemTabViewModel>();
                services.AddTransient<AssessmentTabViewModel>();
                services.AddTransient<GradeTabViewModel>();
                services.AddTransient<CalendarEventTabViewModel>();
                services.AddTransient<TodoItemTabViewModel>();

                // Phase 1 - Foundation
                services.AddSingleton<DashboardViewModel>();

                // Phase 2 - Class Management
                services.AddTransient<TabRosterViewModel>();
                services.AddTransient<TabAttendanceViewModel>();
                services.AddTransient<ClassDetailViewModel>();
                services.AddTransient<ClassManagerViewModel>();

                // Phase 3 - Content & Grades sub-tabs
                services.AddTransient<TabMaterialsViewModel>();
                services.AddTransient<TabGradesViewModel>();

                // Phase 4 - Full-page views
                services.AddSingleton<CalendarViewModel>();
                services.AddSingleton<TodoViewModel>();
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<OnboardingViewModel>();

                // Phase 4 - Windows
                services.AddTransient<OnboardingWindow>();

                // Main
                services.AddTransient<MainViewModel>();
                services.AddTransient<MainWindow>();
            })
            .Build();
    }
}
