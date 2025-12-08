using Loom.Application.Interfaces;
using Loom.Core.Entities;
using Loom.UI.Terminal.Views.UI;
using Terminal.Gui;
using TuiApp = Terminal.Gui.Application;

public sealed class NotificationManager
{
    private readonly List<NotificationView> _active = new();

    public NotificationManager(INotificationService service)
    {
        service.OnNotify += ShowNotification;
        service.OnDismissAll += DismissAllNotifications;
        service.OnDismissLast += DismissLastNotification;
        ;

        TuiApp.MainLoop.AddTimeout(
            TimeSpan.FromMilliseconds(200),
            _ =>
            {
                Cleanup();
                return true;
            }
        );
    }

    private void ShowNotification(Notification n)
    {
        var popup = new NotificationView(n);

        // Right side alignment
        popup.X = Pos.AnchorEnd(popup.Frame.Width + 2);
        popup.Y = _active.Count == 0 ? 1 : _active.Last().Y + 3; // stack vertically

        _active.Add(popup);
        TuiApp.Top.Add(popup);

        TuiApp.Refresh();
    }

    private void DismissAllNotifications()
    {
        foreach (var v in _active)
            TuiApp.Top.Remove(v);

        _active.Clear();
        TuiApp.Refresh();
    }

    private void DismissLastNotification()
    {
        if (_active.Count == 0)
            return;

        var top = _active.Last();
        TuiApp.Top.Remove(top);
        _active.Remove(top);
        TuiApp.Refresh();
    }

    private void Cleanup()
    {
        var now = DateTime.Now;

        foreach (var v in _active.ToList())
        {
            if (now - v.CreatedAt > v.Duration)
            {
                TuiApp.Top.Remove(v);
                _active.Remove(v);
            }
        }

        // Re-stack remaining notifications cleanly
        int y = 1;
        foreach (var v in _active)
        {
            v.Y = y;
            y += 3;
        }

        TuiApp.Refresh();
    }
}
