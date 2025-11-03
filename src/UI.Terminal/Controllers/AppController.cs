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
        TuiApp.Refresh();
    }

    public void ShowTasks()
    {
        _mainContent.RemoveAll();
        _mainContent.Add(_taskList);
        _taskList.FocusFirst();
        TuiApp.Refresh();
    }
}
