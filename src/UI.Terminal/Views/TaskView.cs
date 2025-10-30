using Loom.Core.Common.Extensions;
using Loom.Core.Entities;
using System.Globalization;

namespace Loom.UI.Terminal.Views;

public class TaskView
{
    public TaskItem Item { get; }
    public bool IsExpanded { get; set; }

    public TaskView(TaskItem item)
    {
        Item = item;
    }

    public IEnumerable<string> RenderLines()
    {
        var prefix = Item.Status == TaskItemStatus.Complete ? "◉" : "○";
        var title = $"{prefix} {Item.Title}";

        if (!IsExpanded)
            return new[] { title };

        var lines = new List<string> { title };

        // Helper for indentation (“│” for nice vertical structure)
        string G(string s) => $"   │ {s}";

        // Notes
        if (!string.IsNullOrWhiteSpace(Item.Notes))
            lines.Add(G($"Notes: {Item.Notes}"));

        // Format Due Date
        if (Item.DueDate is { } due)
        {
            string formattedDue = due.ToString("ddd, MMM dd yyyy", CultureInfo.InvariantCulture);
            lines.Add(G($"Due: {formattedDue}"));
        }

        // Format Created / Updated safely
        if (Item.CreatedAt is DateTime createdAt)
        {
            string created = createdAt.ToLocalTime()
                .ToString("HH:mm • MMM dd, yyyy", CultureInfo.InvariantCulture);
            lines.Add(G($"Created: {created}"));

            // Only show Updated if it’s different (by at least 1 second)
            if (Item.UpdatedAt is DateTime updatedAt)
            {
                var createdLocal = createdAt.ToLocalTime().TrimToSeconds();
                var updatedLocal = updatedAt.ToLocalTime().TrimToSeconds();

                if (updatedLocal > createdLocal)
                {
                    string updated = updatedLocal
                        .ToString("HH:mm • MMM dd, yyyy", CultureInfo.InvariantCulture);
                    lines.Add(G($"Updated: {updated}"));
                }
            }
        }

        lines.Add(string.Empty);
        return lines;
    }

}

