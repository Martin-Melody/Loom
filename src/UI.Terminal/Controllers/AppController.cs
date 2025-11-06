using Loom.Application.Interfaces;
using Loom.Infrastructure.Persistence;
using Loom.UI.Terminal.Commands;
using Loom.UI.Terminal.Input;
using Loom.UI.Terminal.Views.Windows;
using Terminal.Gui;

namespace Loom.UI.Terminal.Controllers;

public class AppController
{
    private readonly DashboardWindow _dashboard;
    private readonly TaskListWindow _taskList;
    private readonly DayViewWindow _dayView;
    private readonly WeekViewWindow _weekView;
    private readonly MonthViewWindow _monthView;
    private readonly YearViewWindow _yearView;

    private readonly ICommandRegistry _commands;
    private readonly ViewNavigator _navigator;
    private readonly CommandPaletteController _palette;
    private readonly GlobalShortcutManager _shortcuts;

    public string CurrentViewName => _navigator.CurrentViewName;

    public AppController(
        DashboardWindow dashboard,
        TaskListWindow taskList,
        DayViewWindow dayView,
        WeekViewWindow weekView,
        MonthViewWindow monthView,
        YearViewWindow yearView,
        View mainContent,
        ICommandRegistry commands
    )
    {
        _dashboard = dashboard;
        _taskList = taskList;
        _dayView = dayView;
        _weekView = weekView;
        _monthView = monthView;
        _yearView = yearView;
        _commands = commands;
        _navigator = new ViewNavigator(mainContent);
        _palette = new CommandPaletteController(commands);
        _shortcuts = new GlobalShortcutManager(commands);
    }

    public void ShowDashboard() => _navigator.Show(_dashboard, "Dashboard");

    public void ShowTasks() => _navigator.Show(_taskList, "TaskList");

    public void ShowDay() => _navigator.Show(_dayView, "DayView");

    public void ShowWeek() => _navigator.Show(_weekView, "WeekView");

    public void ShowMonth() => _navigator.Show(_monthView, "MonthView");

    public void ShowYear() => _navigator.Show(_yearView, "YearView");

    public void ShowCommandPalette() => _palette.Show();

    public void RegisterCommands(
        TaskListController taskController,
        DashboardController dashboardController,
        ConfigRepository configRepo
    )
    {
        foreach (var cmd in GlobalCommandDefinitions.Create(this, configRepo))
            _commands.Register(cmd);

        foreach (
            var cmd in TaskListCommandDefinitions.Create(
                taskController,
                () => CurrentViewName == "TaskList"
            )
        )
            _commands.Register(cmd);

        foreach (
            var cmd in DashboardCommandDefinitions.Create(
                dashboardController,
                () => CurrentViewName == "Dashboard"
            )
        )
            _commands.Register(cmd);

        _shortcuts.Configure();
    }
}
