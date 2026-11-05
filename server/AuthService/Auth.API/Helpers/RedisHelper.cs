using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Auth.API.Helpers
{
    public class RedisHelper
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<RedisHelper> _logger;
        public RedisHelper(IDistributedCache distributedCache, ILogger<RedisHelper> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }
        public async Task<T> GetRedisAsync<T>(string key)
        {
            var data = await _distributedCache.GetStringAsync(key);
            if (data == null)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(data);
        }
        public async Task SetRedisAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var data = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, data, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry
            });
        }
        public async Task RemoveRedisAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
    }
}
