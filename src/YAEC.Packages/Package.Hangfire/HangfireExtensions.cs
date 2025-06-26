using System.Reflection;
using Hangfire;
using Hangfire.MemoryStorage;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Package.Hangfire.Abstractions;
using Package.Hangfire.Attributes;

namespace Package.Hangfire;

public static class HangfireExtensions
{
    public static void AddCoreHangfire(this IServiceCollection services)
    {
        services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
            config.UseSimpleAssemblyNameTypeSerializer();
            config.UseRecommendedSerializerSettings();
            config.UseMemoryStorage();
        });
        services.AddHangfireServer();
    }

    public static void UseCoreHangfireDashboard(this WebApplication app, string? pathMatch = null)
    {
        var options = app.Services.GetRequiredService<HangfireOptions>();
        app.UseHangfireDashboard(pathMatch ?? "/hangfire", new DashboardOptions
        {
            DashboardTitle = "Hangfire",
            Authorization =
            [
                new HangfireCustomBasicAuthenticationFilter
                {
                    User = options.UserName,
                    Pass = options.Password
                }
            ],
            IgnoreAntiforgeryToken = true
        });
    }

    public static void UseCoreHangfireScheduleJobs(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var jobs = serviceProvider.GetServices<IHangfireScheduleService>();
        var recurringJobManager = serviceProvider.GetService<IRecurringJobManager>();
        foreach (var job in jobs)
        {
            var name = job.GetType().FullName;
            var attribute = job.GetType().GetCustomAttribute<HangfireCronAttribute>();
            if (attribute is null) continue;
            var cronExpression = attribute.CronExpression;
            recurringJobManager.AddOrUpdate(
                recurringJobId: name,
                methodCall: () => job.RunAsync(),
                cronExpression: cronExpression,
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Utc
                });
        }
    }
}