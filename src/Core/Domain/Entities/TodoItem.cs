﻿using CleanArchitecture.Domain.Common.BaseEntities;
using CleanArchitecture.Shared.Contracts.Todos;

namespace CleanArchitecture.Domain.Entities;

public class TodoItem : BaseAuditableEntity
{
    public string? Title { get; set; }

    public string? Note { get; set; }

    public PriorityLevel Priority { get; set; }

    public DateTime? Reminder { get; set; }

    private bool _done;
    public bool Done
    {
        get => _done;
        set
        {
            if (value && !_done)
            {
                AddDomainEvent(new TodoItemCompletedEvent(this.Id));
            }

            _done = value;
        }
    }
}
