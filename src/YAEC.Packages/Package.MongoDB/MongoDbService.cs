using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using Package.MongoDb.Collections;

namespace Package.MongoDb;

public interface IMongoDbService
{
    IMongoCollection<T> Collection<T>();

    Task<long> NextSequenceAsync<T>(CancellationToken cancellationToken = default);

    Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions? options = null,
        CancellationToken cancellationToken = default);
}

public class MongoDbService : IMongoDbService
{
    private readonly MongoClient _client;
    
    private readonly IMongoDatabase _database;

    public MongoDbService(ILogger<MongoDbService> logger, MongoDbOptions options)
    {
        var settings = MongoClientSettings.FromConnectionString(options.Url);
        settings.ClusterConfigurator = builder =>
        {
            builder.Subscribe<CommandStartedEvent>(e =>
            {
                logger.LogInformation("Mongo driver execute command {Name}: {@Command}", e.CommandName, e.Command.ToJson());
            });
        };
        _client = new MongoClient(settings);
        _database = _client.GetDatabase(options.Database);
    }
    
    public IMongoCollection<T> Collection<T>()
    {
        var collectionName = typeof(T).Name;
        return _database.GetCollection<T>(collectionName);
    }

    public async Task<long> NextSequenceAsync<T>(CancellationToken cancellationToken = default)
    {
        var sequenceFilter =  Builders<MongoDbSequence>.Filter.Eq(x => x.CollectionName, typeof(T).Name);
        var sequenceAsyncCursor = await Collection<MongoDbSequence>()
            .FindAsync(sequenceFilter, cancellationToken: cancellationToken);
        var sequence = await sequenceAsyncCursor.FirstOrDefaultAsync(cancellationToken);
        if (sequence is null)
        {
            sequence = new MongoDbSequence
            {
                Id = ObjectId.GenerateNewId().ToString(),
                CollectionName = typeof(T).Name,
                Value = 1,
                CreatedAt = DateTime.UtcNow,
            };
            await Collection<MongoDbSequence>().InsertOneAsync(sequence, cancellationToken: cancellationToken);
        }
        else
        {
            sequence.Value++;
            sequence.ModifiedAt = DateTime.UtcNow;
            await Collection<MongoDbSequence>().UpdateOneAsync(
                sequenceFilter,
                Builders<MongoDbSequence>.Update
                    .Set(x => x.Value, sequence.Value)
                    .Set(x => x.ModifiedAt, sequence.ModifiedAt),
                cancellationToken: cancellationToken);
        }
        return sequence.Value;
    }

    public async Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return await _client.StartSessionAsync(options, cancellationToken);
    }
}