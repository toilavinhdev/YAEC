using Package.Identity;
using Package.ObjectStorage;
using Package.Shared.Extensions;

namespace Service.Storage;

public class AppSettings : IAppSettings
{
    public IdentityOptions IdentityOptions { get; set; } = null!;
    
    public S3Options S3Options { get; set; } = null!;
}