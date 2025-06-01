using Microsoft.EntityFrameworkCore;

namespace Package.EFCore.Postgres;

public abstract class PostgresDbContext<TDbContext> : DbContext where TDbContext : DbContext
{
    private readonly PostgresDbContextOptions _postgresOptions;
    
    public PostgresDbContext(DbContextOptions<TDbContext> dbContextOptions, PostgresDbContextOptions options) : base(dbContextOptions)
    {
        _postgresOptions = options;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(_postgresOptions.Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TDbContext).Assembly);
    }
}