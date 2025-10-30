namespace Loom.Core.Entities;

public sealed class Habbit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public int CurrentStreak { get; set; }
    public int BestStreak { get; set; }
    public DateOnly? LastChecked { get; set; }

    public bool Tick(DateTime today)
    {

        if (LastChecked == DateOnly.FromDateTime(today)) return false;
        if (LastChecked.HasValue && LastChecked.Value == DateOnly.FromDateTime(today.AddDays(-1)))
            CurrentStreak++;
        else
            CurrentStreak = 1;

        if (CurrentStreak > BestStreak) BestStreak = CurrentStreak;
        LastChecked = DateOnly.FromDateTime(today);
        return true;

    }

}
