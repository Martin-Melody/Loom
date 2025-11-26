namespace Loom.Core.Entities;

using Loom.Core.Entities.Enums;

public class AppConfig
{
    public ViewType LastOpenView { get; set; } = ViewType.Dashboard;
    public SidebarState SidebarState { get; set; } = SidebarState.Closed;

    public ConnectionMode Mode { get; set; } = ConnectionMode.Local;

    // Optional override for local data directory, if empty/null fall back to ~/.loom
    public string? DataDirectory { get; set; }

    public string? ServerUrl { get; set; }

    public string? ApiKey { get; set; }
}
