using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem;
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
            .MapGet(Test, "test")
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

    private static async Task<IResult> DeleteTodoItem(IRequestClient<DeleteTodoItemCommand> sender, Guid id)
    {
        var result = await sender.GetResponse<TodoItemDeletedResponse>(new DeleteTodoItemCommand(id));
        return Results.Ok(result.ToString());
    }

    private static async Task<IResult> Test(IRequestClient<CreateTodoItemCommand> client)
    {
        var command = new CreateTodoItemCommand(1, "Testing");
        var response = await client.GetResponse<TodoItemCreatedResponse>(command);
        return Results.Ok(response.Message);
    }
}
