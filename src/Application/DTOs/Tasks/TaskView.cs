using System.Globalization;
using Loom.Core.Common.Extensions;
using Loom.Core.Entities;

namespace Loom.Application.DTOs.Tasks;

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
        // Use Unicode circles for visual flair
        var prefix = Item.Status == TaskItemStatus.Complete ? "◉" : "○";
        var title = $"{prefix} {Item.Title}";

        if (!IsExpanded)
            return new[] { title };

        var lines = new List<string> { title };

        // Helper for indentation (“│” for nice vertical structure)
        string Indent(string s) => $"   │ {s}";

        // Notes
        if (!string.IsNullOrWhiteSpace(Item.Notes))
            lines.Add(Indent($"Notes: {Item.Notes}"));

        // Due Date
        if (Item.DueDate is { } due)
        {
            string formattedDue = due.ToString("ddd, MMM dd yyyy", CultureInfo.InvariantCulture);
            lines.Add(Indent($"Due: {formattedDue}"));
        }

        // Status & Completion
        lines.Add(Indent($"Status: {Item.Status}"));

        if (Item.CompletedAt is DateTime completedAt)
        {
            string formattedCompleted = completedAt
                .ToLocalTime()
                .ToString("HH:mm • MMM dd, yyyy", CultureInfo.InvariantCulture);
            lines.Add(Indent($"Completed: {formattedCompleted}"));
        }

        // Created / Updated
        if (Item.CreatedAt is DateTime createdAt)
        {
            string created = createdAt
                .ToLocalTime()
                .ToString("HH:mm • MMM dd, yyyy", CultureInfo.InvariantCulture);
            lines.Add(Indent($"Created: {created}"));

            if (Item.UpdatedAt is DateTime updatedAt)
            {
                var createdLocal = createdAt.ToLocalTime().TrimToSeconds();
                var updatedLocal = updatedAt.ToLocalTime().TrimToSeconds();

                if (updatedLocal > createdLocal)
                {
                    string updated = updatedLocal.ToString(
                        "HH:mm • MMM dd, yyyy",
                        CultureInfo.InvariantCulture
                    );
                    lines.Add(Indent($"Updated: {updated}"));
                }
            }
        }

        lines.Add(string.Empty); // spacing
        return lines;
    }

    public override string ToString() =>
        $"{(Item.Status == TaskItemStatus.Complete ? "[x]" : "[ ]")} {Item.Title}";
}
