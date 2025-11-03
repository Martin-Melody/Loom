using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Widgets;

public class WidgetManager
{
    private readonly List<IWidget> _widgets = new();

    public void RegisterWidget(IWidget widget)
    {
        _widgets.Add(widget);
    }

    public IEnumerable<IWidget> GetWidgets() => _widgets;

    public void RenderWidgets(Window parent)
    {
        int margin = 1;
        int rowHeight = 12;

        int index = 0;
        foreach (var widget in _widgets)
        {
            var view = widget.Render();

            // Compute layout
            bool isLeft = index % 2 == 0;
            int row = index / 2;

            view.X = isLeft ? 0 : Pos.Percent(50) + margin;
            view.Y = row * (rowHeight + margin);
            view.Width = Dim.Percent(50) - margin;
            view.Height = rowHeight;

            parent.Add(view);
            index++;
        }
    }
}
