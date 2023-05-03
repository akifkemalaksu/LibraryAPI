using LibraryAPI.Caching.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace LibraryAPI.Caching
{
    public class RedisCache : ICustomCache
    {
        private readonly IDatabase _database;

        public RedisCache(RedisConnector redisConnector)
        {
            _database = redisConnector.GetDatabase();
        }

        public T Get<T>(string key)
        {
            var dataAsJsonString = _database.StringGet(key);
            if (string.IsNullOrEmpty(dataAsJsonString)) return default;

            var data = JsonSerializer.Deserialize<T>(dataAsJsonString);
            return data;
        }

        public void Remove(string key)
        {
            if (_database.KeyExists(key))
                _database.KeyDelete(key);
        }

        public void Set<T>(string key, T value)
        {
            var dataAsJsonString = JsonSerializer.Serialize(value);
            _database.StringSet(key, dataAsJsonString);
        }

        public void Set<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var dataAsJsonString = JsonSerializer.Serialize(value);
            var timeSpan = expirationTime.DateTime.Subtract(DateTime.Now);
            _database.StringSet(key, dataAsJsonString, timeSpan);
        }
    }
}
