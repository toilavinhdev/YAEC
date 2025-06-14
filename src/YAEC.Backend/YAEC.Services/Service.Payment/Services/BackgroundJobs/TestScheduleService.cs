using Package.Hangfire.Abstractions;
using Package.Hangfire.Attributes;

namespace Service.Payment.Services.BackgroundJobs;

[HangfireCron("*/1 * * * *", "Execute at every 1 minutes")]
public class TestScheduleService(ILogger<TestScheduleService> logger) : IHangfireScheduleService
{
    public async Task<string> RunAsync()
    {
        await Task.CompletedTask;
        logger.LogInformation("Hangfire is running.");
        return string.Empty;
    }
}