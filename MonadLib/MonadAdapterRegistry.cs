using System;
using System.Collections.Generic;

namespace MonadLib
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

        private class MonadAdapterWithSubtypesCache
        {
            private readonly Type _monadAdapterType;
            private readonly IDictionary<Type[], object> _dictionary = new Dictionary<Type[], object>();

            public MonadAdapterWithSubtypesCache(Type monadAdapterType)
            {
                _monadAdapterType = MonadHelpers.GetTypeOrGenericTypeDefinition(monadAdapterType);
            }

            public MonadAdapter<TSubtype1, TSubtype2> Get<TSubtype1, TSubtype2>()
            {
                var key = new[]
                    {
                        typeof (TSubtype1),
                        typeof (TSubtype2)
                    };

                return (MonadAdapter<TSubtype1, TSubtype2>) _dictionary.GetValue(key).Match(
                    obj => obj,
                    () =>
                        {
                            var typeParam1Open = MonadHelpers.GetTypeOrGenericTypeDefinition(typeof (TSubtype1));
                            var typeParam1Closed = typeParam1Open.MakeGenericType(typeof (TSubtype2));
                            var monadAdapterClosedType = _monadAdapterType.MakeGenericType(typeParam1Closed, typeof(TSubtype2));
                            var monadAdapter = Activator.CreateInstance(monadAdapterClosedType);
                            _dictionary[key] = monadAdapter;
                            return monadAdapter;
                        });
            }
        }

        private static readonly object LockObject = new object();
        private static readonly IDictionary<Type, MonadAdapterCache> Dictionary = new Dictionary<Type, MonadAdapterCache>();
        private static readonly IDictionary<Tuple<Type, Type>, MonadAdapterWithSubtypesCache> DictionarySubtypes = new Dictionary<Tuple<Type, Type>, MonadAdapterWithSubtypesCache>();

        static MonadAdapterRegistry()
        {
            Register(typeof(Either<>), typeof(EitherMonadAdapter<>));
            Register(typeof(State<>), typeof(StateMonadAdapter<>));
            Register(typeof(Reader<>), typeof(ReaderMonadAdapter<>));

            RegisterWithSubtypes(
                typeof(Writer<ListMonoid<Unit>, Unit, Unit>),
                typeof(ListMonoid<Unit>),
                typeof(WriterMonadAdapter<ListMonoid<Unit>, Unit>));
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

        public static void RegisterWithSubtypes(Type monadType, Type subtype1, Type monadAdapterType)
        {
            var key = Tuple.Create(
                MonadHelpers.GetTypeOrGenericTypeDefinition(monadType),
                MonadHelpers.GetTypeOrGenericTypeDefinition(subtype1));

            lock (LockObject)
            {
                DictionarySubtypes[key] = new MonadAdapterWithSubtypesCache(monadAdapterType);
            }
        }

        public static MonadAdapter<TSubtype1, TSubtype2> GetWithSubtypes<TSubtype1, TSubtype2>(Type monadType)
        {
            var key = Tuple.Create(
                MonadHelpers.GetTypeOrGenericTypeDefinition(monadType),
                MonadHelpers.GetTypeOrGenericTypeDefinition(typeof(TSubtype1)));

            lock (LockObject)
            {
                return DictionarySubtypes.GetValue(key).Match(
                    monadAdapterWithSubtypesCache => monadAdapterWithSubtypesCache.Get<TSubtype1, TSubtype2>(),
                    () =>
                    {
                        throw new InvalidOperationException(
                            string.Format("Could not find a registered monad adapter for {0}", key));
                    });
            }
        }
    }
}
