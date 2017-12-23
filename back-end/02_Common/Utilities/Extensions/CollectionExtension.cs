using System.Collections.Generic;
using System.Linq;

namespace Newegg.MIS.API.Utilities.Extensions
{
    public static class CollectionExtension
    {
        public static bool IsNotEmpty<TEntity>(this List<TEntity> list)
        {
            if (null == list) return false;
            return list.Count > 0;
        }

        public static bool IsEmpty<TEntity>(this List<TEntity> list)
        {
            if (null == list) return true;
            return list.Count == 0;
        }

        public static bool IsNotEmpty<TEntity>(this IEnumerable<TEntity> list)
        {
            if (null == list) return false;
            return list.Any();
        }

        public static bool IsEmpty<TEntity>(this IEnumerable<TEntity> list)
        {
            if (null == list) return true;
            return !list.Any();
        }
    }
}
