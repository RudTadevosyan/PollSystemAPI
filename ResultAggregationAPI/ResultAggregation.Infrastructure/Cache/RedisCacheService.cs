using StackExchange.Redis;

namespace ResultAggregation.Infrastructure.Cache
{
    public class RedisCacheService
    {
        private readonly IDatabase _db; // redis database cache 
        
        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        // User email cache
        public async Task SetUserEmailAsync(int userId, string email, TimeSpan? ttl = null)
        {
            var key = $"user:{userId}";
            await _db.StringSetAsync(key, email, ttl ?? TimeSpan.FromHours(24));
        }

        public async Task<string?> GetUserEmailAsync(int userId)
        {
            var key = $"user:{userId}";
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty) return null;
            return value;
        }
    }
}
