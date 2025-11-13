using Loom.Application.DTOs.Tasks;
using Loom.Core.Entities;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Loom.CLI.Commands.Task;

public sealed class TaskEditSettings : CommandSettings
{
    [CommandArgument(0, "<search>")]
    public string Search { get; set; } = string.Empty;

    [CommandOption("--title <value>")]
    public string? Title { get; set; }

    [CommandOption("--notes <value>")]
    public string? Notes { get; set; }

    [CommandOption("--due <value>")]
    public string? Due { get; set; }
}

public sealed class TaskEditCommand : Command<TaskEditSettings>
{
    public override ValidationResult Validate(CommandContext context, TaskEditSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Search))
            return ValidationResult.Error("You must provide a search string.");

        if (settings.Title is null && settings.Notes is null && settings.Due is null)
            return ValidationResult.Error("No fields to update. Use --title, --notes, or --due.");

        return ValidationResult.Success();
    }

    public override int Execute(
        CommandContext context,
        TaskEditSettings settings,
        CancellationToken cancellationToken
    )
    {
        var service = CliServices.CreateTaskService();
        var clock = new Loom.Infrastructure.Time.SystemClock();

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
            // Show selection prompt
            selected = AnsiConsole.Prompt(
                new SelectionPrompt<TaskItem>()
                    .Title(
                        $"Multiple matches for [yellow]{settings.Search}[/]. Select a task to edit:"
                    )
                    .UseConverter(t =>
                    {
                        var due = t.DueDate?.ToString("yyyy-MM-dd") ?? "No due date";
                        return $"{t.Title} [grey]({due})[/]";
                    })
                    .AddChoices(matches)
            );
        }

        // --- Prepare updated fields ---
        string? newTitle = settings.Title;
        string? newNotes = settings.Notes;
        DateOnly? newDue = null;

        // Parse due date if provided
        if (!string.IsNullOrWhiteSpace(settings.Due))
        {
            if (settings.Due.Equals("today", StringComparison.OrdinalIgnoreCase))
                newDue = clock.Today;
            else if (settings.Due.Equals("tomorrow", StringComparison.OrdinalIgnoreCase))
                newDue = clock.Today.AddDays(1);
            else if (DateOnly.TryParse(settings.Due, out var parsed))
                newDue = parsed;
            else
            {
                AnsiConsole.MarkupLine($"[red]Invalid date:[/] {settings.Due}");
                return 1;
            }
        }

        // Build edit request
        var req = new EditTaskRequest
        {
            Id = selected.Id,
            Title = newTitle ?? selected.Title,
            Notes = newNotes ?? selected.Notes,
            Due = newDue ?? selected.DueDate,
        };

        var updated = service.UpdateTaskAsync(req, cancellationToken).Result;

        // Pretty output
        var panel = new Panel(
            $@"[bold]Title:[/] {updated.Item.Title}
[bold]Notes:[/] {(updated.Item.Notes ?? "-")}
[bold]Due:[/] {(updated.Item.DueDate?.ToString("yyyy-MM-dd") ?? "-")}"
        )
        {
            Header = new PanelHeader("Task Updated", Justify.Center),
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1),
        };

        AnsiConsole.Write(panel);

        return 0;
    }
}
