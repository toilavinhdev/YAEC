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
            config.UseMemoryStorage();
        });
        services.AddHangfireServer(options =>
        {
            options.Queues = ["default"];
            options.WorkerCount = 100;
        });
    }
    
    public static void UseCoreHangfire(this WebApplication app)
    {
        app.UseCoreHangfireDashboard();
        app.UseCoreHangfireCronJobs();
    }

    private static void UseCoreHangfireDashboard(this WebApplication app, string pathMatch = "/hangfire")
    {
        var options = app.Services.GetRequiredService<HangfireOptions>();
        app.UseHangfireDashboard(pathMatch, new DashboardOptions
        {
            DashboardTitle = options.Title,
            Authorization =
            [
                new HangfireCustomBasicAuthenticationFilter
                {
                    User = options.AuthenticationOptions.UserName,
                    Pass = options.AuthenticationOptions.Password
                }
            ],
            IgnoreAntiforgeryToken = true
        });
    }

    private static void UseCoreHangfireCronJobs(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var jobs = scope.ServiceProvider.GetServices<IHangfireScheduleService>();
        foreach (var job in jobs)
        {
            var name = job.GetType().FullName;
            var attribute = job.GetType().GetCustomAttribute<HangfireCronAttribute>();
            if (attribute is null) continue;
            var cronExpression = attribute.CronExpression;
            RecurringJob.AddOrUpdate(
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