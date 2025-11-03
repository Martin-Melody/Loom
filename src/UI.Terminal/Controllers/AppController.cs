using Loom.UI.Terminal.Views.Windows;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Controllers;

public class AppController
{
    private readonly DashboardWindow _dashboard;
    private readonly TaskListWindow _taskList;
    private readonly View _mainContent;

    public AppController(DashboardWindow dashboard, TaskListWindow taskList, View mainContent)
    {
        _dashboard = dashboard;
        _taskList = taskList;
        _mainContent = mainContent;

        AddNavigationKeys();
    }

    private void AddNavigationKeys()
    {
        // Switch to Dashboard (Ctrl+D)
        _taskList.KeyPress += (_, args) =>
        {
            if (args.KeyEvent.Key == (Key.CtrlMask | Key.D))
            {
                ShowDashboard();
                args.Handled = true;
            }
        };

        // Switch to TaskList (Ctrl+T)
        _dashboard.KeyPress += (_, args) =>
        {
            if (args.KeyEvent.Key == (Key.CtrlMask | Key.T))
            {
                ShowTasks();
                args.Handled = true;
            }
        };
    }

    public void ShowDashboard()
    {
        _mainContent.RemoveAll();
        _mainContent.Add(_dashboard);
        TuiApp.Refresh();
    }

    public void ShowTasks()
    {
        _mainContent.RemoveAll();
        _mainContent.Add(_taskList);
        TuiApp.Refresh();
    }
}
