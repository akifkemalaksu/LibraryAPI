using LibraryAPI.Settings;
using StackExchange.Redis;

namespace LibraryAPI.Caching
{
    public class RedisConnector
    {
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        public RedisConnector(RedisSettings redisSettings)
        {
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect($"{redisSettings.Host}:{redisSettings.Port}"));
        }

        public IDatabase GetDatabase(int db = 1) => _connectionMultiplexer.Value.GetDatabase(db);
    }
}
