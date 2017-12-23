using System;
using System.Collections.Concurrent;
using EggKeeper.Sdk.Core;

namespace Newegg.MIS.API.Utilities.Caching
{
    public enum CacheProviderType
    {
        InMemory
    }

    public static class CacheFactory
    {
        private static readonly ConcurrentDictionary<Type, object>
            Providers = new ConcurrentDictionary<Type, object>();

        public static ICacheProvider<TEntity> CreateCacheProvider<TEntity>()
            where TEntity : class, new()
        {
            var provider = Providers.GetOrAdd(
                typeof (TEntity),
                type => new CacheProviderWrapper<TEntity>(
                    new InMemoryCacheProvider<TEntity>(),
                    new NullCacheProvider<TEntity>() // Cloud Store Cache Not yet implemented
                    )) as ICacheProvider<TEntity>;

            if (null != provider) return provider;

            throw new InvalidOperationException(
                "Failed to retrieve cache provider. Something bad happend.");
        }

        public static ICacheProvider<TEntity> SetCacheProvider<TEntity>(ICacheProvider<TEntity> provider)
            where TEntity : class, new()
        {
            Providers.AddOrUpdate(typeof (TEntity), key => provider, (key, val) => provider);

            return provider;
        }

        public static ICacheProvider<TEntity> Reset<TEntity>() where TEntity : class, new()
        {
            return Providers.GetAndRemove(typeof (TEntity)) as ICacheProvider<TEntity>;
        }
    }
}
