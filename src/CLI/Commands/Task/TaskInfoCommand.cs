using Loom.Application.DTOs.Tasks;
using Loom.Core.Entities;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Loom.CLI.Commands.Task;

public sealed class TaskInfoSettings : CommandSettings
{
    [CommandArgument(0, "<search>")]
    public string Search { get; set; } = string.Empty;
}

public sealed class TaskInfoCommand : Command<TaskInfoSettings>
{
    public override ValidationResult Validate(CommandContext context, TaskInfoSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Search))
            return ValidationResult.Error("You must provide a search term.");

        return ValidationResult.Success();
    }

    public override int Execute(
        CommandContext context,
        TaskInfoSettings settings,
        CancellationToken cancellationToken
    )
    {
        var service = CliServices.CreateTaskService();

        // Load all tasks
        var views = service
            .GetTasksAsync(new TaskFilter { IsComplete = null }, cancellationToken)
            .Result;
        var items = views.Select(v => v.Item).ToList();

        // Fuzzy match
        var matches = items
            .Where(t => t.Title.Contains(settings.Search, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matches.Count == 0)
        {
            AnsiConsole.MarkupLine(
                $"[red]No task found matching[/] \"[yellow]{settings.Search}[/]\"."
            );
            return 1;
        }

        TaskItem selected;

        // If multiple matches → choice prompt
        if (matches.Count == 1)
        {
            selected = matches[0];
        }
        else
        {
            selected = AnsiConsole.Prompt(
                new SelectionPrompt<TaskItem>()
                    .Title($"Multiple matches for [yellow]{settings.Search}[/]. Select a task:")
                    .UseConverter(t =>
                    {
                        var due = t.DueDate?.ToString("yyyy-MM-dd") ?? "No due date";
                        return $"{t.Title} [grey]({due})[/]";
                    })
                    .AddChoices(matches)
            );
        }

        // Build the info text
        var rows = new List<string>
        {
            $"[bold]Title:[/] {selected.Title}",
            $"[bold]Status:[/] {(selected.Status == TaskItemStatus.Complete ? "[green]✔ Complete[/]" : "[yellow]⏳ Pending[/]")}",
        };

        if (!string.IsNullOrWhiteSpace(selected.Notes))
            rows.Add($"[bold]Notes:[/] {selected.Notes}");

        rows.Add($"[bold]Due:[/] {(selected.DueDate?.ToString("yyyy-MM-dd") ?? "-")}");
        rows.Add($"[bold]Created:[/] {selected.CreatedAt.ToLocalTime()}");

        if (selected.UpdatedAt is DateTime updated)
            rows.Add($"[bold]Updated:[/] {updated.ToLocalTime()}");

        if (selected.CompletedAt is DateTime completed)
            rows.Add($"[bold]Completed:[/] {completed.ToLocalTime()}");

        var panel = new Panel(string.Join("\n", rows))
        {
            Border = BoxBorder.Rounded,
            Header = new PanelHeader($"Task Info: {selected.Title}", Justify.Center),
            Padding = new Padding(1, 1),
        };

        AnsiConsole.Write(panel);

        return 0;
    }
}
