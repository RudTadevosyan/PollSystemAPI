using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using ResultAggregation.Domain.Interfaces;

namespace ResultAggregation.Infrastructure.Repositories;

public class PollResultRepository : IPollResultRepository
{
    private readonly IMongoCollection<BsonDocument> _collection;

    public PollResultRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<BsonDocument>("PollResults");
    }

    public async Task SetPollResultAsync(int pollId, object pollResult)
    {
        var json = JsonSerializer.Serialize(pollResult);
        var filter = Builders<BsonDocument>.Filter.Eq("PollId", pollId);
        var update = Builders<BsonDocument>.Update.Set("PollResult", json);
        
        await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public async Task<T?> GetPollResultAsync<T>(int pollId)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("PollId", pollId);
        var doc = await _collection.Find(filter).FirstOrDefaultAsync();
        
        if (doc == null) return default;
        
        return JsonSerializer.Deserialize<T>(doc["PollResult"].AsString);
    }

    public async Task DeletePollResultAsync(int pollId)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("PollId", pollId);
        await _collection.DeleteOneAsync(filter);
    }
}