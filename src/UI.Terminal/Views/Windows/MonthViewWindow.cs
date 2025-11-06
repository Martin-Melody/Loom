using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Windows;

public sealed class MonthViewWindow : Window
{
    public MonthViewWindow()
    {
        Title = "Month View";
        Border.BorderStyle = LineStyle.None;

        var label = new Label(
            "Month Screen (e.g. show your tasks, events, logs for the week in 1 month calendar segments"
        )
        {
            X = Pos.Center(),
            Y = Pos.Center(),
        };

        Add(label);
    }
}
