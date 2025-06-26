using Package.Identity;
using YAEC.Shared.Extensions;

namespace Service.Sharing.AppSettings;

public class AppSettings : IAppSettings
{
    public IdentityOptions Identity { get; set; } = null!;
}