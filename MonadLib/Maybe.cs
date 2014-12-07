using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Flinq;
using MonadLib.Registries;

namespace MonadLib
{
    public sealed class Maybe<TA> : IMonadPlus<TA>
    {
        private Maybe(TA a, bool isNothing)
        {
            _a = a;
            _isNothing = isNothing;
        }

        internal Maybe(TA a)
            : this(a, false)
        {
        }

        internal Maybe()
            : this(default(TA), true)
        {
        }

        public bool IsNothing
        {
            get { return _isNothing; }
        }

        public bool IsJust
        {
            get { return !IsNothing; }
        }

        public TA FromJust
        {
            get
            {
                if (IsNothing) throw new InvalidOperationException("FromJust called on Maybe containing Nothing.");
                return _a;
            }
        }

        public TA FromMaybe(TA defaultValue)
        {
            return IsJust ? _a : defaultValue;
        }

        public IEnumerable<TA> ToEnumerable()
        {
            return Match(MonadHelpers.One, () => MonadHelpers.Nil<TA>());
        }

        public void Match(Action<TA> justAction, Action nothingAction)
        {
            if (IsJust)
                justAction(FromJust);
            else
                nothingAction();
        }

        public T Match<T>(Func<TA, T> justFunc, Func<T> nothingFunc)
        {
            return IsJust ? justFunc(FromJust) : nothingFunc();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            var other = obj as Maybe<TA>;
            if (other == null) return false;

            if (IsNothing == other.IsNothing)
            {
                return IsNothing || EqualityComparer<TA>.Default.Equals(FromJust, other.FromJust);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Match(a => a.GetHashCode(), () => 0);
        }

        private readonly TA _a;
        private readonly bool _isNothing;

        public MonadAdapter GetMonadAdapter()
        {
            return MonadAdapterRegistry.Get(typeof(Maybe<>));
        }

        public MonadPlusAdapter<TA> GetMonadPlusAdapter()
        {
            return MonadPlusAdapterRegistry.Get<TA>(typeof(Maybe<>));
        }
    }

    public static partial class Maybe
    {
        public static Maybe<TA> Nothing<TA>()
        {
            return new Maybe<TA>();
        }

        public static Maybe<TA> Just<TA>(TA a)
        {
            return new Maybe<TA>(a);
        }

        public static Maybe<TA> ListToMaybe<TA>(IEnumerable<TA> @as)
        {
            using (var enumerator = @as.GetEnumerator())
            {
                return enumerator.MoveNext() ? Just(enumerator.Current) : Nothing<TA>();
            }
        }

        public static IEnumerable<TB> MapMaybe<TA, TB>(Func<TA, Maybe<TB>> f, IEnumerable<TA> @as)
        {
            return @as.Map(f).Where(m => m.IsJust).Select(m => m.FromJust);
        }

        public static IEnumerable<TA> CatMaybes<TA>(IEnumerable<Maybe<TA>> ms)
        {
            return ms.Where(m => m.IsJust).Select(m => m.FromJust);
        }

        public static TB MapOrDefault<TA, TB>(TB b, Func<TA, TB> f, Maybe<TA> ma)
        {
            return ma.MapOrDefault(b, f);
        }

        public static TB MapOrDefault<TA, TB>(this Maybe<TA> ma, TB b, Func<TA, TB> f)
        {
            return ma.Match(f, () => b);
        }

        public static Maybe<TValue> GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? Just(value) : Nothing<TValue>();
        }

        public static Maybe<string> GetValue(this NameValueCollection nameValueCollection, string name)
        {
            var value = nameValueCollection[name];
            return value != null ? Just(value) : Nothing<string>();
        }

        public static Maybe<TA> ToMaybe<TA>(this TA? nullable) where TA : struct
        {
            return nullable.HasValue ? Just(nullable.Value) : Nothing<TA>();
        }

        public static TA? ToNullable<TA>(this Maybe<TA> ma) where TA : struct
        {
            return ma.Match(a => new TA?(a), () => null);
        }

        public static Maybe<TA> Return<TA>(TA a)
        {
            return Just(a);
        }
    }

    internal class MaybeMonadAdapter : MonadAdapter
    {
        public override IMonad<TA> Return<TA>(TA a)
        {
            return Maybe.Just(a);
        }

        public override IMonad<TB> Bind<TA, TB>(IMonad<TA> ma, Func<TA, IMonad<TB>> f)
        {
            var maybeA = (Maybe<TA>)ma;
            return maybeA.IsJust ? f(maybeA.FromJust) : Maybe.Nothing<TB>();
        }
    }

    internal class MaybeMonadPlusAdapter<TAOuter> : MonadPlusAdapter<TAOuter>
    {
        public override IMonad<TAInner> Return<TAInner>(TAInner a)
        {
            return Maybe.Just(a);
        }

        public override IMonad<TBInner> Bind<TAInner, TBInner>(IMonad<TAInner> ma, Func<TAInner, IMonad<TBInner>> f)
        {
            var maybeA = (Maybe<TAInner>)ma;
            return maybeA.IsJust ? f(maybeA.FromJust) : Maybe.Nothing<TBInner>();
        }

        public override IMonadPlus<TAOuter> MZero
        {
            get
            {
                return Maybe.Nothing<TAOuter>();
            }
        }

        public override IMonadPlus<TAOuter> MPlus(IMonadPlus<TAOuter> xs, IMonadPlus<TAOuter> ys)
        {
            return ((Maybe<TAOuter>) xs).Match(_ => xs, () => ys);
        }
    }
}
