using Loom.Application.DTOs.Tasks;
using Loom.UI.Terminal.Controllers;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Input;

public class TaskListKeyHandler
{
    private readonly TaskListController _controller;
    private readonly ListView _list;

    public TaskListKeyHandler(TaskListController controller, ListView list)
    {
        _controller = controller;
        _list = list;
    }

    public void Attach()
    {
        _list.KeyDown += async (_, args) =>
        {
            var key = args.KeyEvent.Key;

            switch (key)
            {
                // --- Existing shortcuts ---
                case Key.Space:
                    args.Handled = true;
                    await _controller.ToggleCompleteSelected();
                    break;

                case Key.Enter:
                    args.Handled = true;
                    _controller.ToggleExpandCollapse();
                    break;

                case Key.a:
                    args.Handled = true;
                    await _controller.AddTask();
                    break;

                case Key.A:
                    args.Handled = true;
                    await _controller.LoadTasks(new TaskFilter());
                    break;

                case Key.e:
                    args.Handled = true;
                    await _controller.EditSelectedTask();
                    break;

                case Key.f:
                    args.Handled = true;
                    await _controller.FilterTasks();
                    break;

                case Key.d:
                    args.Handled = true;
                    await _controller.DeleteSelectedTask();
                    break;

                case Key.r:
                    args.Handled = true;
                    await _controller.LoadTasks(_controller.CurrentFilter);
                    break;

                case Key.T:
                    args.Handled = true;
                    await _controller.LoadTasks();
                    break;

                case Key.q:
                    args.Handled = true;
                    TuiApp.Shutdown();
                    break;

                // --- Vim-style navigation ---
                case Key.j:
                    args.Handled = true;
                    MoveSelection(1);
                    break;

                case Key.k:
                    args.Handled = true;
                    MoveSelection(-1);
                    break;

                case Key.g:
                    args.Handled = true;
                    _list.SelectedItem = 0;
                    break;

                case Key.G:
                    args.Handled = true;
                    _list.SelectedItem = _list.Source.Count - 1;
                    break;
            }
        };
    }

    private void MoveSelection(int delta)
    {
        if (_list.Source.Count == 0) return;

        int newIndex = Math.Clamp(_list.SelectedItem + delta, 0, _list.Source.Count - 1);
        _list.SelectedItem = newIndex;
        _list.SetNeedsDisplay();
    }

}

