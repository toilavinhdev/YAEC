using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Package.Shared.Domain;
using Service.Catalog.Domain.Aggregates.ProductAggregate;

namespace Service.Catalog.Domain.Aggregates.ShopAggregate;

[PrimaryKey(nameof(Id))]
[Index(nameof(AutoId), IsUnique = true)]
public class Shop : IBaseEntity<Guid>, IAuditableEntity<string>, ISoftDeleteEntity<string>
{
    [Key]
    public Guid Id { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long AutoId { get; set; }
    
    public string UserId { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    [InverseProperty(nameof(Product.Shop))]
    public List<Product> Products { get; set; } = [];
    
    public DateTime? CreatedAt { get; set; }
    
    public string? CreatedBy { get; set; }
    
    public DateTime? ModifiedAt { get; set; }
    
    public string? ModifiedBy { get; set; }
    
    public DateTime? DeletedAt { get; set; }
    
    public string? DeletedBy { get; set; }
}