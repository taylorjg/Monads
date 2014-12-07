using System;
using System.Collections.Generic;

namespace MonadLib.Registries
{
    public static class MonoidAdapterRegistry
    {
        private class MonoidAdapterCache
        {
            private readonly Type _monoidAdapterType;
            private readonly IDictionary<Type, object> _dictionary = new Dictionary<Type, object>();

            public MonoidAdapterCache(Type monoidAdapterType)
            {
                _monoidAdapterType = MonadHelpers.GetTypeOrGenericTypeDefinition(monoidAdapterType);
            }

            public MonoidAdapter<TW> Get<TW>()
            {
                var type = typeof(TW);
                return _dictionary.GetValue(type).Match(
                    obj => (MonoidAdapter<TW>) obj,
                    () =>
                        {
                            var monoidAdapterClosedType = _monoidAdapterType.MakeGenericType(type);
                            var monoidAdapter = (MonoidAdapter<TW>) Activator.CreateInstance(monoidAdapterClosedType);
                            _dictionary[type] = monoidAdapter;
                            return monoidAdapter;
                        });
            }
        }

        private static readonly object LockObject = new object();
        private static readonly IDictionary<Type, MonoidAdapterCache> Dictionary = new Dictionary<Type, MonoidAdapterCache>();

        static MonoidAdapterRegistry()
        {
            Register(typeof(ListMonoid<>), typeof(ListMonoidAdapter<>));
        }

        public static void Register(Type monoidType, Type monoidAdapterType)
        {
            var key = MonadHelpers.GetTypeOrGenericTypeDefinition(monoidType);

            lock (LockObject)
            {
                Dictionary[key] = new MonoidAdapterCache(monoidAdapterType);
            }
        }

        public static MonoidAdapter<TW> Get<TW>(Type monoidType)
        {
            var key = MonadHelpers.GetTypeOrGenericTypeDefinition(monoidType);

            lock (LockObject)
            {
                var value = Dictionary.GetValue(key);

                return value.Match(
                    monoidAdapterCache => monoidAdapterCache.Get<TW>(),
                    () =>
                        {
                            throw new InvalidOperationException(string.Format("Could not find a registered monoid adapter for {0}", key));
                        });
            }
        }
    }
}
