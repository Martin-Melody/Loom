using Loom.Application.Interfaces;
using Loom.Infrastructure.Persistence;
using Loom.UI.Terminal.Commands;
using Loom.UI.Terminal.Input;
using Loom.UI.Terminal.Views.Dialogs;
using Loom.UI.Terminal.Views.Windows;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Controllers;

public class AppController
{
    private readonly DashboardWindow _dashboard;
    private readonly TaskListWindow _taskList;
    private readonly ICommandRegistry _commands;
    private readonly ViewNavigator _navigator;
    private readonly CommandPaletteController _palette;
    private readonly GlobalShortcutManager _shortcuts;

    public string CurrentViewName => _navigator.CurrentViewName;

    public AppController(
        DashboardWindow dashboard,
        TaskListWindow taskList,
        View mainContent,
        ICommandRegistry commands
    )
    {
        _dashboard = dashboard;
        _taskList = taskList;
        _commands = commands;
        _navigator = new ViewNavigator(mainContent);
        _palette = new CommandPaletteController(commands);
        _shortcuts = new GlobalShortcutManager(commands);
    }

    public void ShowDashboard() => _navigator.Show(_dashboard, "Dashboard");

    public void ShowTasks() => _navigator.Show(_taskList, "TaskList");

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
