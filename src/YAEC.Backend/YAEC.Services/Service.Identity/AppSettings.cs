using Package.Identity;
using Package.MongoDb;
using Package.Shared.Extensions;

namespace Service.Identity;

public class AppSettings : IAppSettings
{
    public IdentityOptions IdentityOptions { get; set; } = null!;
    
    public MongoDbOptions MongoDbOptions { get; set; } = null!;
}