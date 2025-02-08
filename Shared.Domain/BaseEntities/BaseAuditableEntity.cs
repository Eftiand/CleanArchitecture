namespace coaches.Modules.Shared.Domain.BaseEntities;

public abstract class BaseAuditableEntity : BaseEntity, ISoftDelete
{
    public DateTimeOffset Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
}
