using Loom.Application.Interfaces;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Input;

public class GlobalShortcutManager
{
    private readonly ICommandRegistry _commands;
    private readonly Dictionary<Key, string> _shortcutMap = new();

    public GlobalShortcutManager(ICommandRegistry commands)
    {
        _commands = commands;
    }

    public void Configure()
    {
        _shortcutMap.Clear();

        foreach (var command in _commands.GetAll())
        {
            if (!command.IsGlobalShortcut || string.IsNullOrWhiteSpace(command.Shortcut))
                continue;

            if (TryParseShortcut(command.Shortcut, out var key))
                _shortcutMap[key] = command.Id;
        }

        TuiApp.RootKeyEvent = keyEvent =>
        {
            if (_shortcutMap.TryGetValue(keyEvent.Key, out var commandId))
            {
                _commands.Execute(commandId);
                return true;
            }

            return false;
        };
    }

    private static bool TryParseShortcut(string? shortcut, out Key key)
    {
        key = Key.Null;
        if (string.IsNullOrWhiteSpace(shortcut))
            return false;

        var result = Key.Null;
        var parts = shortcut.Split(
            '+',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        );

        foreach (var part in parts)
        {
            switch (part.ToLowerInvariant())
            {
                case "ctrl":
                case "control":
                    result |= Key.CtrlMask;
                    break;
                case "shift":
                    result |= Key.ShiftMask;
                    break;
                case "alt":
                    result |= Key.AltMask;
                    break;
                default:
                    if (Enum.TryParse<Key>(part, true, out var parsed))
                        result |= parsed;
                    else if (part.Length == 1)
                        result |= (Key)char.ToUpperInvariant(part[0]);
                    else
                        return false;
                    break;
            }
        }

        key = result;
        return true;
    }
}
