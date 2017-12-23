namespace Newegg.MIS.API.Utilities.Caching
{
    public interface ICacheProvider<T> where T : class, new()
    {
        T Get(string key);
        void Put(string key, T value);
        void Put(string key, T value, int expiration, bool isAbsolute = false);
        T Delete(string key);
    }
}
