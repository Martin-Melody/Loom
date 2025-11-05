using Loom.Application.Interfaces;
using Loom.Core.Entities;
using Loom.UI.Terminal.Controllers;

namespace Loom.UI.Terminal.Commands;

public class TaskCommandProvider : ICommandProvider
{
    private readonly TaskListController _controller;

    public TaskCommandProvider(TaskListController controller)
    {
        _controller = controller;
    }

    public IEnumerable<Command> GetCommands()
    {
        yield return new Command("task.add", "Add New Task", "Tasks", _controller.AddTask, "a");

        yield return new Command(
            "task.edit",
            "Edit Selected Task",
            "Tasks",
            _controller.EditSelectedTask
        );

        yield return new Command(
            "task.delete",
            "Delete Selected Task",
            "Tasks",
            _controller.DeleteSelectedTask
        );

        yield return new Command(
            "task.refresh",
            "Reload Tasks",
            "Tasks",
            () => _controller.LoadTasks()
        );
    }
}
