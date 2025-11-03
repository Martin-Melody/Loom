using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Widgets;

public abstract class BaseWidget : FrameView, IWidget
{
    public string WidgetTitle { get; protected set; }

    protected BaseWidget(string title)
    {
        WidgetTitle = title;
        base.Title = title;
        BorderStyle = LineStyle.Rounded;
        CanFocus = false;
    }

    // Each widget returns itself (it *is* a View)
    public virtual View Render() => this;

    // Optional refresh logic
    public virtual void Refresh() { }
}
