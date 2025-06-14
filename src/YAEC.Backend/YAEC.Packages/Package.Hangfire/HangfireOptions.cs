namespace Package.Hangfire;

public class HangfireOptions
{
    public string? Title { get; set; }
    
    public HangfireStorageOptions StorageOptions { get; set; } = null!;
    
    public HangfireAuthenticationOptions AuthenticationOptions { get; set; } = null!;
}

public class HangfireStorageOptions
{
    public string? ConnectionString { get; set; }
}

public class HangfireAuthenticationOptions
{
    public string UserName { get; set; } = null!;
    
    public string Password { get; set; } = null!;
}