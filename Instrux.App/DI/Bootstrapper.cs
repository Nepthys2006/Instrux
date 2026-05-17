using Instrux.App.Data;
using Instrux.App.Services;
using Instrux.App.ViewModels;
using Instrux.App.ViewModels.Tabs;
using Instrux.App.Views;
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

                services.AddTransient<TeacherProfileTabViewModel>();
                services.AddTransient<SchoolClassTabViewModel>();
                services.AddTransient<StudentTabViewModel>();
                services.AddTransient<AttendanceRecordTabViewModel>();
                services.AddTransient<ContentItemTabViewModel>();
                services.AddTransient<AssessmentTabViewModel>();
                services.AddTransient<GradeTabViewModel>();
                services.AddTransient<CalendarEventTabViewModel>();
                services.AddTransient<TodoItemTabViewModel>();

                services.AddTransient<MainViewModel>();
                services.AddTransient<MainWindow>();
            })
            .Build();
    }
}
