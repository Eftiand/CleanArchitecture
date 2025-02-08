using coaches.Modules.Shared.Application.Common.Interfaces;
using coaches.Modules.Shared.Application.Common.Models;
using coaches.Modules.Shared.Contracts.Common.Interfaces;
using coaches.Modules.Todos.Contracts.Commands;
using coaches.Web.Common;
using Microsoft.EntityFrameworkCore;

namespace coaches.Web.Endpoints;

public class TodoItemsController : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapPost(CreateTodoItem)
            .MapGet(GetAll)
            .MapPut(Update, "{id}")
            .MapGet(GetById, "{id}")
            .MapDelete(DeleteTodoItem, "{id}");
    }

    private static async Task<IResult> CreateTodoItem(
        ISender sender,
        CreateTodoItemCommand command,
        CancellationToken ct = default)
    {
        var result = await sender.SendAsync<CreateTodoItemCommand, Result<Guid>>(command, cancellationToken: ct);
        return result.ToApiResponse("Todo created");
    }

    private static async Task<IResult> GetById(
        IApplicationDbContext dbContext,
        Guid id,
        CancellationToken ct = default)
    {
        var result = await dbContext.TodoItems
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            .ToApiResponse();

        return result;
    }

    private static async Task<IResult> GetAll(
        IApplicationDbContext dbContext,
        CancellationToken ct = default)
    {
        var result = await dbContext.TodoItems
            .ToListAsync(ct)
            .ToApiResponse();

        return result;
    }

    private static async Task<IResult> DeleteTodoItem(
        ISender sender,
        Guid id,
        CancellationToken ct = default)
    {
        var result = await sender.SendAsync<DeleteTodoItemCommand,Result<Guid>>(new DeleteTodoItemCommand(id), ct);
        return result.ToApiResponse("Todo deleted");
    }

    private static async Task<IResult> Update(
        ISender sender,
        Guid id,
        UpdateTodoItemCommand command,
        CancellationToken ct = default)
    {
        if (command.TodoId != id)
        {
            return Results.BadRequest();
        }

        var result = await sender.SendAsync<UpdateTodoItemCommand, Result<Guid>>(command, ct);
        return result.ToApiResponse();
    }
}
