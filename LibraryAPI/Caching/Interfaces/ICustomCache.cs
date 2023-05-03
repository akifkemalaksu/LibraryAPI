namespace LibraryAPI.Caching.Interfaces
{
    public interface ICustomCache
    {
        public T Get<T>(string key);
        public void Set<T>(string key, T value);
        public void Set<T>(string key, T value, DateTimeOffset expirationTime);
        public void Remove(string key);
    }
}
