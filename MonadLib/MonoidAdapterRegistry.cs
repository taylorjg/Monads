using System;
using System.Collections.Generic;

namespace MonadLib
{
    internal class MonoidAdapterRegistry
    {
        private static readonly object LockObject = new object();
        private static readonly IDictionary<string, Type> Dictionary = new Dictionary<string, Type>();

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
                Dictionary[key] = monoidAdapterOpenType;
            }
        }

        public static MonoidAdapter<TW> Get<TW>(Type monoidOpenOrClosedType)
        {
            var key = GetTypeFullNameWithoutGenericBits(monoidOpenOrClosedType);

            lock (LockObject)
            {
                var value = Dictionary.GetValue(key);

                return value.Match(
                    monoidAdapterOpenType =>
                        {
                            var monoidAdapterClosedType = monoidAdapterOpenType.MakeGenericType(typeof (TW));
                            return (MonoidAdapter<TW>) Activator.CreateInstance(monoidAdapterClosedType);
                        },
                    () =>
                        {
                            throw new InvalidOperationException(
                                string.Format("Could not find a registered monoid adapter for {0}", key));
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
