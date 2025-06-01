using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Package.EFCore.Postgres;

public static class PostgresDbContextExtensions
{
    private const string MigrationsHistoryTable = "__EFMigrationsHistory";
    
    private const string DefaultSchema = "public";
    
    public static void AddPostgresDbContext<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        var postgresOptions = services.BuildServiceProvider()
            .GetRequiredService<PostgresDbContextOptions>();
        var schema = string.IsNullOrEmpty(postgresOptions.Schema) ? DefaultSchema : postgresOptions.Schema;
        services.AddDbContext<TDbContext>(options =>
        {
            options.UseNpgsql(postgresOptions.ConnectionString, builder =>
            {
                builder.MigrationsAssembly(typeof(TDbContext).Assembly.GetName().FullName);
                builder.MigrationsHistoryTable(MigrationsHistoryTable, schema);
            });
        });
    }
}