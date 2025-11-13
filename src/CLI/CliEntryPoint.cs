using Loom.CLI.Commands.Task;
using Spectre.Console.Cli;

namespace Loom.CLI;

public static class CliEntryPoint
{
    public static int Run(string[] args)
    {
        var app = new CommandApp();

        app.Configure(config =>
        {
            config.SetApplicationName("loom");

            config.AddBranch(
                "task",
                task =>
                {
                    task.SetDescription("Manage tasks in Loom");

                    task.AddCommand<TaskListCommand>("list")
                        .WithDescription("List tasks with optional filters");

                    task.AddCommand<TaskAddCommand>("add").WithDescription("Add a new task");

                    task.AddCommand<TaskEditCommand>("edit")
                        .WithDescription("Edit an existing task");

                    task.AddCommand<TaskCompleteCommand>("complete")
                        .WithDescription("Toggle completion of a matching task");

                    task.AddCommand<TaskDeleteCommand>("delete")
                        .WithDescription("Delete a matching task");

                    task.AddCommand<TaskInfoCommand>("info")
                        .WithDescription("Show details for a matching task");

                    task.AddCommand<TaskAgendaCommand>("agenda")
                        .WithDescription("Show tasks grouped by due date");
                }
            );
        });

        return app.Run(args);
    }
}
