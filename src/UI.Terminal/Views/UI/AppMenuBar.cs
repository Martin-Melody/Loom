using Loom.Application.DTOs.Tasks;
using Loom.Core.Entities;
using Loom.Infrastructure.Persistence;
using Loom.UI.Terminal.Controllers;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Views.UI;

public static class AppMenuBar
{
    public static MenuBar Create(
        TaskListController taskController,
        AppController appController,
        ConfigRepository configRepo
    )
    {
        return new MenuBar
        {
            Menus = new[]
            {
                // --- FILE ---
                new MenuBarItem(
                    "_File",
                    new[] { new MenuItem("_Quit (q)", "", () => TuiApp.RequestStop()) }
                ),
                // --- TASKS ---
                new MenuBarItem(
                    "_Tasks",
                    new[]
                    {
                        new MenuItem("_Add (a)", "", async () => await taskController.AddTask()),
                        new MenuItem(
                            "_Edit (e)",
                            "",
                            async () => await taskController.EditSelectedTask()
                        ),
                        new MenuItem(
                            "_Delete (d)",
                            "",
                            async () => await taskController.DeleteSelectedTask()
                        ),
                        new MenuItem(
                            "_Toggle Complete (Space)",
                            "",
                            async () => await taskController.ToggleCompleteSelected()
                        ),
                        new MenuItem(
                            "_Expand / Collapse (Enter)",
                            "",
                            () => taskController.ToggleExpandCollapse()
                        ),
                        new MenuItem(
                            "_Filter Tasks (f)",
                            "",
                            async () => await taskController.FilterTasks()
                        ),
                        new MenuItem(
                            "_All Tasks (A)",
                            "",
                            async () => await taskController.LoadTasks(new TaskFilter())
                        ),
                    }
                ),
                // --- NAVIGATION ---
                new MenuBarItem(
                    "_Navigation",
                    new[]
                    {
                        new MenuItem(
                            "_Dashboard (Ctrl+D)",
                            "",
                            () => appController.ShowDashboard()
                        ),
                        new MenuItem("_Task List (Ctrl+T)", "", () => appController.ShowTasks()),
                    }
                ),
                // --- SETTINGS ---
                new MenuBarItem(
                    "_Settings",
                    new[]
                    {
                        new MenuItem(
                            "_Save Config",
                            "",
                            async () =>
                            {
                                var config = new AppConfig
                                {
                                    LastOpenView = appController.CurrentViewName,
                                };

                                await configRepo.SaveAsync(config);
                                MessageBox.Query(
                                    "Config Saved",
                                    "Configuration successfully saved!",
                                    "OK"
                                );
                            }
                        ),
                    }
                ),
                // --- TOOLS ---
                new MenuBarItem(
                    "_Tools",
                    new[]
                    {
                        new MenuItem(
                            "_Command Palette (Ctrl+P)",
                            "",
                            async () => await appController.ShowCommandPaletteAsync()
                        ),
                    }
                ),
            },
        };
    }
}
