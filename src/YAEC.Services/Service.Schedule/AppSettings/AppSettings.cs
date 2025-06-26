using Package.Hangfire;
using Package.Identity;
using YAEC.Shared.Extensions;

namespace Service.Schedule.AppSettings;

public class AppSettings : IAppSettings
{
    public IdentityOptions Identity { get; set; } = null!;
    
    public HangfireOptions Hangfire { get; set; } = null!;
}