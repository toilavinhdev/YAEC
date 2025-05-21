using System.Diagnostics.CodeAnalysis;
using Package.Identity;
using Package.RabbitMQ;
using Package.Redis;
using Package.Shared.Extensions;

namespace Service.Catalog;

public class AppSettings : IAppSettings
{
    public IdentityOptions IdentityOptions { get; set; } = null!;
    
    public RedisOptions RedisOptions { get; set; } = null!;
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public RabbitMQOptions RabbitMQOptions { get; set; } = null!;
}