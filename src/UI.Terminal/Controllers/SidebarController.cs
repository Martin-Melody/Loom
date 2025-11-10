using Loom.UI.Terminal.Views.UI;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Controllers;

public class SidebarController
{
    private readonly SidebarView _sidebar;
    private readonly View _mainContent;
    private bool _isVisible = true;

    public SidebarController(SidebarView sidebar, View mainContent)
    {
        _sidebar = sidebar;
        _mainContent = mainContent;
    }

    public bool IsVisible => _isVisible;

    public void Toggle()
    {
        _isVisible = !_isVisible;

        if (_isVisible)
        {
            _sidebar.Visible = true;
            _mainContent.X = Pos.Right(_sidebar);

            _sidebar.SetFocus();

            for (int w = 0; w <= 25; w += 5)
            {
                _sidebar.Width = w;
                _sidebar.SuperView?.LayoutSubviews();
                TuiApp.Refresh();
                Thread.Sleep(10);
            }
        }
        else
        {
            for (int w = 25; w >= 0; w -= 5)
            {
                _sidebar.Width = w;
                _sidebar.SuperView?.LayoutSubviews();
                TuiApp.Refresh();
                Thread.Sleep(10);
            }

            _sidebar.Visible = false;
            _mainContent.X = 0;
            _mainContent.SetFocus();
        }

        _sidebar.SuperView?.LayoutSubviews();
        _sidebar.SetNeedsDisplay();
        _mainContent.SetNeedsDisplay();
        TuiApp.Refresh();
    }

    public void Show() => SetVisibility(true);

    public void Hide() => SetVisibility(false);

    public void FocusMainContent() => _mainContent.SetFocus();

    public void FocusSidebar()
    {
        if (IsVisible)
            _sidebar.FocusList();
    }

    private void SetVisibility(bool visible)
    {
        if (_isVisible == visible)
            return;

        _isVisible = visible;
        _sidebar.Visible = visible;
        _mainContent.X = visible ? Pos.Right(_sidebar) : 0;

        if (visible)
            _sidebar.SetFocus();
        else
            _mainContent.SetFocus();

        _sidebar.SuperView?.LayoutSubviews();
        _sidebar.SetNeedsDisplay();
        _mainContent.SetNeedsDisplay();
        TuiApp.Refresh();
    }
}
