namespace Newegg.MIS.API.Utilities.Caching
{
    public class CacheProviderWrapper<TEntity> :
        ICacheProvider<TEntity>
        where TEntity : class, new()
    {
        private readonly ICacheProvider<TEntity> _level1;
        private readonly ICacheProvider<TEntity> _level2;

        public CacheProviderWrapper(
            ICacheProvider<TEntity> level1,
            ICacheProvider<TEntity> level2
            )
        {
            _level1 = level1;
            _level2 = level2;
        }

        public TEntity Get(string key)
        {
            var entity = _level1.Get(key);

            if (null != entity) return entity;

            entity = _level2.Get(key);

            if (null == entity) return null;

            // Refill level 1 cache
            _level1.Put(key, entity);

            return entity;
        }

        public void Put(string key, TEntity value)
        {
            _level1.Put(key, value);
            _level2.Put(key, value);
        }

        public void Put(string key, TEntity value, int expiration, bool isAbsolute)
        {
            _level1.Put(key, value, expiration, isAbsolute);
            _level2.Put(key, value, expiration, isAbsolute);
        }


        public TEntity Delete(string key)
        {
            var entity = _level1.Delete(key);

            var entityLevel2 = _level2.Delete(key);

            return entity ?? entityLevel2;
        }
    }
}
