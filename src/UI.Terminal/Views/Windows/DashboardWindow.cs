using Loom.UI.Terminal.Views.Widgets;
using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Windows;

public class DashboardWindow : Window
{
    public DashboardWindow()
    {
        Title = "Loom — Dashboard";
        Border.BorderStyle = LineStyle.None;

        CanFocus = true;
        TabStop = false;
        WantMousePositionReports = true;

        // Widgets
        var tasksWidget = new TaskSummaryExampleWidget
        {
            X = 1,
            Y = 1,
            Width = 40,
            Height = 10,
        };

        var statsWidget = new TaskSummaryExampleWidget
        {
            Title = "Stats Overview",
            X = Pos.Right(tasksWidget) + 2,
            Y = 1,
            Width = 40,
            Height = 10,
        };

        Add(tasksWidget, statsWidget);

        Add(new Label("Press Ctrl+T to view Tasks") { X = 2, Y = Pos.Bottom(tasksWidget) + 1 });
    }
}
