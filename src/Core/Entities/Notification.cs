using Loom.Core.Entities.Enums;

namespace Loom.Core.Entities;

public sealed class Notification
{
    public string Message { get; }
    public NotificationLevel Level { get; }
    public DateTime CreatedAt { get; }
    public TimeSpan Duration { get; }

    public Notification(string message, NotificationLevel level, int seconds = 4)
    {
        Message = message;
        Level = level;
        Duration = TimeSpan.FromSeconds(seconds);
        CreatedAt = DateTime.UtcNow;
    }
}
