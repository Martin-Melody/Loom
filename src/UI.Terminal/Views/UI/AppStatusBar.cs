using Terminal.Gui;

namespace Loom.UI.Terminal.Views.UI;

public static class AppStatusBar
{
    public static StatusBar Create(string initialModeLabel, out Action<bool, string> updateStatus)
    {
        // Single item showing mode info
        var modeItem = new StatusItem(Key.Null, $"Mode: {initialModeLabel}", null);

        var statusBar = new StatusBar(new[] { modeItem });

        updateStatus = (isOnline, label) =>
        {
            modeItem.Title = $"Mode: {label}";
            statusBar.SetNeedsDisplay();
        };

        return statusBar;
    }
}
