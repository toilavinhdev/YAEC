using Microsoft.EntityFrameworkCore;
using Package.EFCore.Postgres;
using Service.Catalog.Domain.Aggregates.CategoryAggregate;
using Service.Catalog.Domain.Aggregates.ProductAggregate;
using Service.Catalog.Domain.Aggregates.ShopAggregate;

namespace Service.Catalog.Domain.Persistence;

public class CatalogDbContext : PostgresDbContext<CatalogDbContext>
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> dbContextOptions,
        PostgresDbContextOptions options) : base(dbContextOptions, options)
    { }

    public DbSet<Shop> Shops { get; set; } = null!;
    
    public DbSet<Category> Categories { get; set; } = null!;
    
    public DbSet<Product> Products { get; set; } = null!;
    
    public DbSet<ProductCategory> ProductCategories { get; set; } = null!;
}