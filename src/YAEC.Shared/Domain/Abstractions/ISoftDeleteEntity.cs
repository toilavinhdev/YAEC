namespace YAEC.Shared.Domain.Abstractions;

public interface ISoftDeleteEntity<TKey>
{
    DateTime? DeletedAt { get; set; }
    
    TKey? DeletedBy { get; set; }
}