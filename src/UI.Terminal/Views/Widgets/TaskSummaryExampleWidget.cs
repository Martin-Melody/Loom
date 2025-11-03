using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Widgets;

public class TaskSummaryExampleWidget : BaseWidget
{
    private readonly Label _status;

    public TaskSummaryExampleWidget()
        : base("Tasks Overview")
    {
        _status = new Label("Press 'r' to refresh") { X = 1, Y = 1 };
        Add(_status);

        BindKey(Key.r, () => Refresh());
    }

    public override void Refresh()
    {
        _status.Text = $"Last refreshed at {DateTime.Now:T}";
        SetNeedsDisplay();
    }
}
