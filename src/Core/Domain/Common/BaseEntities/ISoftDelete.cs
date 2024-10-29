namespace CleanArchitecture.Domain.Common.BaseEntities;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}
