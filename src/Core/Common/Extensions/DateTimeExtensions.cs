namespace Loom.Core.Common.Extensions;

public static class DateTimeExtensions
{
    public static DateTime TrimToSeconds(this DateTime dt) =>
        new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Kind);
}

