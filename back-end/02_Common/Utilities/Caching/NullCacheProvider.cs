namespace Newegg.MIS.API.Utilities.Caching
{
    public class NullCacheProvider<T> : CacheProviderBase<T>, ICacheProvider<T>
        where T : class, new()
    {
        public T Get(string key)
        {
            return null;
        }

        public void Put(string key, T value)
        {
        }

        public void Put(string key, T value, int expiration, bool isAbsolute)
        {
        }

        public T Delete(string key)
        {
            return null;
        }
    }
}
