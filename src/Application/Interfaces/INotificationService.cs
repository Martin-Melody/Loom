using Loom.Core.Entities;
using Loom.Core.Entities.Enums;

namespace Loom.Application.Interfaces;

public interface INotificationService
{
    void Show(
        string message,
        NotificationLevel level = NotificationLevel.Info,
        int durationSeconds = 4
    );

    void Info(string message, int durationSeconds = 4);
    void Success(string message, int durationSeconds = 4);
    void Warn(string message, int durationSeconds = 4);
    void Error(string message, int durationSeconds = 4);

    void DismissAll();
    void DismissLast();

    event Action<Notification>? OnNotify;
    event Action? OnDismissAll;
    event Action? OnDismissLast;

    IReadOnlyList<Notification> History { get; }
}
