using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Windows;

public class DashboardWindow : Window
{
    public DashboardWindow()
    {
        Title = "Loom — Dashboard";
        Border.BorderStyle = LineStyle.None;

        // Example layout
        var widget1 = new FrameView("Widget 1")
        {
            X = 1,
            Y = 1,
            Width = 40,
            Height = 10,
            BorderStyle = LineStyle.Rounded,
        };

        var widget2 = new FrameView("Widget 2")
        {
            X = Pos.Right(widget1) + 2,
            Y = 1,
            Width = 40,
            Height = 10,
            BorderStyle = LineStyle.Rounded,
        };

        Add(widget1, widget2);

        Add(new Label("Press Ctrl+T to view Tasks") { X = 2, Y = Pos.Bottom(widget1) + 1 });
    }
}
