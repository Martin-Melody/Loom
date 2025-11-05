using Loom.Application.Interfaces;
using Loom.Core.Entities;

namespace Loom.UI.Terminal.Commands;

/// <summary>
/// Defines global and navigation commands.
/// </summary>
public class GlobalCommandProvider : ICommandProvider
{
    private readonly Func<Task> _openCommandPalette;
    private readonly Action _quitApp;
    private readonly Func<Task> _showDashboard;
    private readonly Func<Task> _showTasks;

    public GlobalCommandProvider(
        Func<Task> openCommandPalette,
        Action quitApp,
        Func<Task> showDashboard,
        Func<Task> showTasks
    )
    {
        _openCommandPalette = openCommandPalette;
        _quitApp = quitApp;
        _showDashboard = showDashboard;
        _showTasks = showTasks;
    }

    public IEnumerable<Command> GetCommands()
    {
        yield return new Command(
            "command.palette",
            "Open Command Palette",
            "Global",
            _openCommandPalette,
            "ctrl+p"
        );

        yield return new Command(
            "app.quit",
            "Quit Application",
            "Global",
            () =>
            {
                _quitApp();
                return Task.CompletedTask;
            },
            "ctrl+q"
        );

        yield return new Command(
            "nav.dashboard",
            "Open Dashboard",
            "Navigation",
            _showDashboard,
            "ctrl+d"
        );

        yield return new Command("nav.tasks", "Open Task List", "Navigation", _showTasks, "ctrl+t");
    }
}
