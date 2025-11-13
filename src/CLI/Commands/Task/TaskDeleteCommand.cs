using Loom.Application.DTOs.Tasks;
using Loom.Core.Entities;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Loom.CLI.Commands.Task;

public sealed class TaskDeleteSettings : CommandSettings
{
    [CommandArgument(0, "<search>")]
    public string Search { get; set; } = string.Empty;
}

public sealed class TaskDeleteCommand : Command<TaskDeleteSettings>
{
    public override ValidationResult Validate(CommandContext context, TaskDeleteSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Search))
            return ValidationResult.Error("You must provide a search term.");

        return ValidationResult.Success();
    }

    public override int Execute(
        CommandContext context,
        TaskDeleteSettings settings,
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
            // More than one match → let user choose
            selected = AnsiConsole.Prompt(
                new SelectionPrompt<TaskItem>()
                    .Title(
                        $"Multiple matches for [yellow]{settings.Search}[/]. Select a task to delete:"
                    )
                    .UseConverter(t =>
                    {
                        var due = t.DueDate?.ToString("yyyy-MM-dd") ?? "No due date";
                        return $"{t.Title} [grey]({due})[/]";
                    })
                    .AddChoices(matches)
            );
        }

        // Confirmation
        bool confirmed = AnsiConsole.Confirm($"[red]Delete[/] task [yellow]{selected.Title}[/]?");
        if (!confirmed)
        {
            AnsiConsole.MarkupLine("[grey]Cancelled.[/]");
            return 0;
        }

        // Delete the task
        service.DeleteTaskAsync(selected.Id, cancellationToken).Wait();

        AnsiConsole.MarkupLine($"[red]✘ Deleted[/]: [white]{selected.Title}[/]");

        return 0;
    }
}
