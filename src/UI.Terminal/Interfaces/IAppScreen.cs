using Terminal.Gui;

namespace Loom.UI.Terminal.Interfaces;

public interface IAppScreen
{
    string Name { get; }
    View Window { get; }
    void Action();
}
