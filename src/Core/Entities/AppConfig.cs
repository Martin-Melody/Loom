namespace Loom.Core.Entities;

using Loom.Core.Entities.Enums;

public class AppConfig
{
    public ViewType LastOpenView { get; set; } = ViewType.Dashboard;
    public SidebarState SidebarState { get; set; } = SidebarState.Closed;
}
