using System.Security.Cryptography;
using System.Text;
using Newegg.MIS.API.Utilities.Extensions;

namespace Newegg.MIS.API.Utilities.Caching
{
    public static class CacheHelper
    {
        //must modify if difference
        private const string KeyPrefix = "EDICommonAPI";

        private const string HashKey =
            "bd1f4ba58ad04a29aa5afce720a60f4b04926358e5074134ae057e30a83a5411" +
            "88b2271ca25e40f288b4d2ae0966a1bb022136cbbb334c1488a815cfebd47ca2";

        public static T Get<T>(string key) where T : class, new()
        {
            var provider = CacheFactory.CreateCacheProvider<T>();

            return provider.Get(key);
        }

        public static void Put<T>(string key, T value)
            where T : class, new()
        {
            var provider = CacheFactory.CreateCacheProvider<T>();

            provider.Put(key, value);
        }

        public static void Put<T>(string key, T value, int expiration, bool isAbsolute = false)
            where T : class, new()
        {
            var provider = CacheFactory.CreateCacheProvider<T>();

            provider.Put(key, value, expiration, isAbsolute);
        }

        public static T Delete<T>(string key)
            where T : class, new()
        {
            var provider = CacheFactory.CreateCacheProvider<T>();

            return provider.Delete(key);
        }

        public static string ToCacheKey<T>(string extension)
        {
            var content = string.Format("{0}|{1}|{2}", KeyPrefix, typeof (T).FullName, extension);

            using (var hash = new HMACMD5(Encoding.UTF8.GetBytes(HashKey)))
            {
                var hashBytes =
                    hash.ComputeHash(Encoding.UTF8.GetBytes(content));

                return hashBytes.ToHexString(true);
            }
        }

        public static string ToCacheKey<T>(string keyPrefix, string extension)
        {
            var content = string.Format("{0}|{1}|{2}", keyPrefix, typeof (T).FullName, extension);

            using (var hash = new HMACMD5(Encoding.UTF8.GetBytes(HashKey)))
            {
                var hashBytes =
                    hash.ComputeHash(Encoding.UTF8.GetBytes(content));

                return hashBytes.ToHexString(true);
            }
        }
    }
}
