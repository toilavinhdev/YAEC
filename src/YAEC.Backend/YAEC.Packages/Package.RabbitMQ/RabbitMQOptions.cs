using System.Diagnostics.CodeAnalysis;

namespace Package.RabbitMQ;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class RabbitMQOptions
{
    public string Host { get; set; } = null!;
    
    public int Port { get; set; }
    
    public string UserName { get; set; } = null!;
    
    public string Password { get; set; } = null!;
    
    public string? VirtualHost { get; set; }
}