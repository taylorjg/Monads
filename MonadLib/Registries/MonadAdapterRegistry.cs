using System;
using System.Collections.Generic;

namespace MonadLib.Registries
{
    public static class MonadAdapterRegistry
    {
        private class MonadAdapterCache
        {
            private readonly Type _monadAdapterType;
            private readonly IDictionary<Type[], object> _dictionary = new Dictionary<Type[], object>();

            public MonadAdapterCache(Type monadAdapterType)
            {
                _monadAdapterType = MonadHelpers.GetTypeOrGenericTypeDefinition(monadAdapterType);
            }

            public MonadAdapter Get()
            {
                return (MonadAdapter) GetInternal();
            }

            public MonadAdapter<T1> Get<T1>()
            {
                return (MonadAdapter<T1>) GetInternal(typeof (T1));
            }

            public MonadAdapter<T1, T2> Get<T1, T2>()
            {
                return (MonadAdapter<T1, T2>) GetInternal(typeof (T1), typeof (T2));
            }

            public MonadAdapter<T1, T2, T3> Get<T1, T2, T3>()
            {
                return (MonadAdapter<T1, T2, T3>) GetInternal(typeof (T1), typeof (T2), typeof (T3));
            }

            private object GetInternal(params Type[] types)
            {
                return _dictionary.GetValue(types).Match(
                    obj => obj,
                    () =>
                        {
                        object monadAdapter;
                        if (types.Length > 0)
                        {
                            var monadAdapterClosedType = _monadAdapterType.MakeGenericType(types);
                            monadAdapter = Activator.CreateInstance(monadAdapterClosedType);
                        }
                        else
                        {
                            monadAdapter = Activator.CreateInstance(_monadAdapterType);
                        }
                        _dictionary[types] = monadAdapter;
                        return monadAdapter;
                    });
            }
        }

        private static readonly object LockObject = new object();
        private static readonly IDictionary<Type, MonadAdapterCache> Dictionary = new Dictionary<Type, MonadAdapterCache>();

        static MonadAdapterRegistry()
        {
            Register(typeof(Maybe<>), typeof(MaybeMonadAdapter));
            Register(typeof(Either<>), typeof(EitherMonadAdapter<>));
            Register(typeof(State<>), typeof(StateMonadAdapter<>));
            Register(typeof(Reader<>), typeof(ReaderMonadAdapter<>));
            Register(typeof(Writer<,,>), typeof(WriterMonadAdapter<,>));
        }

        public static void Register(Type monadType, Type monadAdapterType)
        {
            var key = MonadHelpers.GetTypeOrGenericTypeDefinition(monadType);

            lock (LockObject)
            {
                Dictionary[key] = new MonadAdapterCache(monadAdapterType);
            }
        }

        public static MonadAdapter Get(Type monadType)
        {
            return GetInternal(monadType, monadAdapterCache => monadAdapterCache.Get());
        }

        public static MonadAdapter<T1> Get<T1>(Type monadType)
        {
            return GetInternal(monadType, monadAdapterCache => monadAdapterCache.Get<T1>());
        }

        public static MonadAdapter<T1, T2> Get<T1, T2>(Type monadType)
        {
            return GetInternal(monadType, monadAdapterCache => monadAdapterCache.Get<T1, T2>());
        }

        public static MonadAdapter<T1, T2, T3> Get<T1, T2, T3>(Type monadType)
        {
            return GetInternal(monadType, monadAdapterCache => monadAdapterCache.Get<T1, T2, T3>());
        }

        private static T GetInternal<T>(Type monadType, Func<MonadAdapterCache, T> cacheLookupFunc)
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
