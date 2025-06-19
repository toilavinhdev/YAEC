namespace YAEC.Shared.Domain.Abstractions;

public interface IAuditableEntity<TKey>
{
    DateTime? CreatedAt { get; set; }
    
    TKey? CreatedBy { get; set; }
    
    DateTime? ModifiedAt { get; set; }
    
    TKey? ModifiedBy { get; set; }
}