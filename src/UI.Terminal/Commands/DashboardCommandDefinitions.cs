using Loom.Core.Entities;
using Loom.UI.Terminal.Commands;

namespace Loom.UI.Terminal.Controllers;

public static class DashboardCommandDefinitions
{
    public static IEnumerable<CommandDefinition> Create(
        DashboardController controller,
        Func<bool> isActive
    )
    {
        yield return new CommandDefinition(
            CommandIds.Dashboard.RefreshAll,
            "Refresh Dashboard",
            "Dashboard",
            controller.RefreshAllWidgets,
            "Refresh all widgets on the dashboard.",
            shortcut: "r",
            canExecute: isActive
        );

        yield return new CommandDefinition(
            CommandIds.Dashboard.NextWidget,
            "Next Widget",
            "Dashboard",
            controller.FocusNextWidget,
            "Move focus to the next widget.",
            shortcut: "Tab",
            canExecute: isActive
        );

        yield return new CommandDefinition(
            CommandIds.Dashboard.PreviousWidget,
            "Previous Widget",
            "Dashboard",
            controller.FocusPreviousWidget,
            "Move focus to the previous widget.",
            shortcut: "Shift+Tab",
            canExecute: isActive
        );

        yield return new CommandDefinition(
            CommandIds.Dashboard.PromptFocusWidget,
            "Focus Widget by Number",
            "Dashboard",
            controller.PromptFocusWidget,
            "Prompt for a widget number and move focus there.",
            shortcut: "Ctrl+G",
            canExecute: isActive
        );
    }
}
