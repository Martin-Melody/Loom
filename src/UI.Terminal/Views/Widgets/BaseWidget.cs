using Terminal.Gui;

namespace Loom.UI.Terminal.Views.Widgets;

public abstract class BaseWidget : FrameView
{
    private readonly Dictionary<Key, Action> _keyBindings = new();

    protected BaseWidget(string title)
        : base(title)
    {
        Border.BorderStyle = LineStyle.Rounded;
        CanFocus = true;

        // Set base appearance using theme
        ColorScheme = Colors.Base;

        // Handle focus/blur
        Enter += (_, _) => OnFocus();
        Leave += (_, _) => OnBlur();

        // Handle key presses only while focused
        KeyPress += (_, e) =>
        {
            if (_keyBindings.TryGetValue(e.KeyEvent.Key, out var handler))
            {
                handler.Invoke();
                e.Handled = true;
            }
        };

        // Optional: focus by mouse click anywhere in the widget
        MouseClick += (_, _) => SetFocus();
    }

    protected virtual void OnFocus()
    {
        // Use the theme’s focus color scheme
        ColorScheme = Colors.Dialog; // slightly brighter green accent in your theme
        SetNeedsDisplay();
    }

    protected virtual void OnBlur()
    {
        // Revert to normal base scheme
        ColorScheme = Colors.Base;
        SetNeedsDisplay();
    }

    protected void BindKey(Key key, Action handler)
    {
        _keyBindings[key] = handler;
    }

    public abstract void Refresh();
}
