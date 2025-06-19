using Package.Identity;
using Package.Redis;
using YAEC.Shared.Extensions;

namespace Service.Catalog.AppSettings;

public class AppSettings : IAppSettings
{
    public IdentityOptions Identity { get; set; } = null!;
    
    public RedisOptions Redis { get; set; } = null!;
}