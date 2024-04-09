namespace CodeFirstApi.Interfaces
{
    public interface IRedisCache
    {
        Task<T> GetCacheData<T>(string key);
        Task<object> RemoveCacheData<T>(string key);
        Task<bool> SetCacheData<T>(string key, T value,DateTimeOffset expirationTime);
    }
}
