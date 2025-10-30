using TuiApp = Terminal.Gui.Application;
using Terminal.Gui;
using Loom.UI.Terminal.Input;
using Loom.UI.Terminal.Controllers;
using Loom.Application.DTOs.Tasks;

namespace Loom.UI.Terminal;

public sealed class MainWindow : Window
{
    private readonly TaskListController _controller;
    private readonly ListView _list;


    public MainWindow(TaskListController controller, ListView list)
    {
        _controller = controller;
        _list = list;

        Title = "Loom — Terminal Productivity";
        Border.BorderStyle = LineStyle.None;


        var menu = new MenuBar
        {
            Menus =
            [
                new MenuBarItem("_Tasks",
                [
                    new MenuItem("_Add (a)", "", async () => await _controller.AddTask()),
                    new MenuItem("_Edit (e)", "", async () => await _controller.EditSelectedTask()),
                    new MenuItem("_Delete (d)", "", async () => await _controller.DeleteSelectedTask()),
                    new MenuItem("_Toggle Complete (Space)", "", async () => await _controller.ToggleCompleteSelected()),
                    new MenuItem("_Expand / Collapse (Enter)", "", () => _controller.ToggleExpandCollapse()),
                    new MenuItem("_Refresh (r)", "", async () => await _controller.LoadTasks(_controller.CurrentFilter)),                    new MenuItem("_Filter Tasks (f)", "", async () => await _controller.FilterTasks()),
                    new MenuItem("_All Tasks (A)", "", async () => await _controller.LoadTasks(new TaskFilter())),
                    new MenuItem("_Quit (q)", "", () => TuiApp.Shutdown())
                ])
            ]
        };

        var listFrame = new FrameView("Tasks")
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill(1),
            Height = Dim.Fill(1),
            CanFocus = true,
            BorderStyle = LineStyle.Rounded

        };

        listFrame.Add(_list);
        Add(listFrame);

        new TaskListKeyHandler(_controller, _list).Attach();

        // Load initial data
        _ = _controller.LoadTasks();

        _controller.ViewLabelChanged += label => listFrame.Title = $"Tasks — {label}";
    }
}

