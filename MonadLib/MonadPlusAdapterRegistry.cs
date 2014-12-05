using System;
using System.Collections.Generic;

namespace MonadLib
{
    public static class MonadPlusAdapterRegistry
    {
        private class MonadPlusAdapterCache
        {
            private readonly Type _monadPlusAdapterType;
            private readonly IDictionary<Type[], object> _dictionary = new Dictionary<Type[], object>();

            public MonadPlusAdapterCache(Type monadPlusAdapterType)
            {
                _monadPlusAdapterType = MonadHelpers.GetTypeOrGenericTypeDefinition(monadPlusAdapterType);
            }

            public MonadPlusAdapter<TA> Get<TA>()
            {
                return (MonadPlusAdapter<TA>) GetInternal(typeof (TA));
            }

            private object GetInternal(params Type[] types)
            {
                return _dictionary.GetValue(types).Match(
                    obj => obj,
                    () =>
                        {
                            object monadPlusAdapter;
                            if (_monadPlusAdapterType.IsGenericType)
                            {
                                var monadPlusAdapterClosedType = _monadPlusAdapterType.MakeGenericType(types);
                                monadPlusAdapter = Activator.CreateInstance(monadPlusAdapterClosedType);
                            }
                            else
                            {
                                monadPlusAdapter = Activator.CreateInstance(_monadPlusAdapterType);
                            }
                            _dictionary[types] = monadPlusAdapter;
                            return monadPlusAdapter;
                        });
            }
        }

        private static readonly object LockObject = new object();
        private static readonly IDictionary<Type, MonadPlusAdapterCache> Dictionary = new Dictionary<Type, MonadPlusAdapterCache>();

        static MonadPlusAdapterRegistry()
        {
            Register(typeof(Maybe<>), typeof(MaybeMonadPlusAdapter<>));
        }

        public static void Register(Type monadPlusType, Type monadPlusAdapterType)
        {
            var key = MonadHelpers.GetTypeOrGenericTypeDefinition(monadPlusType);

            lock (LockObject)
            {
                Dictionary[key] = new MonadPlusAdapterCache(monadPlusAdapterType);
            }
        }

        public static MonadPlusAdapter<TA> Get<TA>(Type monadPlusType)
        {
            return GetInternal(monadPlusType, monadPlusAdapterCache => monadPlusAdapterCache.Get<TA>());
        }

        private static T GetInternal<T>(Type monadType, Func<MonadPlusAdapterCache, T> cacheLookupFunc)
        {
            var key = MonadHelpers.GetTypeOrGenericTypeDefinition(monadType);

            lock (LockObject)
            {
                return Dictionary.GetValue(key).Match(cacheLookupFunc, () =>
                    {
                        throw new InvalidOperationException(
                            string.Format("Could not find a registered monad adapter for {0}", key));
                    });
            }
        }
    }
}
