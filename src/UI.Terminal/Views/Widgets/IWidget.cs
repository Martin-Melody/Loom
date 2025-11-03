using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Widgets;

public interface IWidget
{
    String Title { get; }

    View Render();

    void Refresh();
}
