namespace CleanArchitecture.Domain.Events;

public record TodoItemDeletedEvent(TodoItem item)
    : BaseEvent
{
    public TodoItem Item { get; } = item;
}
