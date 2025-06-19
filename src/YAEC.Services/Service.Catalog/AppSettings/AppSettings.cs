using Package.Identity;
using YAEC.Shared.Extensions;

namespace Service.Catalog.AppSettings;

public class AppSettings : IAppSettings
{
    public IdentityOptions Identity { get; set; } = null!;
}