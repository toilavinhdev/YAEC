using System.Text.Json;
using Microsoft.Extensions.Logging;
using Package.Redis.Events;
using StackExchange.Redis;

namespace Package.Redis;

public interface IRedisService
{
    Task<bool> ExistsAsync(string key);

    Task<bool> KeyDeleteAsync(string key);

    Task<List<string>> KeysByPatternAsync(string pattern);
    
    Task<string?> StringGetAsync(string key);

    Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = null);
    
    Task ListLeftPushAsync(string key, string value);

    Task<RedisValue?> ListLeftPopAsync(string key);

    Task ListRightPushAsync(string key, string value);

    Task<RedisValue?> ListRightPopAsync(string key);

    Task<RedisValue[]> ListRangeAsync(string key, long start, long stop);

    Task<bool> SetAddAsync(string key, string value);

    Task<bool> SetRemoveAsync(string key, string value);

    Task<bool> SetContainsAsync(string key, string value);

    Task<long> SetLengthAsync(string key);
    
    Task<long> IncrementAsync(string key);

    Task<long> DecrementAsync(string key);
    
    Task Transaction(Func<ITransaction, Task> callback);

    Task<long> PublishAsync<TEvent>(TEvent message) where TEvent : RedisPubSubEvent;

    Task SubscribeAsync<TEvent>(Func<TEvent, Task> callback) where TEvent : RedisPubSubEvent;
}

public class RedisService : IRedisService
{
    private readonly ILogger<RedisService> _logger;

    private readonly IConnectionMultiplexer _connection;

    private IDatabase Database() => _connection.GetDatabase();

    private IServer Server()
    {
        foreach (var endpoint in _connection.GetEndPoints())
        {
            var server = _connection.GetServer(endpoint);
            if (!server.IsReplica) return server;
        }
        throw new RedisException("Redis master database was not found");
    }
    
    private ISubscriber Subscriber() => _connection.GetSubscriber();

    public RedisService(ILogger<RedisService> logger, IConnectionMultiplexer connection)
    {
        _connection = connection;
        _logger = logger;
    }
    
    public async Task<bool> ExistsAsync(string key)
    {
        return await Database().KeyExistsAsync(key);
    }
    
    public async Task<bool> KeyDeleteAsync(string key)
    {
        return await Database().KeyDeleteAsync(key);
    }
    
    public async Task<List<string>> KeysByPatternAsync(string pattern)
    {
        var asyncKeys = Server().KeysAsync(database: -1, pattern: pattern);
        var keys = new List<string>();
        await foreach (var key in asyncKeys) keys.Add(key.ToString());
        return keys;
    }

    public async Task<string?> StringGetAsync(string key)
    {
        var database = Database();
        var value = await database.StringGetAsync(key);
        return value.ToString();
    }

    public async Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = null)
    {
        var database = Database();
        return await database.StringSetAsync(key, value, expiry);
    }
    
    public async Task ListLeftPushAsync(string key, string value)
    {
        await Database().ListLeftPushAsync(key, value);
    }
    
    public async Task<RedisValue?> ListLeftPopAsync(string key)
    {
        return await Database().ListLeftPopAsync(key);
    }
    
    public async Task ListRightPushAsync(string key, string value)
    {
        await Database().ListRightPushAsync(key, value);
    }
    
    public async Task<RedisValue?> ListRightPopAsync(string key)
    {
        return await Database().ListRightPopAsync(key);
    }
    
    public async Task<RedisValue[]> ListRangeAsync(string key, long start, long stop)
    {
        return await Database().ListRangeAsync(key, start, stop);
    }
    
    public async Task<bool> SetAddAsync(string key, string value)
    {
        return await Database().SetAddAsync(key, value);
    }
    
    public async Task<bool> SetRemoveAsync(string key, string value)
    {
        return await Database().SetRemoveAsync(key, value);
    }
    
    public async Task<bool> SetContainsAsync(string key, string value)
    {
        return await Database().SetContainsAsync(key, value);
    }
    
    public async Task<long> SetLengthAsync(string key)
    {
        return await Database().SetLengthAsync(key);
    }
    
    public async Task<long> IncrementAsync(string key)
    {
        return await Database().StringIncrementAsync(key);
    }

    public async Task<long> DecrementAsync(string key)
    {
        return await Database().StringDecrementAsync(key);
    }
    
    public async Task Transaction(Func<ITransaction, Task> callback)
    {
        var transaction = Database().CreateTransaction();
        await callback(transaction);
        await transaction.ExecuteAsync();
    }

    public async Task<long> PublishAsync<TEvent>(TEvent message) where TEvent : RedisPubSubEvent
    {
        var channel = new RedisChannel(typeof(TEvent).Name, RedisChannel.PatternMode.Auto);
        var json = JsonSerializer.Serialize(message);
        var result = await Subscriber().PublishAsync(channel, json);
        _logger.LogInformation("Published event {EventName} to [Redis]: {EventPayload}", typeof(TEvent).Name, json);
        return result;
    }

    public async Task SubscribeAsync<TEvent>(Func<TEvent, Task> callback) where TEvent : RedisPubSubEvent
    {
        var channel = new RedisChannel(typeof(TEvent).Name, RedisChannel.PatternMode.Auto);
        await Subscriber().SubscribeAsync(channel, (_, value) =>
        {
            var @event = JsonSerializer.Deserialize<TEvent>(value.ToString());
            if (@event is null) return;
            using var scope = _logger.BeginScope("{@CorrelationId}", @event.CorrelationId);
            _logger.LogInformation("Received event {EventName} from [Redis]: {EventPayload}", typeof(TEvent).Name, value.ToString());
            callback(@event);
            _logger.LogInformation("Processed event {EventName} from [Redis]", typeof(TEvent).Name);
        });
    }
}