using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Package.Shared.Domain;
using Service.Catalog.Domain.Aggregates.ProductAggregate;

namespace Service.Catalog.Domain.Aggregates.CategoryAggregate;

[PrimaryKey(nameof(Id))]
[Index(nameof(AutoId), IsUnique = true)]
public class Category : IBaseEntity<Guid>, IAuditableEntity<string>, ISoftDeleteEntity<string>
{
    [Key]
    public Guid Id { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long AutoId { get; set; }

    public string Name { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public int? SortOrder { get; set; }
    
    [InverseProperty(nameof(ProductCategory.Category))]
    public List<ProductCategory> ProductCategories { get; set; } = [];
    
    public DateTime? CreatedAt { get; set; }
    
    public string? CreatedBy { get; set; }
    
    public DateTime? ModifiedAt { get; set; }
    
    public string? ModifiedBy { get; set; }
    
    public DateTime? DeletedAt { get; set; }
    
    public string? DeletedBy { get; set; }
}