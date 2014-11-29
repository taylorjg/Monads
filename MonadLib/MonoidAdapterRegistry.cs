using System;
using System.Collections.Generic;

namespace MonadLib
{
    internal class MonoidAdapterRegistry
    {
        private class MonoidAdapterCache
        {
            private readonly Type _monoidAdapterOpenType;
            private readonly IDictionary<Type, object> _dictionary = new Dictionary<Type, object>();

            public MonoidAdapterCache(Type monoidAdapterOpenType)
            {
                _monoidAdapterOpenType = monoidAdapterOpenType;
            }

            public MonoidAdapter<TW> Get<TW>()
            {
                var type = typeof(TW);
                return _dictionary.GetValue(type).Match(
                    obj => (MonoidAdapter<TW>)obj,
                    () =>
                    {
                        var monoidAdapterClosedType = _monoidAdapterOpenType.MakeGenericType(type);
                        var monoidAdapter = (MonoidAdapter<TW>)Activator.CreateInstance(monoidAdapterClosedType);
                        _dictionary[type] = monoidAdapter;
                        return monoidAdapter;
                    });
            }
        }

        private static readonly object LockObject = new object();
        private static readonly IDictionary<string, MonoidAdapterCache> Dictionary = new Dictionary<string, MonoidAdapterCache>();

        static MonoidAdapterRegistry()
        {
            Register(typeof(ListMonoid<>), typeof(ListMonoidAdapter<>));
        }

        public static void Register(Type monoidOpenOrClosedType, Type monoidAdapterOpenType)
        {
            if (!monoidAdapterOpenType.IsGenericTypeDefinition)
            {
                throw new ArgumentException("Monoid adapter must be a generic open type", "monoidAdapterOpenType");
            }

            var key = GetTypeFullNameWithoutGenericBits(monoidOpenOrClosedType);

            lock (LockObject)
            {
                Dictionary[key] = new MonoidAdapterCache(monoidAdapterOpenType);
            }
        }

        public static MonoidAdapter<TW> Get<TW>(Type monoidOpenOrClosedType)
        {
            var key = GetTypeFullNameWithoutGenericBits(monoidOpenOrClosedType);

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

        private static string GetTypeFullNameWithoutGenericBits(Type type)
        {
            var fullName = type.FullName;
            var pos = fullName.IndexOf('[');
            return pos >= 0 ? fullName.Substring(0, pos) : fullName;
        }
    }
}
