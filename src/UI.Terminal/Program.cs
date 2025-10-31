using Loom.Application.Interfaces;
using Loom.Application.UseCases.Tasks;
using Loom.Infrastructure.Persistence.Json;
using Loom.Infrastructure.Time;
using Loom.UI.Terminal.Controllers;
using Loom.UI.Terminal.Theme;
using Loom.UI.Terminal.Windows;
using Terminal.Gui;

using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal;

public static class Program
{
    public static void Main()
    {
        // --- Composition Root (manual DI) ---
        var dataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".loom"
        );

        var tasksRepo = new JsonTaskRepository(dataDir);
        IUnitOfWork uow = new JsonUnitOfWork(tasksRepo);
        IDateTimeProvider clock = new SystemClock();

        var createTask = new CreateTask(tasksRepo, uow);
        var editTask = new EditTask(tasksRepo, uow);
        var filterTasks = new FilterTasks(tasksRepo, clock);
        var deleteTask = new DeleteTask(tasksRepo, uow);
        var completeTask = new ToggleCompleteTask(tasksRepo, uow);

        // --- Initialise Terminal.Gui ---
        TuiApp.Init();
        LoomTheme.ApplyDarkTheme();
        TuiApp.Run(new DashboardWindow());

        // --- Build UI ---
        // var listView = new ListView
        // {
        //     X = 1,
        //     Y = 1,
        //     Width = Dim.Fill() - 2,
        //     Height = Dim.Fill() - 2
        // };
        //
        // var controller = new TaskListController(
        //     listView,
        //     createTask,
        //     editTask,
        //     deleteTask,
        //     completeTask,
        //     filterTasks
        // );
        //
        // var window = new MainWindow(controller, listView);

        // --- Run ---
        // TuiApp.Run(window);
        TuiApp.Shutdown();
    }
}

