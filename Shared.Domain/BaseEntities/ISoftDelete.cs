namespace coaches.Modules.Shared.Domain.BaseEntities;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTimeOffset? DeletedAt { get; set; }
}
