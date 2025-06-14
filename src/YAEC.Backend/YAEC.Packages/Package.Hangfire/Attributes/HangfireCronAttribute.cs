namespace Package.Hangfire.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class HangfireCronAttribute(string cronExpression, string? description = null) : Attribute
{
    public string CronExpression { get; set; } = cronExpression;

    public string? Description { get; set; } = description;
}