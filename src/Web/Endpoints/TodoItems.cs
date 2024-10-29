using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Security;
using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.UpdateTodoItemDetail;
using CleanArchitecture.Infrastructure.Common.Extensions;
using MassTransit.Mediator;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Shared.Contracts.Messaging;

namespace CleanArchitecture.Web.Endpoints;

public class TodoItems : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(Test, "test")
            .MapPost(CreateTodoItem)
            .MapPut(UpdateTodoItem, "{id}")
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
        var response = await sender.SendAsync<TodoCreatedResponse>(command, cancellationToken: ct);
        return Results.Ok(response);
    }

    private static async Task<IResult> UpdateTodoItem(IRequestClient<UpdateTodoItemCommand> sender, UpdateTodoItemDetailCommand command)
    {
        var result = await sender.GetResponse<TodoItemDeletedResponse>(command);
        return Results.Ok(result.Message.Id);
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
