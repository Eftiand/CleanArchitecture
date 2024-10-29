namespace Shared.Contracts;

public class TodoCreatedEvent
{
    public Guid TodoId { get; set; }
}
public class TodoCreatedResponse(Guid todoId)
{
    public Guid TodoId { get; set; } = todoId;
}
