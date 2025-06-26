namespace Package.Hangfire.Abstractions;

public interface IHangfireScheduleService
{
    Task<string> RunAsync();
}