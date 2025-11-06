using Loom.Application.Interfaces;
using Loom.UI.Terminal.Commands;
using Terminal.Gui;

namespace Loom.UI.Terminal.Input;

public class TaskListKeyHandler
{
    private readonly ListView _list;
    private readonly ICommandRegistry _registry;

    private static readonly Dictionary<Key, string> _commandBindings = new()
    {
        { Key.Space, CommandIds.Tasks.ToggleComplete },
        { Key.Enter, CommandIds.Tasks.ToggleExpand },
        { Key.a, CommandIds.Tasks.Add },
        { Key.A, CommandIds.Tasks.ShowAll },
        { Key.e, CommandIds.Tasks.Edit },
        { Key.f, CommandIds.Tasks.Filter },
        { Key.d, CommandIds.Tasks.Delete },
        { Key.r, CommandIds.Tasks.Refresh },
        { Key.T, CommandIds.Tasks.ShowToday },
        { Key.q, CommandIds.App.Quit },
    };

    public TaskListKeyHandler(ListView list, ICommandRegistry registry)
    {
        _list = list;
        _registry = registry;
    }

    public void Attach()
    {
        _list.KeyDown += (_, args) =>
        {
            var key = args.KeyEvent.Key;

            if (_commandBindings.TryGetValue(key, out var commandId))
            {
                args.Handled = true;
                _registry.Execute(commandId);
                return;
            }

            switch (key)
            {
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
        if (_list.Source.Count == 0)
            return;

        int newIndex = Math.Clamp(_list.SelectedItem + delta, 0, _list.Source.Count - 1);
        _list.SelectedItem = newIndex;
        _list.SetNeedsDisplay();
    }
}
