using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.TodoItems.Consumers;
using CleanArchitecture.Infrastructure.Common.Extensions;
using CleanArchitecture.Shared.Contracts.Messaging;
using CleanArchitecture.Shared.Contracts.Todos;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Web.Endpoints;

public class TodoItems : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateTodoItem)
            .MapGet(GetTodoItems)
            .MapDelete(DeleteTodoItem, "{id}");
    }

    private static async Task<IResult> GetTodoItems(IApplicationDbContext context)
    {
        var result = await context.TodoItems.ToListAsync();
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateTodoItem(
        ISender sender,
        CreateTodoItemCommand command,
        CancellationToken ct = default)
    {
        var response = await sender.SendAsync<TodoItemCreatedResponse>(command, cancellationToken: ct);
        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteTodoItem(
        ISender sender,
        Guid id,
        CancellationToken ct = default)
    {
        var result = await sender.SendAsync<TodoItemDeletedResponse>(new DeleteTodoItemCommand(id));
        return Results.Ok(result.ToString());
    }
}
