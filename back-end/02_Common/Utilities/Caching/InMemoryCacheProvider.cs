using System;
using System.Runtime.Caching;

namespace Newegg.MIS.API.Utilities.Caching
{
    public class InMemoryCacheProvider<T>
        : CacheProviderBase<T>, ICacheProvider<T>
        where T : class, new()
    {
        private readonly MemoryCache _cache;
        private const int DefaultExpriationInSeconds = 3600;

        public string RegionName { get; private set; }

        internal InMemoryCacheProvider()
        {
            RegionName = ExtractSlotName();
            _cache = new MemoryCache(RegionName);
        }

        private CacheItemPolicy GetSlidingCacheItemPolicy(int customizedExpiration)
        {
            var expiration = DefaultExpriationInSeconds;

            if (customizedExpiration > 0)
            {
                expiration = customizedExpiration;
            }

            return new CacheItemPolicy
            {
                Priority = CacheItemPriority.Default,
                SlidingExpiration = TimeSpan.FromSeconds(expiration)
            };
        }

        private CacheItemPolicy GetAbsoluteCacheItemPolicy(int customizedExpiration)
        {
            var expiration = DefaultExpriationInSeconds;

            if (customizedExpiration > 0)
            {
                expiration = customizedExpiration;
            }

            return new CacheItemPolicy
            {
                Priority = CacheItemPriority.Default,
                AbsoluteExpiration =
                    DateTimeOffset.Now.AddSeconds(expiration),
            };
        }

        public T Get(string key)
        {
            return (T) _cache.Get(key, null);
        }

        public void Put(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException(
                    "Value key must not be empty to store in cache.");
            }

            _cache.Set(key, value, GetSlidingCacheItemPolicy(-1));
        }

        public void Put(string key, T value, int expiration, bool isAbsolute)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException(
                    "Value key must not be empty to store in cache.");
            }

            if (isAbsolute)
            {
                _cache.Set(key, value, GetAbsoluteCacheItemPolicy(expiration));
                return;
            }

            _cache.Set(key, value, GetSlidingCacheItemPolicy(expiration));
        }

        public T Delete(string key)
        {
            return (T) _cache.Remove(
                key);
        }
    }
}
