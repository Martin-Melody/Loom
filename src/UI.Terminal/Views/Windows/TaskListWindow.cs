using Loom.Application.DTOs.Tasks;
using Loom.Application.Interfaces;
using Loom.Core.Entities;
using Loom.UI.Terminal.Controllers;
using Loom.UI.Terminal.Input;
using Loom.UI.Terminal.Views.Dialogs;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Views.Windows;

public sealed class TaskListWindow : Window
{
    private readonly TaskListController _controller;
    private readonly ICommandRegistry _commandRegistry;

    private readonly FrameView _listFrame;
    private readonly ListView _list;

    public TaskListWindow(TaskListController controller, ICommandRegistry commandRegistry)
    {
        _controller = controller;
        _commandRegistry = commandRegistry;

        Title = "Loom - Task List";
        Border.BorderStyle = LineStyle.None;

        // --- Layout ---
        _list = new ListView
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = true,
        };

        // Keep controller’s selected index in sync with UI
        _list.SelectedItemChanged += (_, args) =>
        {
            _controller.SetSelectedIndex(args.Item);
        };

        _listFrame = new FrameView("Tasks")
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill(1),
            Height = Dim.Fill(1) - 1,
            BorderStyle = LineStyle.Rounded,
        };
        _listFrame.Add(_list);
        Add(_listFrame);

        // --- Bind controller events ---
        _controller.TasksUpdated += OnTasksUpdated;
        _controller.ViewLabelChanged += label => _listFrame.Title = $"Tasks — {label}";

        // --- Supply UI delegates for controller ---
        _controller.RequestTaskAdd += OnTaskAddRequested;
        _controller.RequestTaskEdit += OnTaskEditRequested;
        _controller.RequestTaskFilter += OnTaskFilterRequested;
        _controller.RequestConfirmDelete += OnConfirmDelete;

        // --- Key / Command handling ---
        new TaskListKeyHandler(_list, commandRegistry).Attach();

        // --- Initial load ---
        _ = _controller.LoadTasksAsync();
    }

    // --- Update the list safely while preserving selection ---
    private void OnTasksUpdated(IReadOnlyList<TaskView> views)
    {
        var rendered = views.SelectMany(v => v.RenderLines()).ToList();

        _list.SetSource(rendered);

        if (_controller.HasSelection)
        {
            _list.SelectedItem = Math.Min(
                _controller.SelectedIndex,
                rendered.Count > 0 ? rendered.Count - 1 : 0
            );
        }

        _list.SetNeedsDisplay();
        _list.SetFocus();
        ;
    }

    private AddTaskRequest? OnTaskAddRequested(TaskItem? _)
    {
        var dlg = new AddTaskDialog();
        TuiApp.Run(dlg);
        return dlg.TaskCreated ? dlg.Result : null;
    }

    private EditTaskRequest? OnTaskEditRequested(TaskItem original)
    {
        var dlg = new EditTaskDialog(original);
        TuiApp.Run(dlg);
        return dlg.TaskUpdated ? dlg.Result : null;
    }

    private TaskFilter? OnTaskFilterRequested(TaskFilter? current)
    {
        var dlg = new FilterTasksDialog();
        TuiApp.Run(dlg);
        return dlg.Applied ? dlg.Filter : null;
    }

    private bool OnConfirmDelete(string title, string msg)
    {
        var result = MessageBox.Query(title, msg, "Yes", "No");
        return result == 0;
    }
}
