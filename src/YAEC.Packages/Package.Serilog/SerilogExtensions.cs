using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Package.Serilog;

public static class SerilogExtensions
{
    public static void AddCoreSerilog(this IServiceCollection services)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        services.AddSerilog(logger);
    }
}