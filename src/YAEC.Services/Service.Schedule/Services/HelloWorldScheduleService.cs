using Package.Hangfire.Abstractions;
using Package.Hangfire.Attributes;

namespace Service.Schedule.Services;

[HangfireCron("*/30 * * * *", "Execute every 30 minute")]
public class HelloWorldScheduleService : IHangfireScheduleService
{
    private readonly ILogger<HelloWorldScheduleService> _logger;

    public HelloWorldScheduleService(ILogger<HelloWorldScheduleService> logger)
    {
        _logger = logger;
    }

    public async Task<string> RunAsync()
    {
        _logger.LogInformation("Starting job: {Job}", nameof(HelloWorldScheduleService));
        await Task.CompletedTask;
        _logger.LogInformation("Hangfire schedule job: Hello World!");
        _logger.LogInformation("Finished job: {Job}", nameof(HelloWorldScheduleService));
        return string.Empty;
    }
}