using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Package.Shared.Domain;
using Service.Catalog.Domain.Aggregates.CategoryAggregate;

namespace Service.Catalog.Domain.Aggregates.ProductAggregate;

[PrimaryKey(nameof(Id))]
[Index(nameof(AutoId), IsUnique = true)]
public class ProductCategory : IBaseEntity<Guid>, IAuditableEntity<string>
{
    [Key]
    public Guid Id { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long AutoId { get; set; }
    
    public Guid ProductId { get; set; }
    [ForeignKey(nameof(ProductId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public Product Product { get; set; } = null!;
    
    public Guid CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId)), DeleteBehavior(DeleteBehavior.Restrict)]
    public Category Category { get; set; } = null!;
    
    public DateTime? CreatedAt { get; set; }
    
    public string? CreatedBy { get; set; }
    
    public DateTime? ModifiedAt { get; set; }
    
    public string? ModifiedBy { get; set; }
}