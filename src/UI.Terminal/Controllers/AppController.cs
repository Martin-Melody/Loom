using Loom.Application.Interfaces;
using Loom.Application.Services;
using Loom.Core.Entities.Enums;
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
    private readonly SidebarController _sidebarController;
    private readonly NotificationHistoryController _notificationHistoryController;

    private readonly ICommandRegistry _commands;

    private readonly ViewNavigator _navigator;
    private readonly CommandPaletteController _palette;
    private readonly ConnectionSettingsController _connectionSettings;
    private readonly GlobalShortcutManager _shortcuts;
    private readonly AppStateService _state;

    public ViewType CurrentView => _state.LastOpenView;

    public AppController(
        DashboardWindow dashboard,
        TaskListWindow taskList,
        DayViewWindow dayView,
        WeekViewWindow weekView,
        MonthViewWindow monthView,
        YearViewWindow yearView,
        View mainContent,
        ICommandRegistry commands,
        SidebarController sidebarController,
        NotificationHistoryController notificationHistoryController,
        AppStateService state,
        Action<bool, string> updateStatusBar
    )
    {
        _dashboard = dashboard;
        _taskList = taskList;
        _dayView = dayView;
        _weekView = weekView;
        _monthView = monthView;
        _yearView = yearView;
        _commands = commands;
        _sidebarController = sidebarController;
        _notificationHistoryController = notificationHistoryController;
        _state = state;

        _navigator = new ViewNavigator(mainContent);
        _palette = new CommandPaletteController(commands);
        _connectionSettings = new ConnectionSettingsController(state, updateStatusBar);
        _shortcuts = new GlobalShortcutManager(commands);
    }

    private void Show(ViewType type, View window, string name)
    {
        _navigator.Show(window, name);
        _state.LastOpenView = type;
        _ = _state.SaveAsync(); // fire and forget
    }

    public void ShowDashboard() => Show(ViewType.Dashboard, _dashboard, "Dashboard");

    public void ShowTasks() => Show(ViewType.TaskList, _taskList, "TaskList");

    public void ShowDay() => Show(ViewType.DayView, _dayView, "DayView");

    public void ShowWeek() => Show(ViewType.WeekView, _weekView, "WeekView");

    public void ShowMonth() => Show(ViewType.MonthView, _monthView, "MonthView");

    public void ShowYear() => Show(ViewType.YearView, _yearView, "YearView");

    public void ShowNotificationHistory()
    {
        var win = _notificationHistoryController.CreateWindow();
        Show(ViewType.NotificationHistory, win, "NotificationHistory");
    }

    public void ShowView(ViewType viewType)
    {
        switch (viewType)
        {
            case ViewType.TaskList:
                ShowTasks();
                break;
            case ViewType.DayView:
                ShowDay();
                break;
            case ViewType.WeekView:
                ShowWeek();
                break;
            case ViewType.MonthView:
                ShowMonth();
                break;
            case ViewType.YearView:
                ShowYear();
                break;
            case ViewType.NotificationHistory:
                ShowNotificationHistory();
                break;
            default:
                ShowDashboard();
                break;
        }
    }

    public void ShowCommandPalette() => _palette.Show();

    public void ShowConnectionSettings() => _connectionSettings.Show();

    public void ToggleSidebar() => _sidebarController.Toggle();

    public void FocusSidebar() => _sidebarController.FocusSidebar();

    public void FocusMainContent() => _navigator.FocusCurrentView();

    public async Task SaveAppStateAsync() => await _state.SaveAsync();

    public void RegisterCommands(
        TaskListController taskController,
        DashboardController dashboardController,
        NotificationHistoryController notificationHistoryController
    )
    {
        foreach (var cmd in GlobalCommandDefinitions.Create(this))
            _commands.Register(cmd);

        foreach (
            var cmd in TaskListCommandDefinitions.Create(
                taskController,
                () => CurrentView == ViewType.TaskList
            )
        )
            _commands.Register(cmd);

        foreach (
            var cmd in DashboardCommandDefinitions.Create(
                dashboardController,
                () => CurrentView == ViewType.Dashboard
            )
        )
            _commands.Register(cmd);

        foreach (
            var cmd in NotificationHistoryCommandDefinitions.Create(
                notificationHistoryController,
                () => CurrentView == ViewType.Dashboard
            )
        )
            _commands.Register(cmd);

        _shortcuts.Configure();
    }
}
