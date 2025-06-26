namespace Package.Redis.Events;

public abstract class RedisPubSubEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();

    public string CorrelationId { get; set; } = default!;
}