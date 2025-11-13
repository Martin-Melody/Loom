using Loom.Application.DTOs.Tasks;
using Loom.Core.Entities;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Loom.CLI.Commands.Task;

public sealed class TaskListSettings : CommandSettings
{
    [CommandOption("--all")]
    public bool All { get; set; }

    [CommandOption("--pending")]
    public bool Pending { get; set; }

    [CommandOption("--complete")]
    public bool Complete { get; set; }

    [CommandOption("--due <value>")]
    public string? Due { get; set; }

    [CommandOption("--search <text>")]
    public string? Search { get; set; }
}

public sealed class TaskListCommand : Command<TaskListSettings>
{
    public override int Execute(
        CommandContext context,
        TaskListSettings settings,
        CancellationToken cancellationToken
    )
    {
        var service = CliServices.CreateTaskService();

        var filter = new TaskFilter();

        // status filter
        if (settings.Pending)
            filter.IsComplete = false;
        else if (settings.Complete)
            filter.IsComplete = true;
        else if (settings.All)
            filter.IsComplete = null;

        // due filter
        if (!string.IsNullOrWhiteSpace(settings.Due))
        {
            var clock = new Infrastructure.Time.SystemClock();
            if (settings.Due.Equals("today", StringComparison.OrdinalIgnoreCase))
                filter.DueBefore = clock.Today;
            else if (settings.Due.Equals("tomorrow", StringComparison.OrdinalIgnoreCase))
                filter.DueBefore = clock.Today.AddDays(1);
            else if (DateOnly.TryParse(settings.Due, out var parsed))
                filter.DueBefore = parsed;
        }

        // search filter
        if (!string.IsNullOrWhiteSpace(settings.Search))
            filter.TextContains = settings.Search;

        var views = service.GetTasksAsync(filter, cancellationToken).Result;

        if (!views.Any())
        {
            AnsiConsole.MarkupLine("[grey]No matching tasks found.[/]");
            return 0;
        }

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("[bold]Title[/]");
        table.AddColumn("[bold]Due[/]");
        table.AddColumn("[bold]Status[/]");

        foreach (var view in views)
        {
            var t = view.Item;

            string status =
                t.Status == TaskItemStatus.Complete
                    ? "[green]✔ Complete[/]"
                    : "[yellow]⏳ Pending[/]";

            table.AddRow(t.Title, t.DueDate?.ToString("yyyy-MM-dd") ?? "-", status);
        }

        AnsiConsole.Write(table);
        return 0;
    }
}
