using Loom.Application.Interfaces;
using Loom.Application.UseCases.Tasks;
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

        var dashboardController = new DashboardController(widgetManager, dashboardWindow);

        // --- Root View ---
        var top = Toplevel.Create();

        var mainContent = new FrameView
        {
            X = 0,
            Y = 1,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            BorderStyle = LineStyle.None,
        };

        var appController = new AppController(
            dashboardWindow,
            taskListWindow,
            mainContent,
            commandRegistry
        );
        appController.RegisterCommands(taskController, dashboardController, configRepo);

        var menuBar = AppMenuBar.Create(commandRegistry);

        top.Add(menuBar, mainContent);

        // --- Start with last open view ---
        if (config.LastOpenView == nameof(TaskListWindow).Replace("Window", ""))
            appController.ShowTasks();
        else
            appController.ShowDashboard();

        // --- Run ---
        TuiApp.Run(top);
        TuiApp.Shutdown();

        // --- Save config on exit ---
        config.LastOpenView = appController.CurrentViewName;
        await configRepo.SaveAsync(config);
    }
}
