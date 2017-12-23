using System;

namespace Newegg.MIS.API.Utilities.Caching
{
    public abstract class CacheProviderBase<TEntity>
        where TEntity : class, new()
    {
        private static CacheAttribute GetCacheAttribute(Type entityType)
        {
            var attributes
                = entityType.GetCustomAttributes(typeof(CacheAttribute), true);

            if (attributes.Length == 0) return null;

            return attributes[0] as CacheAttribute;
        }

        protected string ExtractSlotName()
        {
            return ExtractSlotName(typeof(TEntity));
        }

        protected string ExtractSlotName(Type entityType)
        {
            var slot = entityType.FullName;

            var attribute = GetCacheAttribute(entityType);

            if (null != attribute &&
                !string.IsNullOrWhiteSpace(attribute.Slot))
            {
                slot = attribute.Slot.Trim();
            }

            return slot;
        }
    }
}
