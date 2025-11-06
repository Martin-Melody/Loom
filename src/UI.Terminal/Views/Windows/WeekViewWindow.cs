using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Windows;

public sealed class WeekViewWindow : Window
{
    public WeekViewWindow()
    {
        Title = "Week View";
        Border.BorderStyle = LineStyle.None;

        var label = new Label(
            "Week Screen (e.g. show your tasks, events, logs for the week in 1 week calendar segments"
        )
        {
            X = Pos.Center(),
            Y = Pos.Center(),
        };

        Add(label);
    }
}
