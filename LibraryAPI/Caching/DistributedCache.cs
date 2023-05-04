using LibraryAPI.Caching.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace LibraryAPI.Caching
{
    public class DistributedCache : ICustomCache
    {
        private readonly IDistributedCache _distributedCache;

        public DistributedCache(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public T Get<T>(string key)
        {
            var dataAsJsonString = _distributedCache.GetString(key);
            if (string.IsNullOrEmpty(dataAsJsonString)) return default;

            var data = JsonSerializer.Deserialize<T>(dataAsJsonString);
            return data;
        }

        public void Remove(string key) => _distributedCache.Remove(key);

        public void Set<T>(string key, T value)
        {
            var dataAsJsonString = JsonSerializer.Serialize(value);
            _distributedCache.SetString(key, dataAsJsonString);
        }

        public void Set<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var dataAsJsonString = JsonSerializer.Serialize(value);

            var distributedCacheEntryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expirationTime
            };

            _distributedCache.SetString(key, dataAsJsonString, distributedCacheEntryOptions);
        }
    }
}
