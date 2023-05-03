using LibraryAPI.Caching.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace LibraryAPI.Caching
{
    public class InMemoryCache : ICustomCache
    {
        private readonly IMemoryCache _memoryCache;

        public InMemoryCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T Get<T>(string key) => _memoryCache.Get<T>(key);

        public void Remove(string key) => _memoryCache.Remove(key);

        public void Set<T>(string key, T value) => _memoryCache.Set(key, value);
        public void Set<T>(string key, T value, DateTimeOffset expirationTime) => _memoryCache.Set(key, value, expirationTime);
    }
}
