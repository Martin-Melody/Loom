using Loom.Application.DTOs.Tasks;
using Loom.Application.Interfaces;

namespace Loom.Server.Endpoints;

public static class TasksEndpoints
{
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tasks").WithTags("Tasks");

        //
        // GET /tasks
        //
        group.MapGet(
            "/",
            async (ITaskService tasks, CancellationToken ct) =>
            {
                var views = await tasks.GetTasksAsync(null, ct);

                // Extract TaskItem from TaskView
                var items = views.Select(v => v.Item).ToList();

                return Results.Ok(items);
            }
        );

        //
        // POST /tasks
        //
        group.MapPost(
            "/",
            async (AddTaskRequest req, ITaskService tasks, CancellationToken ct) =>
            {
                var createdView = await tasks.AddTaskAsync(req, ct);
                return Results.Created($"/tasks/{createdView.Item.Id}", createdView.Item);
            }
        );

        //
        // PATCH /tasks/{id}
        //
        group.MapPatch(
            "/{id:guid}",
            async (Guid id, EditTaskRequest req, ITaskService tasks, CancellationToken ct) =>
            {
                // ensure ID comes from URL, not body
                req = req with
                {
                    Id = id,
                };

                var updatedView = await tasks.UpdateTaskAsync(req, ct);
                return Results.Ok(updatedView.Item);
            }
        );

        //
        // POST /tasks/{id}/toggle
        //
        group.MapPost(
            "/{id:guid}/toggle",
            async (Guid id, ITaskService tasks, CancellationToken ct) =>
            {
                var updatedView = await tasks.ToggleCompleteAsync(id, ct);
                return Results.Ok(updatedView.Item);
            }
        );

        //
        // DELETE /tasks/{id}
        //
        group.MapDelete(
            "/{id:guid}",
            async (Guid id, ITaskService tasks, CancellationToken ct) =>
            {
                await tasks.DeleteTaskAsync(id, ct);
                return Results.NoContent();
            }
        );

        return app;
    }
}
