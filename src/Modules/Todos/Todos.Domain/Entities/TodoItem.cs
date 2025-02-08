using coaches.Modules.Shared.Domain.BaseEntities;

namespace Todos.Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    public string? Title { get; set; }
    public string? Note { get; set; }

    public DateTime? Reminder { get; set; }
    public bool Done { get; set; }

    public static TodoItem Create(string title, string note)
    {
        return new TodoItem
        {
            Title = title,
            Note = note,
        };
    }
}
