using Loom.Core.Entities;
using Loom.UI.Terminal.Commands;
using Loom.UI.Terminal.Controllers;

public static class NotificationHistoryCommandDefinitions
{
    public static IEnumerable<CommandDefinition> Create(
        NotificationHistoryController controller,
        Func<bool> isActive
    )
    {
        yield return new CommandDefinition(
            CommandIds.notificationHistory.DismissAll,
            "Dismiss All Notifications",
            "Notification History",
            controller.DismissAll,
            shortcut: "Ctrl+D",
            isGlobalShortcut: true
        );

        yield return new CommandDefinition(
            CommandIds.notificationHistory.DismissLast,
            "Dismiss Last Notifications",
            "Notification History",
            controller.DismissLast,
            shortcut: "Ctrl+D",
            isGlobalShortcut: true
        );
    }
}
