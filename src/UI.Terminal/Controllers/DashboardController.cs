using Loom.UI.Terminal.Views.Widgets;

namespace Loom.UI.Terminal.Controllers;

public class DashboardController
{
    private readonly WidgetManager _widgetManager;

    public DashboardController(WidgetManager widgetManager)
    {
        _widgetManager = widgetManager;
    }

    public void RefreshAllWidgets()
    {
        foreach (var w in _widgetManager.GetWidgets())
            w.Refresh();
    }
}
