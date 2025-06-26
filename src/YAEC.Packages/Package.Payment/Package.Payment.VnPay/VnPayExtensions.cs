using Microsoft.Extensions.DependencyInjection;

namespace Package.Payment.VnPay;

public static class VnPayExtensions
{
    public static void AddVnPay(this IServiceCollection services)
    {
        services.AddSingleton<IVnPayService, VnPayService>();
    }
}