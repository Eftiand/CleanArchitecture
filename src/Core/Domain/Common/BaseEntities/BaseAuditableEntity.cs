namespace CleanArchitecture.Domain.Common.BaseEntities;

public abstract class BaseAuditableEntity : BaseEntity, ISoftDelete
{
    public DateTimeOffset Created { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }
}
