using CleanArchitecture.Domain.Common.BaseEntities;

namespace CleanArchitecture.Domain.Entities;

public class TodoList : BaseAuditableEntity
{
    public string? Title { get; set; }

    public Colour Colour { get; set; } = Colour.White;

    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();

    public void AddTodo(TodoItem todo)
    {
        Items.Add(todo);
    }

    public void Remove(TodoItem todo)
    {
        Items.Remove(todo);
    }
}
