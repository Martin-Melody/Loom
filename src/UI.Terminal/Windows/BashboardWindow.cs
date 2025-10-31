using Terminal.Gui;

namespace Loom.UI.Terminal.Windows;

public class DashboardWindow : Window
{
    public DashboardWindow()
    {
        Title = "Loom — Dashboard Layout Test";

        // First frame (e.g., placeholder for TaskList)
        var taskListFrame = new FrameView("Task List")
        {
            X = 0,
            Y = 0,
            Width = Dim.Percent(50),   // half the screen width
            Height = Dim.Fill(),       // full height
            CanFocus = true,
        };

        // Second frame (e.g., placeholder for Calendar)
        var calendarFrame = new FrameView("Calendar")
        {
            X = Pos.Right(taskListFrame),
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            CanFocus = true,
        };

        // Optional: add some placeholder content
        taskListFrame.Add(new Label("Tasks will go here") { X = 1, Y = 0 });
        calendarFrame.Add(new Label("Calendar widget area") { X = 1, Y = 0 });

        Add(taskListFrame, calendarFrame);
    }
}

