using Loom.UI.Terminal.Views.Widgets;
using Loom.UI.Terminal.Views.Windows;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Controllers;

public class DashboardController
{
    private readonly WidgetManager _widgetManager;
    private readonly DashboardWindow _dashboard;

    public DashboardController(WidgetManager widgetManager, DashboardWindow dashboard)
    {
        _widgetManager = widgetManager;
        _dashboard = dashboard;
    }

    public void RefreshAllWidgets()
    {
        foreach (var w in _widgetManager.GetWidgets())
            w.Refresh();
    }

    public void FocusNextWidget()
    {
        _dashboard.FocusNext();
    }

    public void FocusPreviousWidget()
    {
        _dashboard.FocusPrev();
    }

    public void FocusWidget(int index)
    {
        var subviews = _dashboard.Subviews;
        if (subviews.Count == 0)
            return;

        if (index < 0)
            index = 0;
        if (index >= subviews.Count)
            index = subviews.Count - 1;

        var target = subviews[index];
        if (target.CanFocus)
            target.SetFocus();
    }

    public void PromptFocusWidget()
    {
        var dialog = new Dialog()
        {
            Width = 50,
            Height = 10,
            Title = "Focus Widget",
        };

        var label = new Label("Enter widget number:") { X = 1, Y = 1 };
        var input = new TextField("")
        {
            X = Pos.Right(label) + 1,
            Y = 1,
            Width = 10,
        };
        dialog.Add(label, input);

        bool confirmed = false;

        var okButton = new Button("OK", is_default: true);
        okButton.Clicked += (_, __) =>
        {
            confirmed = true;
            TuiApp.RequestStop();
        };

        var cancelButton = new Button("Cancel");
        cancelButton.Clicked += (_, __) => TuiApp.RequestStop();

        dialog.AddButton(okButton);
        dialog.AddButton(cancelButton);

        TuiApp.Run(dialog);

        if (!confirmed)
            return;

        if (int.TryParse(input.Text.ToString(), out int n))
        {
            // Adjust because user will likely enter 1-based index
            FocusWidget(n - 1);
        }
        else
        {
            MessageBox.ErrorQuery("Invalid Input", "Please enter a valid number.", "OK");
        }
    }
}
