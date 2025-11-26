using Loom.Application.Interfaces;
using Loom.Application.Services;
using Loom.CLI;
using Loom.Core.Entities.Enums;
using Loom.Infrastructure.Persistence;
using Loom.Infrastructure.Persistence.Json;
using Loom.Infrastructure.Persistence.Providers;
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
    public static async Task Main(string[] args)
    {
        // --- CLI Invocation ---
        if (args.Length > 0)
        {
            CliEntryPoint.Run(args);
            return;
        }

        // --- Load Config ---
        var configRepo = new ConfigRepository();
        var appState = new AppStateService(configRepo);
        await appState.InitalizeAsync();

        var config = appState.Current;

        // Determine data directory
        var dataDir = string.IsNullOrWhiteSpace(config.DataDirectory)
            ? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".loom"
            )
            : config.DataDirectory;

        // --- Choose Storage Provider (Local or Remote) ---
        IStorageProvider storageProvider;
        IDateTimeProvider clock = new SystemClock();

        switch (config.Mode)
        {
            case ConnectionMode.Remote:
                if (string.IsNullOrWhiteSpace(config.ServerUrl))
                    throw new InvalidOperationException(
                        "ConnectionMode.Remote requires a ServerUrl in config.json"
                    );

                var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                storageProvider = new RemoteApiProvider(http, config.ServerUrl!, config.ApiKey);
                break;

            case ConnectionMode.Local:
            default:
                var tasksRepo = new JsonTaskRepository(dataDir);
                IUnitOfWork uow = new JsonUnitOfWork(tasksRepo);
                storageProvider = new LocalStorageProvider(tasksRepo, uow);
                break;
        }

        // --- Application Service Layer ---
        ITaskService taskService = new TaskService(storageProvider, clock);

        // --- Initialize Terminal UI ---
        TuiApp.Init();
        LoomTheme.ApplyDarkTheme();

        // --- Controllers ---
        var taskController = new TaskListController(taskService);
        var widgetManager = new WidgetManager();
        var commandRegistry = new CommandRegistry();

        var dashboardWindow = new DashboardWindow();
        var taskListWindow = new TaskListWindow(taskController, commandRegistry);
        var dayViewWindow = new DayViewWindow();
        var weekViewWindow = new WeekViewWindow();
        var monthViewWindow = new MonthViewWindow();
        var yearViewWindow = new YearViewWindow();

        var dashboardController = new DashboardController(widgetManager, dashboardWindow);

        // --- Root View Setup ---
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
            weekViewWindow,
            monthViewWindow,
            yearViewWindow,
            mainContent,
            commandRegistry,
            sidebarController,
            appState
        );

        // Register commands + load sidebar
        appController.RegisterCommands(taskController, dashboardController);
        sidebarView.LoadCommands();

        // --- Menu bar ---
        var menuBar = AppMenuBar.Create(commandRegistry);
        top.Add(menuBar, sidebarView, mainContent);

        // --- Load last view ---
        appController.ShowView(appState.LastOpenView);

        // --- Run the App ---
        TuiApp.Run(top);
        TuiApp.Shutdown();

        // --- Persist App State ---
        appState.LastOpenView = appController.CurrentView;
        appState.SidebarState = sidebarController.IsVisible
            ? SidebarState.Opened
            : SidebarState.Closed;

        await appState.SaveAsync();
    }
}
