using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;
using TuiAtr = Terminal.Gui.Attribute;

namespace Loom.UI.Terminal.Theme;

public static class LoomTheme
{
    public static void ApplyLightTheme()
    {
        TuiApp.Driver?.SetAttribute(new TuiAtr(Color.Black, Color.Gray));

        Colors.Base = new ColorScheme()
        {
            Normal = new(Color.Black, Color.Gray),
            Focus = new(Color.Black, Color.Green),
            HotNormal = new(Color.BrightGreen, Color.Gray),
            HotFocus = new(Color.BrightGreen, Color.Green),
            Disabled = new(Color.Gray, Color.Black)
        };

        Colors.Dialog = new ColorScheme()
        {
            Normal = new(Color.Black, Color.Gray),
            Focus = new(Color.Black, Color.Green),
            HotNormal = new(Color.Green, Color.Gray),
            HotFocus = new(Color.Black, Color.Green),
            Disabled = new(Color.Gray, Color.Black)
        };

        Colors.Menu = new ColorScheme()
        {
            Normal = new(Color.Black, Color.BrightGreen),
            Focus = new(Color.Black, Color.Green),
            HotNormal = new(Color.BrightGreen, Color.Black),
            HotFocus = new(Color.BrightGreen, Color.Green),
            Disabled = new(Color.Gray, Color.Black)
        };

        Colors.Error = new ColorScheme()
        {
            Normal = new(Color.BrightRed, Color.Black),
            Focus = new(Color.White, Color.Red)
        };
    }

    public static void ApplyDarkTheme()
    {
        TuiApp.Driver?.SetAttribute(new TuiAtr(Color.White, Color.Black));

        Colors.Base = new ColorScheme()
        {
            Normal = new(Color.White, Color.Black),
            Focus = new(Color.Black, Color.BrightGreen),
            HotNormal = new(Color.Green, Color.Black),
            HotFocus = new(Color.Black, Color.BrightGreen),
            Disabled = new(Color.Gray, Color.Black)
        };

        Colors.Dialog = new ColorScheme()
        {
            Normal = new(Color.White, Color.Black),
            Focus = new(Color.Black, Color.BrightGreen),
            HotNormal = new(Color.Green, Color.Black),
            HotFocus = new(Color.Black, Color.BrightGreen),
            Disabled = new(Color.Gray, Color.Black)
        };
    }
}

