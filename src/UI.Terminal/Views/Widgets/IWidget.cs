using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Widgets;

public interface IWidget
{
    bool IsFocused { get; }

    View Render();

    void Refresh();

    void SetFocus();
}
