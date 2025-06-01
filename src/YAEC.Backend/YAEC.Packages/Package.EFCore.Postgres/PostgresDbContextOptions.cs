namespace Package.EFCore.Postgres;

public class PostgresDbContextOptions
{
    public string ConnectionString { get; set; } = null!;
    
    public string? Schema { get; set; }
}