using System.Diagnostics.CodeAnalysis;
using Package.Identity;
using Package.MongoDb;
using Package.RabbitMQ;
using Package.Shared.Extensions;

namespace Service.Identity;

public class AppSettings : IAppSettings
{
    public IdentityOptions IdentityOptions { get; set; } = null!;
    
    public MongoDbOptions MongoDbOptions { get; set; } = null!;
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public RabbitMQOptions RabbitMQOptions { get; set; } = null!;
}