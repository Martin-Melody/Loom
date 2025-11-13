using Loom.Application.DTOs.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Loom.CLI.Commands.Task;

public sealed class TaskAddSettings : CommandSettings
{
    [CommandArgument(0, "<title>")]
    public string Title { get; set; } = string.Empty;

    [CommandOption("--due <value>")]
    public string? Due { get; set; }

    [CommandOption("--notes <text>")]
    public string? Notes { get; set; }
}

public sealed class TaskAddCommand : Command<TaskAddSettings>
{
    public override ValidationResult Validate(CommandContext context, TaskAddSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Title))
            return ValidationResult.Error("Title cannot be empty.");

        return ValidationResult.Success();
    }

    public override int Execute(
        CommandContext context,
        TaskAddSettings settings,
        CancellationToken cancellationToken
    )
    {
        var service = CliServices.CreateTaskService();
        var clock = new Infrastructure.Time.SystemClock();

        DateOnly? due = null;

        if (!string.IsNullOrWhiteSpace(settings.Due))
        {
            if (settings.Due.Equals("today", StringComparison.OrdinalIgnoreCase))
                due = clock.Today;
            else if (settings.Due.Equals("tomorrow", StringComparison.OrdinalIgnoreCase))
                due = clock.Today.AddDays(1);
            else if (DateOnly.TryParse(settings.Due, out var parsed))
                due = parsed;
            else
            {
                AnsiConsole.MarkupLine($"[red]Invalid date:[/] {settings.Due}");
                return 1;
            }
        }

        var req = new AddTaskRequest
        {
            Title = settings.Title,
            Notes = settings.Notes,
            Due = due,
        };

        var created = service.AddTaskAsync(req, cancellationToken).Result;

        var panel = new Panel(
            $@"[bold]{created.Item.Title}[/]
Notes: {(created.Item.Notes ?? "-")}
Due: {(created.Item.DueDate?.ToString("yyyy-MM-dd") ?? "-")}"
        )
        {
            Header = new PanelHeader("Task Added", Justify.Center),
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1),
        };

        AnsiConsole.Write(panel);
        return 0;
    }
}
