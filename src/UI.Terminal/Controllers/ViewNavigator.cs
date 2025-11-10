using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Controllers;

public class ViewNavigator
{
    private readonly View _mainContent;
    private View? _currentView;

    public string CurrentViewName { get; private set; } = string.Empty;

    public ViewNavigator(View mainContent)
    {
        _mainContent = mainContent;
    }

    public void Show(View newView, string name)
    {
        _mainContent.RemoveAll();
        _mainContent.Add(newView);

        _currentView = newView;
        CurrentViewName = name;

        TuiApp.Refresh();
        newView.SetFocus();
        newView.FocusFirst();
    }

    public void FocusCurrentView()
    {
        var target = _currentView ?? _mainContent.Subviews.LastOrDefault();
        if (target is null)
        {
            _mainContent.SetFocus();
            _mainContent.FocusFirst();
            TuiApp.Refresh();
            return;
        }

        target.SetFocus();
        target.FocusFirst();
        TuiApp.Refresh();
    }
}
