using Loom.Application.Services;
using Loom.Core.Entities.Enums;
using Loom.UI.Terminal.Views.UI;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

namespace Loom.UI.Terminal.Controllers;

public class SidebarController
{
    private readonly SidebarView _sidebar;
    private readonly View _mainContent;
    private readonly AppStateService _state;

    private bool _isVisible;
    private bool _isAnimating;

    private const int TargetWidth = 25;
    private const int Step = 5;
    private const int FrameMs = 10;

    public SidebarController(SidebarView sidebar, View mainContent, AppStateService state)
    {
        _sidebar = sidebar;
        _mainContent = mainContent;
        _state = state;

        _isVisible = _state.SidebarState == SidebarState.Opened;
        ApplyStateInstant();
    }

    public bool IsVisible => _isVisible;

    public void Toggle()
    {
        if (_isAnimating)
            return;

        _isVisible = !_isVisible;

        if (_isVisible)
            SmoothShow();
        else
            SmoothHide();

        _state.SidebarState = _isVisible ? SidebarState.Opened : SidebarState.Closed;
    }

    public void Show()
    {
        if (!_isVisible)
            Toggle();
    }

    public void Hide()
    {
        if (_isVisible)
            Toggle();
    }

    public void FocusSidebar()
    {
        if (_isVisible)
            _sidebar.FocusList();
    }

    // --- helpers ---

    private void ApplyStateInstant()
    {
        _sidebar.Visible = _isVisible;
        _sidebar.Width = _isVisible ? TargetWidth : 0;
        _mainContent.X = _isVisible ? Pos.Right(_sidebar) : 0;

        if (_isVisible)
            _sidebar.SetFocus();
        else
            _mainContent.SetFocus();

        _sidebar.SuperView?.LayoutSubviews();
        _sidebar.SetNeedsDisplay();
        _mainContent.SetNeedsDisplay();
        TuiApp.Refresh();
    }

    private void SmoothShow()
    {
        _isAnimating = true;

        _sidebar.Visible = true;
        _sidebar.Width = 0;
        _mainContent.X = Pos.Right(_sidebar);
        _sidebar.SetFocus();

        for (int w = 0; w <= TargetWidth; w += Step)
        {
            _sidebar.Width = w;
            _sidebar.SuperView?.LayoutSubviews();
            TuiApp.Refresh();
            Thread.Sleep(FrameMs);
        }

        _sidebar.Width = TargetWidth;
        FinishFrame();
        _isAnimating = false;
    }

    private void SmoothHide()
    {
        _isAnimating = true;

        for (int w = TargetWidth; w >= 0; w -= Step)
        {
            _sidebar.Width = w;
            _sidebar.SuperView?.LayoutSubviews();
            TuiApp.Refresh();
            Thread.Sleep(FrameMs);
        }

        _sidebar.Visible = false;
        _mainContent.X = 0;
        _mainContent.SetFocus();

        FinishFrame();
        _isAnimating = false;
    }

    private void FinishFrame()
    {
        _sidebar.SuperView?.LayoutSubviews();
        _sidebar.SetNeedsDisplay();
        _mainContent.SetNeedsDisplay();
        TuiApp.Refresh();
    }
}
