using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Controllers;

public class ViewNavigator
{
    private readonly View _mainContent;

    public string CurrentViewName { get; private set; } = string.Empty;

    public ViewNavigator(View mainContent)
    {
        _mainContent = mainContent;
    }

    public void Show(View newView, string name)
    {
        _mainContent.RemoveAll();
        _mainContent.Add(newView);

        TuiApp.Refresh();
        newView.SetFocus();
        newView.FocusFirst();

        CurrentViewName = name;
    }
}
