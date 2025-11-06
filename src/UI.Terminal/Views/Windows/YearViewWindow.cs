using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Windows;

public sealed class YearViewWindow : Window
{
    public YearViewWindow()
    {
        Title = "Year View";
        Border.BorderStyle = LineStyle.None;

        var label = new Label(
            "Year Screen (e.g. show your tasks, events, logs for the week in 1 year segments"
        )
        {
            X = Pos.Center(),
            Y = Pos.Center(),
        };

        Add(label);
    }
}
