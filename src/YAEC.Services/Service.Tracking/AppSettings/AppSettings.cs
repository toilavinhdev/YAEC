using Package.Identity;
using Package.MongoDb;
using YAEC.Shared.Extensions;

namespace Service.Tracking.AppSettings;

public class AppSettings : IAppSettings
{
    public IdentityOptions Identity { get; set; } = null!;
    
    public MongoDbOptions MongoDb { get; set; } = null!;
}