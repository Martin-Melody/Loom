using Loom.Infrastructure.Services;
using Loom.UI.Terminal.Views.Dialogs;
using Loom.UI.Terminal.Views.Windows;
using Terminal.Gui;
using LoomCommand = Loom.Core.Entities.Command;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Controllers;

public class AppController
{
    private readonly DashboardWindow _dashboard;
    private readonly TaskListWindow _taskList;
    private readonly View _mainContent;
    private readonly CommandRegistry _commandRegistry;
    public string CurrentViewName { get; private set; } = "Dashboard";

    public AppController(
        DashboardWindow dashboard,
        TaskListWindow taskList,
        View mainContent,
        CommandRegistry commandRegistry
    )
    {
        _dashboard = dashboard;
        _taskList = taskList;
        _mainContent = mainContent;
        _commandRegistry = commandRegistry;

        AddNavigationKeys();
    }

    private void AddNavigationKeys()
    {
        TuiApp.RootKeyEvent = (keyEvent) =>
        {
            if (keyEvent.Key == (Key.CtrlMask | Key.D))
            {
                ShowDashboard();
                return true;
            }
            else if (keyEvent.Key == (Key.CtrlMask | Key.T))
            {
                ShowTasks();
                return true;
            }

            return false;
        };
    }

    public void ShowDashboard()
    {
        _mainContent.RemoveAll();
        _mainContent.Add(_dashboard);
        _dashboard.FocusFirst();
        CurrentViewName = "Dashboard";
        TuiApp.Refresh();
    }

    public void ShowTasks()
    {
        _mainContent.RemoveAll();
        _mainContent.Add(_taskList);
        _taskList.FocusFirst();
        CurrentViewName = "TaskList";
        TuiApp.Refresh();
    }

    public async Task ShowCommandPaletteAsync()
    {
        var dlg = new CommandPaletteDialog(_commandRegistry);
        TuiApp.Run(dlg);
    }

    public void RegisterCommands(TaskListController taskController)
    {
        _commandRegistry.Register(
            new LoomCommand("Open Dashboard", "Navigation", async () => ShowDashboard())
        );
        _commandRegistry.Register(
            new LoomCommand("Open Task List", "Navigation", async () => ShowTasks())
        );
        _commandRegistry.Register(
            new LoomCommand("Add Task", "Tasks", async () => await taskController.AddTask())
        );
    }
}
