#region Using directives

using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

#endregion


namespace Api.Client
{
    public interface ICacheManager
    {
        Task<TObject> GetWithCachingAsync<TObject>(string key, Func<Task<TObject>> function, bool forceRefresh = false) where TObject : class;
    }

    public class CacheManager : ICacheManager
    {
        private static readonly MemoryCache _cache = new MemoryCache(Guid.NewGuid().ToString());

        public async Task<TObject> GetWithCachingAsync<TObject>(string key, Func<Task<TObject>> function, bool forceRefresh)
            where TObject : class
        {
            TObject objectInstance = null;

            // Try to get the value from cache.
            if (!forceRefresh)
            {
                objectInstance = (TObject) _cache.Get(key);
            }

            if (forceRefresh || objectInstance == null)
            {
                objectInstance = await function();
                var policy = new CacheItemPolicy();
                _cache.Add(key, objectInstance, policy);
            }

            return objectInstance;
        }

    }


    
}