using Loom.Application.Interfaces;
using Loom.Application.Services;
using Loom.Application.UseCases.Tasks;
using Loom.Core.Entities.Enums;
using Loom.Infrastructure.Persistence;
using Loom.Infrastructure.Persistence.Json;
using Loom.Infrastructure.Registry;
using Loom.Infrastructure.Time;
using Loom.UI.Terminal.Controllers;
using Loom.UI.Terminal.Theme;
using Loom.UI.Terminal.Views.UI;
using Loom.UI.Terminal.Views.Widgets;
using Loom.UI.Terminal.Views.Windows;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal;

public static class Program
{
    public static async Task Main()
    {
        // --- Composition Root (manual DI) ---
        var dataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".loom"
        );

        var configRepo = new ConfigRepository();
        var appState = new AppStateService(configRepo);
        await appState.InitalizeAsync();

        var config = await configRepo.LoadAsync();

        var tasksRepo = new JsonTaskRepository(dataDir);
        IUnitOfWork uow = new JsonUnitOfWork(tasksRepo);
        IDateTimeProvider clock = new SystemClock();

        var createTask = new AddTask(tasksRepo, uow);
        var editTask = new EditTask(tasksRepo, uow);
        var filterTasks = new FilterTasks(tasksRepo, clock);
        var deleteTask = new DeleteTask(tasksRepo, uow);
        var completeTask = new ToggleCompleteTask(tasksRepo, uow);

        // --- Initialise Terminal.Gui ---
        TuiApp.Init();
        LoomTheme.ApplyDarkTheme();

        // --- Build UI ---
        var listView = new ListView
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill() - 2,
            Height = Dim.Fill() - 2,
        };

        var taskController = new TaskListController(
            listView,
            createTask,
            editTask,
            deleteTask,
            completeTask,
            filterTasks
        );

        var widgetManager = new WidgetManager();

        var commandRegistry = new CommandRegistry();

        var taskListWindow = new TaskListWindow(taskController, listView, commandRegistry);
        var dashboardWindow = new DashboardWindow();
        var dayViewWindow = new DayViewWindow();
        var WeekViewWindow = new WeekViewWindow();
        var MonthViewWindow = new MonthViewWindow();
        var YearViewWindow = new YearViewWindow();

        var dashboardController = new DashboardController(widgetManager, dashboardWindow);

        // --- Root View ---
        var top = Toplevel.Create();

        var sidebarView = new SidebarView(commandRegistry);

        var mainContent = new FrameView
        {
            X = Pos.Right(sidebarView),
            Y = 1,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            BorderStyle = LineStyle.None,
        };

        var sidebarController = new SidebarController(sidebarView, mainContent, appState);

        var appController = new AppController(
            dashboardWindow,
            taskListWindow,
            dayViewWindow,
            WeekViewWindow,
            MonthViewWindow,
            YearViewWindow,
            mainContent,
            commandRegistry,
            sidebarController,
            appState
        );
        appController.RegisterCommands(taskController, dashboardController);

        sidebarView.LoadCommands();

        var menuBar = AppMenuBar.Create(commandRegistry);

        top.Add(menuBar, sidebarView, mainContent);

        appController.ShowView(appState.LastOpenView);

        // --- Run ---
        TuiApp.Run(top);
        TuiApp.Shutdown();

        appState.LastOpenView = appController.CurrentView;
        appState.SidebarState = sidebarController.IsVisible
            ? SidebarState.Opened
            : SidebarState.Closed;
        await appState.SaveAsync();
    }
}
