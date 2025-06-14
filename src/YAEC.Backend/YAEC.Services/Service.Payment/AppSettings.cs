using Package.Hangfire;
using Package.Identity;
using Package.Payments.VnPay;
using Package.Shared.Extensions;

namespace Service.Payment;

public class AppSettings : IAppSettings
{
    public IdentityOptions IdentityOptions { get; set; } = null!;
    
    public HangfireOptions HangfireOptions { get; set; } = null!;
    
    public VnPayOptions VnPayOptions { get; set; } = null!;
}