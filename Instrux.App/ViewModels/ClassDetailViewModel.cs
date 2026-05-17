using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Instrux.App.Data;
using Instrux.Services.DTOs;

namespace Instrux.App.ViewModels;

public enum ClassSubTab
{
    Roster,
    Attendance,
    Materials,
    Grades
}

public partial class ClassDetailViewModel : ObservableObject
{
    [ObservableProperty]
    private SchoolClassDto? _classDto;

    [ObservableProperty]
    private ClassSubTab _activeSubTab = ClassSubTab.Roster;

    [ObservableProperty]
    private bool _isRosterSelected = true;

    [ObservableProperty]
    private bool _isAttendanceSelected;

    [ObservableProperty]
    private bool _isMaterialsSelected;

    [ObservableProperty]
    private bool _isGradesSelected;

    public void LoadClass(SchoolClassDto classDto)
    {
        ClassDto = classDto;
        SwitchToSubTab(ClassSubTab.Roster);
    }

    partial void OnActiveSubTabChanged(ClassSubTab value)
    {
        IsRosterSelected = value == ClassSubTab.Roster;
        IsAttendanceSelected = value == ClassSubTab.Attendance;
        IsMaterialsSelected = value == ClassSubTab.Materials;
        IsGradesSelected = value == ClassSubTab.Grades;
    }

    [RelayCommand]
    private void SwitchToSubTab(ClassSubTab tab)
    {
        ActiveSubTab = tab;
    }
}
