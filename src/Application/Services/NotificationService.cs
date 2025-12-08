using Loom.Application.Interfaces;
using Loom.Core.Entities;
using Loom.Core.Entities.Enums;

namespace Loom.Application.Services;

public sealed class NotificationService : INotificationService
{
    private readonly List<Notification> _history = new();
    public IReadOnlyList<Notification> History => _history;

    private const int DefaultDuration = 4;

    public event Action<Notification>? OnNotify;

    public event Action? OnDismissAll;
    public event Action? OnDismissLast;

    public void Show(
        string message,
        NotificationLevel level = NotificationLevel.Info,
        int durationSeconds = DefaultDuration
    )
    {
        var n = new Notification(message, level, durationSeconds);

        _history.Add(n);

        if (_history.Count > 200)
            _history.RemoveAt(0);

        OnNotify?.Invoke(n);
    }

    public void Info(string msg, int durationSeconds = DefaultDuration) =>
        Show(msg, NotificationLevel.Info, durationSeconds);

    public void Success(string msg, int durationSeconds = DefaultDuration) =>
        Show(msg, NotificationLevel.Success, durationSeconds);

    public void Warn(string msg, int durationSeconds = DefaultDuration) =>
        Show(msg, NotificationLevel.Warning, durationSeconds);

    public void Error(string msg, int durationSeconds = DefaultDuration) =>
        Show(msg, NotificationLevel.Error, durationSeconds);

    public void DismissAll()
    {
        OnDismissAll?.Invoke();
    }

    public void DismissLast()
    {
        OnDismissLast?.Invoke();
    }
}
