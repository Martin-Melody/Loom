using Loom.Application.Interfaces;
using Loom.UI.Terminal.Views.Dialogs;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Controllers;

public class CommandPaletteController
{
    private readonly ICommandRegistry _commands;

    public CommandPaletteController(ICommandRegistry commands)
    {
        _commands = commands;
    }

    public void Show()
    {
        var dialog = new CommandPaletteDialog(_commands);
        TuiApp.Run(dialog);

        if (dialog.SelectedCommand is { } cmd)
            _commands.Execute(cmd.Id);
    }
}
