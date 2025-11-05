using Loom.Infrastructure.Registry;
using Terminal.Gui;
using LoomCommand = Loom.Core.Entities.Command;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Input;

public class ShortcutManager
{
    private readonly CommandRegistry _registry;
    private readonly Dictionary<Key, LoomCommand> _bindings = new();

    public ShortcutManager(CommandRegistry registry)
    {
        _registry = registry;
    }

    public void Initialize()
    {
        foreach (var cmd in _registry.GetAll())
        {
            if (!string.IsNullOrEmpty(cmd.Shortcut) && TryParseKey(cmd.Shortcut!, out var key))
                _bindings[key] = cmd;
        }

        TuiApp.RootKeyEvent += OnKeyPressed;
    }

    private bool OnKeyPressed(KeyEvent keyEvent)
    {
        if (_bindings.TryGetValue(keyEvent.Key, out var command))
        {
            _ = Task.Run(command.Action);
            return true;
        }

        return false;
    }

    private static bool TryParseKey(string shortcut, out Key key)
    {
        key = shortcut.ToLower() switch
        {
            "ctrl+p" => Key.CtrlMask | Key.p,
            "ctrl+q" => Key.CtrlMask | Key.q,
            "ctrl+d" => Key.CtrlMask | Key.d,
            "ctrl+t" => Key.CtrlMask | Key.t,
            "ctrl+s" => Key.CtrlMask | Key.s,
            "a" => Key.A,
            _ => Key.Null,
        };

        return key != Key.Null;
    }
}
