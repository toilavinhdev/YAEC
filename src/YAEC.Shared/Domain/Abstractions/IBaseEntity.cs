namespace YAEC.Shared.Domain.Abstractions;

public interface IBaseEntity<TKey>
{
    TKey Id { get; set; }
    
    long SubId { get; set; }
}