using System.Collections.Generic;
using System.Linq;

namespace Newegg.MIS.API.Utilities.Extensions
{
    public static class LinqExtension
    {
        /// <summary>
        /// Return null if the enumerable is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static List<T> ToListNullSafe<T>(this IEnumerable<T> enumerable)
        {
            if (null == enumerable) return null;
            return enumerable.ToList();
        }
    }
}
