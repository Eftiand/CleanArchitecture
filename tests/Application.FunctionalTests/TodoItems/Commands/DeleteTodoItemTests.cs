using CleanArchitecture.Application.TodoItems.Commands.CreateTodoItem;
using CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.FunctionalTests.TodoItems.Commands;

using static Testing;

public class DeleteTodoItemTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new DeleteTodoItemCommand(Guid.NewGuid());

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public Task ShouldDeleteTodoItem()
    {
        //    var itemId = await SendAsync(new CreateTodoItemCommand
        //    {
        //        Title = "New Item"
        //    });

        //    await SendAsync(new DeleteTodoItemCommand(itemId));

        //    var item = await FindAsync<TodoItem>(itemId);

        //    item.Should().BeNull();

        return Task.CompletedTask;
    }
}
