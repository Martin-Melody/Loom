using Loom.Core.Entities;
using Loom.Infrastructure.Persistence;
using Loom.UI.Terminal.Controllers;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Commands;

public static class GlobalCommandDefinitions
{
    public static IEnumerable<CommandDefinition> Create(
        AppController app,
        ConfigRepository configRepo
    )
    {
        return new[]
        {
            new CommandDefinition(
                CommandIds.Navigation.ShowDashboard,
                "Open Dashboard",
                "Navigation",
                app.ShowDashboard,
                shortcut: "Ctrl+D",
                isGlobalShortcut: true
            ),
            new CommandDefinition(
                CommandIds.Navigation.ShowTasks,
                "Open Task List",
                "Navigation",
                app.ShowTasks,
                shortcut: "Ctrl+T",
                isGlobalShortcut: true
            ),
            //TODO: Change these shortcuts as Terminal GUI can't see them
            new CommandDefinition(
                CommandIds.Navigation.ShowDay,
                "Open Day View",
                "Navigation",
                app.ShowDay,
                shortcut: "Ctrl+1",
                isGlobalShortcut: true
            ),
            new CommandDefinition(
                CommandIds.Navigation.ShowWeek,
                "Open Week View",
                "Navigation",
                app.ShowWeek,
                shortcut: "Ctrl+2",
                isGlobalShortcut: true
            ),
            new CommandDefinition(
                CommandIds.Navigation.ShowMonth,
                "Open Month View",
                "Navigation",
                app.ShowMonth,
                shortcut: "Ctrl+3",
                isGlobalShortcut: true
            ),
            new CommandDefinition(
                CommandIds.Navigation.ShowYear,
                "Open Year View",
                "Navigation",
                app.ShowYear,
                shortcut: "Ctrl+4",
                isGlobalShortcut: true
            ),
            new CommandDefinition(
                CommandIds.Settings.SaveConfig,
                "Save Configuration",
                "Settings",
                async () =>
                {
                    var config = new AppConfig { LastOpenView = app.CurrentViewName };
                    await configRepo.SaveAsync(config);
                    MessageBox.Query("Config Saved", "Configuration successfully saved!", "OK");
                }
            ),
            new CommandDefinition(
                CommandIds.App.Quit,
                "Quit",
                "Application",
                () => TuiApp.RequestStop(),
                shortcut: "Ctrl+Q",
                isGlobalShortcut: true
            ),
            new CommandDefinition(
                CommandIds.Tools.CommandPalette,
                "Command Palette",
                "Tools",
                app.ShowCommandPalette,
                "Open the command palette.",
                shortcut: "Ctrl+P",
                isGlobalShortcut: true
            ),
        };
    }
}
