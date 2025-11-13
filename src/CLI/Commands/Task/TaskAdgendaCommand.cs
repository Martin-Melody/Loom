using Loom.Application.DTOs.Tasks;
using Loom.Core.Entities;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Loom.CLI.Commands.Task;

public sealed class TaskAgendaSettings : CommandSettings { }

public sealed class TaskAgendaCommand : Command<TaskAgendaSettings>
{
    public override int Execute(
        CommandContext context,
        TaskAgendaSettings settings,
        CancellationToken cancellationToken
    )
    {
        var service = CliServices.CreateTaskService();
        var clock = new Loom.Infrastructure.Time.SystemClock();

        // Fetch ALL tasks
        var views = service
            .GetTasksAsync(new TaskFilter { IsComplete = null }, cancellationToken)
            .Result;
        var tasks = views.Select(v => v.Item).ToList();

        // Group by due date
        var groups = tasks
            .GroupBy(t => t.DueDate)
            .OrderBy(g => g.Key ?? DateOnly.MaxValue)
            .ToList();

        int maxContentWidth = 0;
        int maxHeaderWidth = 0;

        foreach (var g in groups)
        {
            string header = g.Key switch
            {
                null => "Someday",
                var d when d == clock.Today => $"Today ({d:yyyy-MM-dd})",
                var d when d == clock.Today.AddDays(1) => $"Tomorrow ({d:yyyy-MM-dd})",
                var d => $"{d:ddd, MMM dd yyyy}",
            };

            maxHeaderWidth = Math.Max(maxHeaderWidth, header.Length + 4);

            foreach (var t in g)
            {
                string line = $"{(t.Status == TaskItemStatus.Complete ? "✔" : "⏳")} {t.Title}";
                maxContentWidth = Math.Max(maxContentWidth, line.Length + 4);
            }
        }

        int panelWidth = Math.Max(maxHeaderWidth, maxContentWidth) + 4;

        foreach (var g in groups)
        {
            string header = g.Key switch
            {
                null => "[grey]Someday[/]",
                var d when d == clock.Today => $"[yellow]Today[/] ({d:yyyy-MM-dd})",
                var d when d == clock.Today.AddDays(1) => $"[yellow]Tomorrow[/] ({d:yyyy-MM-dd})",
                var d => $"[blue]{d:ddd, MMM dd yyyy}[/]",
            };

            var items = g.OrderBy(t => t.Status).ToList();

            var paddedLines = items.Select(t =>
            {
                string status =
                    t.Status == TaskItemStatus.Complete ? "[green]✔[/]" : "[yellow]⏳[/]";

                string text = $"{status} {t.Title}";
                return text.PadRight(panelWidth - 4);
            });

            var body = new Markup(string.Join("\n", paddedLines));

            var panel = new Panel(body)
            {
                Header = new PanelHeader(header, Justify.Center),
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1),
                Expand = false,
                Width = panelWidth,
            };

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }

        return 0;
    }
}
