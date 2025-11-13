using Loom.Application.DTOs.Tasks;
using Loom.Core.Entities;
using Loom.UI.Terminal.Commands;

namespace Loom.UI.Terminal.Controllers;

public static class TaskListCommandDefinitions
{
    public static IEnumerable<CommandDefinition> Create(
        TaskListController controller,
        Func<bool> isActive
    )
    {
        bool HasSelection() => isActive() && controller.HasSelection;

        // --- Add Task ---
        yield return new CommandDefinition(
            CommandIds.Tasks.Add,
            "Add Task",
            "Tasks",
            () => controller.AddTaskAsync().FireAndForget(),
            "Create a new task.",
            shortcut: "a",
            canExecute: isActive
        );

        // --- Edit Task ---
        yield return new CommandDefinition(
            CommandIds.Tasks.Edit,
            "Edit Task",
            "Tasks",
            () =>
            {
                if (controller.SelectedTask is not null)
                    controller.EditTaskAsync(controller.SelectedTask).FireAndForget();
            },
            "Edit the selected task.",
            shortcut: "e",
            canExecute: HasSelection
        );

        // --- Delete Task ---
        yield return new CommandDefinition(
            CommandIds.Tasks.Delete,
            "Delete Task",
            "Tasks",
            () =>
            {
                if (controller.SelectedTask is not null)
                    controller.DeleteTaskAsync(controller.SelectedTask).FireAndForget();
            },
            "Delete the selected task.",
            shortcut: "d",
            canExecute: HasSelection
        );

        // --- Toggle Complete ---
        yield return new CommandDefinition(
            CommandIds.Tasks.ToggleComplete,
            "Toggle Complete",
            "Tasks",
            () =>
            {
                if (controller.SelectedTask is not null)
                    controller.ToggleCompleteAsync(controller.SelectedTask).FireAndForget();
            },
            "Toggle completion state for the selected task.",
            shortcut: "Space",
            canExecute: HasSelection
        );

        // --- Expand / Collapse ---
        yield return new CommandDefinition(
            CommandIds.Tasks.ToggleExpand,
            "Expand / Collapse",
            "Tasks",
            () => controller.ToggleExpandCollapse(),
            "Expand or collapse the selected task’s details.",
            shortcut: "Enter",
            canExecute: HasSelection
        );

        // --- Filter ---
        yield return new CommandDefinition(
            CommandIds.Tasks.Filter,
            "Filter Tasks",
            "Tasks",
            () => controller.FilterTasksAsync().FireAndForget(),
            "Apply filters to the task list.",
            shortcut: "f",
            canExecute: isActive
        );

        // --- Show All ---
        yield return new CommandDefinition(
            CommandIds.Tasks.ShowAll,
            "Show All Tasks",
            "Tasks",
            () => controller.LoadTasksAsync(new TaskFilter()).FireAndForget(),
            "Show all tasks.",
            shortcut: "A",
            canExecute: isActive
        );

        // --- Refresh ---
        yield return new CommandDefinition(
            CommandIds.Tasks.Refresh,
            "Refresh Tasks",
            "Tasks",
            () => controller.LoadTasksAsync(controller.CurrentFilter).FireAndForget(),
            "Reload current task view.",
            shortcut: "r",
            canExecute: isActive
        );

        // --- Show Today ---
        yield return new CommandDefinition(
            CommandIds.Tasks.ShowToday,
            "Today’s Tasks",
            "Tasks",
            () => controller.LoadTasksAsync().FireAndForget(),
            "Show tasks due today.",
            shortcut: "T",
            canExecute: isActive
        );
    }

    // Utility helper for safely fire-and-forget async actions
    private static void FireAndForget(this Task task)
    {
        _ = task.ContinueWith(
            t =>
            {
                if (t.Exception is not null)
                    Console.Error.WriteLine(t.Exception);
            },
            TaskContinuationOptions.OnlyOnFaulted
        );
    }
}
