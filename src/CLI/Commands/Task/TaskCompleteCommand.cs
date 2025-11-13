using Loom.Application.DTOs.Tasks;
using Loom.Core.Entities;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Loom.CLI.Commands.Task;

public sealed class TaskCompleteSettings : CommandSettings
{
    [CommandArgument(0, "<search>")]
    public string Search { get; set; } = string.Empty;
}

public sealed class TaskCompleteCommand : Command<TaskCompleteSettings>
{
    public override ValidationResult Validate(CommandContext context, TaskCompleteSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Search))
            return ValidationResult.Error("You must provide a search term.");

        return ValidationResult.Success();
    }

    public override int Execute(
        CommandContext context,
        TaskCompleteSettings settings,
        CancellationToken cancellationToken
    )
    {
        var service = CliServices.CreateTaskService();

        // Fetch all tasks
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
                $"[red]No tasks found matching[/] \"[yellow]{settings.Search}[/]\"."
            );
            return 1;
        }

        TaskItem selected;

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

        // Toggle complete
        service.ToggleCompleteAsync(selected.Id, cancellationToken).Wait();

        // Display result
        string msg =
            selected.Status == TaskItemStatus.Complete
                ? "[green]✔ Marked complete[/]"
                : "[yellow]⟳ Marked pending[/]";

        AnsiConsole.MarkupLine($"{msg}: [white]{selected.Title}[/]");

        return 0;
    }
}
