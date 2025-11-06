using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Windows;

public sealed class DayViewWindow : Window
{
    public DayViewWindow()
    {
        Title = "Day View";
        Border.BorderStyle = LineStyle.None;

        var label = new Label(
            "Day Screen (e.g. show today's tasks, events, logs in a vertical time blocky format"
        )
        {
            X = Pos.Center(),
            Y = Pos.Center(),
        };

        Add(label);
    }
}
