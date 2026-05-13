using Instrux.App.ViewModels;
using System.Windows;

namespace Instrux.App.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += async (_, _) =>
        {
            await viewModel.TeacherProfileTab.LoadCommand.ExecuteAsync(null);
            await viewModel.SchoolClassTab.LoadCommand.ExecuteAsync(null);
            await viewModel.StudentTab.LoadCommand.ExecuteAsync(null);
            await viewModel.AttendanceRecordTab.LoadCommand.ExecuteAsync(null);
            await viewModel.ContentItemTab.LoadCommand.ExecuteAsync(null);
            await viewModel.AssessmentTab.LoadCommand.ExecuteAsync(null);
            await viewModel.GradeTab.LoadCommand.ExecuteAsync(null);
            await viewModel.CalendarEventTab.LoadCommand.ExecuteAsync(null);
            await viewModel.TodoItemTab.LoadCommand.ExecuteAsync(null);
        };
    }
}
