using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Interfaces
{
    public interface IRedisCacheService
    {
        Task<T> GetCacheAsync<T>(string key); 
        Task<bool> SetCacheAsync<T>(string key, T value); //, DateTimeOffset expirationTime  Task int? expiryMinutes = null 
        Task<object> RemoveCacheAsync(string key);
    }
}