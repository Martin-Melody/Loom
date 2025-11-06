namespace Loom.UI.Terminal.Commands;

public static class CommandIds
{
    public static class App
    {
        public const string Quit = "app.quit";
    }

    public static class Navigation
    {
        public const string ShowDashboard = "navigation.dashboard";
        public const string ShowTasks = "navigation.tasks";
    }

    public static class Tasks
    {
        public const string Add = "tasks.add";
        public const string Edit = "tasks.edit";
        public const string Delete = "tasks.delete";
        public const string ToggleComplete = "tasks.toggleComplete";
        public const string ToggleExpand = "tasks.toggleExpand";
        public const string Filter = "tasks.filter";
        public const string ShowAll = "tasks.showAll";
        public const string Refresh = "tasks.refresh";
        public const string ShowToday = "tasks.showToday";
    }

    public static class Dashboard
    {
        public const string RefreshAll = "dashboard.refreshAll";
        public const string NextWidget = "dashboard.nextWidget";
        public const string PreviousWidget = "dashboard.previousWidget";
        public const string PromptFocusWidget = "dashboard.promptFocusWidget";
    }

    public static class Settings
    {
        public const string SaveConfig = "settings.saveConfig";
    }

    public static class Tools
    {
        public const string CommandPalette = "tools.commandPalette";
    }
}
