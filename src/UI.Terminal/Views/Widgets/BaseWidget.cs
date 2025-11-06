using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Widgets;

public abstract class BaseWidget : FrameView, IWidget
{
    private readonly Dictionary<Key, Action> _keyBindings = new();
    private bool _isFocused;

    public bool IsFocused => _isFocused;

    protected BaseWidget(string title)
        : base(title)
    {
        Border.BorderStyle = LineStyle.Rounded;
        CanFocus = true;
        ColorScheme = Colors.Base;

        Enter += (_, _) =>
        {
            _isFocused = true;
            OnFocus();
        };

        Leave += (_, _) =>
        {
            _isFocused = false;
            OnBlur();
        };

        KeyPress += (_, e) =>
        {
            if (_keyBindings.TryGetValue(e.KeyEvent.Key, out var handler))
            {
                handler.Invoke();
                e.Handled = true;
            }
        };

        MouseClick += (_, _) => SetFocus();
    }

    protected virtual void OnFocus()
    {
        ColorScheme = Colors.Dialog;
        SetNeedsDisplay();
    }

    protected virtual void OnBlur()
    {
        ColorScheme = Colors.Base;
        SetNeedsDisplay();
    }

    protected void BindKey(Key key, Action handler)
    {
        _keyBindings[key] = handler;
    }

    public View Render() => this;

    public abstract void Refresh();
}
