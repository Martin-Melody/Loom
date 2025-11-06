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

        yield return new CommandDefinition(
            CommandIds.Tasks.Add,
            "Add Task",
            "Tasks",
            () => _ = controller.AddTask(),
            "Create a new task.",
            shortcut: "a",
            canExecute: isActive
        );

        yield return new CommandDefinition(
            CommandIds.Tasks.Edit,
            "Edit Task",
            "Tasks",
            () => _ = controller.EditSelectedTask(),
            "Edit the selected task.",
            shortcut: "e",
            canExecute: HasSelection
        );

        yield return new CommandDefinition(
            CommandIds.Tasks.Delete,
            "Delete Task",
            "Tasks",
            () => _ = controller.DeleteSelectedTask(),
            "Delete the selected task.",
            shortcut: "d",
            canExecute: HasSelection
        );

        yield return new CommandDefinition(
            CommandIds.Tasks.ToggleComplete,
            "Toggle Complete",
            "Tasks",
            () => _ = controller.ToggleCompleteSelected(),
            "Toggle the completion state of the selected task.",
            shortcut: "Space",
            canExecute: HasSelection
        );

        yield return new CommandDefinition(
            CommandIds.Tasks.ToggleExpand,
            "Expand/Collapse Details",
            "Tasks",
            controller.ToggleExpandCollapse,
            "Toggle expanded view for the selected task.",
            shortcut: "Enter",
            canExecute: HasSelection
        );

        yield return new CommandDefinition(
            CommandIds.Tasks.Filter,
            "Filter Tasks",
            "Tasks",
            () => _ = controller.FilterTasks(),
            "Apply filters to the task list.",
            shortcut: "f",
            canExecute: isActive
        );

        yield return new CommandDefinition(
            CommandIds.Tasks.ShowAll,
            "Show All Tasks",
            "Tasks",
            () => _ = controller.LoadTasks(new TaskFilter()),
            "Display all tasks.",
            shortcut: "A",
            canExecute: isActive
        );

        yield return new CommandDefinition(
            CommandIds.Tasks.Refresh,
            "Refresh Tasks",
            "Tasks",
            () => _ = controller.LoadTasks(controller.CurrentFilter),
            "Reload the current task view.",
            shortcut: "r",
            canExecute: isActive
        );

        yield return new CommandDefinition(
            CommandIds.Tasks.ShowToday,
            "Today's Tasks",
            "Tasks",
            () => _ = controller.LoadTasks(),
            "Show tasks due today.",
            shortcut: "T",
            canExecute: isActive
        );
    }
}
