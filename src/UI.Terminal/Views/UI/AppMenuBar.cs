using Loom.Application.Interfaces;
using Loom.Core.Entities;
using Loom.UI.Terminal.Commands;
using Terminal.Gui;

namespace Loom.UI.Terminal.Views.UI;

public static class AppMenuBar
{
    public static MenuBar Create(ICommandRegistry commandRegistry)
    {
        static string FormatMenuLabel(string label, CommandDefinition? command)
        {
            if (command is null || string.IsNullOrWhiteSpace(command.Shortcut))
                return label;

            return $"{label} ({command.Shortcut})";
        }

        MenuItem CreateMenuItem(string label, string commandId)
        {
            var command =
                commandRegistry.GetById(commandId)
                ?? throw new InvalidOperationException($"Command '{commandId}' is not registered");

            return new MenuItem(
                FormatMenuLabel(label, command),
                command.Description ?? string.Empty,
                () => commandRegistry.Execute(commandId)
            );
        }
        return new MenuBar
        {
            Menus = new[]
            {
                // --- FILE ---
                new MenuBarItem("_File", new[] { CreateMenuItem("_Quit", CommandIds.App.Quit) }),
                // --- TASKS ---
                new MenuBarItem(
                    "_Tasks",
                    new[]
                    {
                        CreateMenuItem("_Add", CommandIds.Tasks.Add),
                        CreateMenuItem("_Edit", CommandIds.Tasks.Edit),
                        CreateMenuItem("_Delete", CommandIds.Tasks.Delete),
                        CreateMenuItem("_Toggle Complete", CommandIds.Tasks.ToggleComplete),
                        CreateMenuItem("_Expand / Collapse", CommandIds.Tasks.ToggleExpand),
                        CreateMenuItem("_Filter Tasks", CommandIds.Tasks.Filter),
                        CreateMenuItem("_All Tasks", CommandIds.Tasks.ShowAll),
                    }
                ),
                // --- NAVIGATION ---
                new MenuBarItem(
                    "_Navigation",
                    new[]
                    {
                        CreateMenuItem("_Toggle Sidebar", CommandIds.Navigation.ToggleSidebar),
                        CreateMenuItem("_Dashboard", CommandIds.Navigation.ShowDashboard),
                        CreateMenuItem("_Task List", CommandIds.Navigation.ShowTasks),
                        CreateMenuItem("_Day View", CommandIds.Navigation.ShowDay),
                        CreateMenuItem("_Week View", CommandIds.Navigation.ShowWeek),
                        CreateMenuItem("_Month View", CommandIds.Navigation.ShowMonth),
                        CreateMenuItem("_Year View", CommandIds.Navigation.ShowYear),
                    }
                ),
                // --- SETTINGS ---
                new MenuBarItem(
                    "_Settings",
                    new[] { CreateMenuItem("_Save Config", CommandIds.Settings.SaveConfig) }
                ),
                // --- TOOLS ---
                new MenuBarItem(
                    "_Tools",
                    new[] { CreateMenuItem("_Command Palette", CommandIds.Tools.CommandPalette) }
                ),
            },
        };
    }
}
