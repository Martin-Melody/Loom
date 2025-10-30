using Loom.Application.Interfaces;

namespace Loom.Infrastructure.Time;

public sealed class SystemClock : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}

