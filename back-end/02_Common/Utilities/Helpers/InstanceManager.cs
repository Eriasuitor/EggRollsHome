using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Newegg.MIS.API.Utilities.Helpers
{
    public static class InstanceManager
    {
        public class InstanceEntry<TInterface>
        {
            public TInterface Instance { get; set; }
            public Lazy<TInterface> InstanceBuilder { get; set; }

            public void Use(TInterface instance)
            {
                Instance = instance;

                AddOrUpdate(this);
            }

            public void Reset()
            {
                Instance = default(TInterface);
                AddOrUpdate(this);
            }
        }

        private static readonly ConcurrentDictionary<Type, object> Pool =
            new ConcurrentDictionary<Type, object>();

        public static void RegisterBuilder<TInterface, TInstance>(
            Func<TInstance> instanceBuilder)
            where TInstance : class, TInterface
        {
            if (null == instanceBuilder)
            {
                throw new ArgumentNullException(
                    string.Format(
                        "An instance builder of type {0} must be provided",
                        typeof (TInterface).FullName),
                    "instanceBuilder");
            }

            var instanceEntry = GetOrCreate<TInterface>(
                e => e.InstanceBuilder = new Lazy<TInterface>(
                    instanceBuilder,
                    LazyThreadSafetyMode.ExecutionAndPublication
                    ));

            Pool.AddOrUpdate(typeof (TInterface),
                key => instanceEntry,
                (key, old) => instanceEntry
                );
        }

        private static InstanceEntry<TInterface> GetOrCreate<TInterface>(
            Action<InstanceEntry<TInterface>> action)
        {
            object entry;

            InstanceEntry<TInterface> instanceEntry = null;

            if (Pool.TryGetValue(typeof (TInterface), out entry))
            {
                instanceEntry = entry as InstanceEntry<TInterface>;
            }

            if (instanceEntry == null)
            {
                instanceEntry = new InstanceEntry<TInterface>();
            }

            if (null != action)
            {
                action(instanceEntry);
            }

            return instanceEntry;
        }

        public static InstanceEntry<TInterface> Register<TInterface>()
        {
            var instanceEntry = GetOrCreate<TInterface>(null);

            return instanceEntry;
        }

        private static void AddOrUpdate<TInterface>(InstanceEntry<TInterface> entry)
        {
            Pool.AddOrUpdate(typeof (TInterface),
                key => entry,
                (key, old) => entry
                );
        }

        public static TInterface GetInstance<TInterface>()
        {
            var interfaceType = typeof (TInterface);

            if (!interfaceType.IsInterface)
            {
                throw new InvalidOperationException(
                    string.Format("The type parameter TInterface must be an Interface. " +
                                  "You have specified a {0}",
                        typeof (TInterface).FullName));
            }

            object entry;
            if (!Pool.TryGetValue(typeof (TInterface), out entry))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Unable to get instance of {0}, " +
                        "please make sure it get registered before get its instance.",
                        typeof (TInterface).FullName));
            }

            if (null == entry)
            {
                throw new InvalidOperationException(
                    string.Format(
                        "Unable to get instance of {0}, " +
                        "please make sure it get registered before get its instance.",
                        typeof (TInterface).FullName));
            }

            var instanceEntry = entry as InstanceEntry<TInterface>;

            if (null == instanceEntry)
            {
                throw new InvalidOperationException(
                    string.Format("Unable to cast type {0} into InstanceEntry<{1}>.",
                        entry.GetType().Name,
                        typeof (TInterface).Name));
            }

            if (null == instanceEntry.Instance &&
                null == instanceEntry.InstanceBuilder)
            {
                throw new InvalidOperationException(
                    string.Format("The instance of registered type {0} null, " +
                                  "and the instance builder of type {0} is also null.",
                        typeof (TInterface).Name));
            }

            if (null == instanceEntry.Instance)
            {
                instanceEntry.Instance = instanceEntry.InstanceBuilder.Value;
            }

            return instanceEntry.Instance;
        }
    }
}
