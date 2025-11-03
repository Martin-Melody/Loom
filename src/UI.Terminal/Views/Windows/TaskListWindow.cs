using Loom.UI.Terminal.Controllers;
using Loom.UI.Terminal.Input;
using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Windows;

public sealed class TaskListWindow : Window
{
    private readonly TaskListController _controller;
    private readonly ListView _list;

    public TaskListWindow(TaskListController controller, ListView list)
    {
        _controller = controller;
        _list = list;

        Title = "Loom - Task List";
        Border.BorderStyle = LineStyle.None;

        var listFrame = new FrameView("Tasks")
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill(1),
            Height = Dim.Fill(1) - 1,
            CanFocus = true,
            BorderStyle = LineStyle.Rounded,
        };

        listFrame.Add(_list);
        Add(listFrame);

        new TaskListKeyHandler(_controller, _list).Attach();
        _ = _controller.LoadTasks();

        _controller.ViewLabelChanged += label => listFrame.Title = $"Tasks — {label}";
    }
}
