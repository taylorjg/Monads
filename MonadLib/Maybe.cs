using System;
using System.Collections.Generic;
using System.Linq;
using Flinq;

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

        public IEnumerable<TA> ToList()
        {
            return Match(
                a => System.Linq.Enumerable.Repeat(a, 1),
                System.Linq.Enumerable.Empty<TA>);
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

        private readonly TA _a;
        private readonly bool _isNothing;

        private MonadPlusAdapter<TA> _monadPlusAdapter;

        public MonadAdapter GetMonadAdapter()
        {
            return GetMonadPlusAdapter();
        }

        public MonadPlusAdapter<TA> GetMonadPlusAdapter()
        {
            return _monadPlusAdapter ?? (_monadPlusAdapter = new MaybeMonadPlusAdapter<TA>());
        }
    }

    public static class Maybe
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
            return ma.Match(f, () => b);
        }

        public static Maybe<TValue> GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? Just(value) : Nothing<TValue>();
        }

        public static Maybe<TA> Return<TA>(TA a)
        {
            return Just(a);
        }

        public static Maybe<TB> Bind<TA, TB>(this Maybe<TA> ma, Func<TA, Maybe<TB>> f)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (Maybe<TB>)monadAdapter.Bind(ma, f);
        }

        public static Maybe<TB> LiftM<TA, TB>(this Maybe<TA> ma, Func<TA, TB> f)
        {
            return (Maybe<TB>)MonadCombinators.LiftM(f, ma);
        }

        public static Maybe<TB> LiftM<TA, TB>(Func<TA, TB> f, Maybe<TA> ma)
        {
            return (Maybe<TB>)MonadCombinators.LiftM(f, ma);
        }

        public static Maybe<TC> LiftM2<TA, TB, TC>(this Maybe<TA> ma, Maybe<TB> mb, Func<TA, TB, TC> f)
        {
            return (Maybe<TC>)MonadCombinators.LiftM2(f, ma, mb);
        }

        public static Maybe<TC> LiftM2<TA, TB, TC>(Func<TA, TB, TC> f, Maybe<TA> ma, Maybe<TB> mb)
        {
            return (Maybe<TC>)MonadCombinators.LiftM2(f, ma, mb);
        }

        public static Maybe<TD> LiftM3<TA, TB, TC, TD>(this Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Func<TA, TB, TC, TD> f)
        {
            return (Maybe<TD>)MonadCombinators.LiftM3(f, ma, mb, mc);
        }

        public static Maybe<TD> LiftM3<TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc)
        {
            return (Maybe<TD>)MonadCombinators.LiftM3(f, ma, mb, mc);
        }

        public static Maybe<TE> LiftM4<TA, TB, TC, TD, TE>(this Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Maybe<TD> md, Func<TA, TB, TC, TD, TE> f)
        {
            return (Maybe<TE>)MonadCombinators.LiftM4(f, ma, mb, mc, md);
        }

        public static Maybe<TE> LiftM4<TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Maybe<TD> md)
        {
            return (Maybe<TE>)MonadCombinators.LiftM4(f, ma, mb, mc, md);
        }

        public static Maybe<TF> LiftM5<TA, TB, TC, TD, TE, TF>(this Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Maybe<TD> md, Maybe<TE> me, Func<TA, TB, TC, TD, TE, TF> f)
        {
            return (Maybe<TF>)MonadCombinators.LiftM5(f, ma, mb, mc, md, me);
        }

        public static Maybe<TF> LiftM5<TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Maybe<TD> md, Maybe<TE> me)
        {
            return (Maybe<TF>)MonadCombinators.LiftM5(f, ma, mb, mc, md, me);
        }

        public static Maybe<IEnumerable<TA>> Sequence<TA>(IEnumerable<Maybe<TA>> ms)
        {
            return (Maybe<IEnumerable<TA>>)MonadCombinators.SequenceInternal(ms, new MaybeMonadPlusAdapter<TA>());
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> Sequence_<TA>(IEnumerable<Maybe<TA>> ms)
        // ReSharper restore InconsistentNaming
        {
            return (Maybe<Unit>)MonadCombinators.SequenceInternal_(ms, new MaybeMonadPlusAdapter<TA>());
        }

        public static Maybe<IEnumerable<TB>> MapM<TA, TB>(Func<TA, Maybe<TB>> f, IEnumerable<TA> @as)
        {
            return (Maybe<IEnumerable<TB>>)MonadCombinators.MapMInternal(f, @as, new MaybeMonadPlusAdapter<TA>());
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> MapM_<TA, TB>(Func<TA, Maybe<TB>> f, IEnumerable<TA> @as)
        // ReSharper restore InconsistentNaming
        {
            return (Maybe<Unit>)MonadCombinators.MapMInternal_(f, @as, new MaybeMonadPlusAdapter<TA>());
        }

        public static Maybe<IEnumerable<TB>> ForM<TA, TB>(IEnumerable<TA> @as, Func<TA, Maybe<TB>> f)
        {
            return (Maybe<IEnumerable<TB>>)MonadCombinators.MapMInternal(f, @as, new MaybeMonadPlusAdapter<TA>());
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> ForM_<TA, TB>(IEnumerable<TA> @as, Func<TA, Maybe<TB>> f)
        // ReSharper restore InconsistentNaming
        {
            return (Maybe<Unit>)MonadCombinators.MapMInternal_(f, @as, new MaybeMonadPlusAdapter<TA>());
        }

        public static Maybe<IEnumerable<TA>> ReplicateM<TA>(int n, Maybe<TA> ma)
        {
            return (Maybe<IEnumerable<TA>>)MonadCombinators.ReplicateM(n, ma);
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> ReplicateM_<TA>(int n, Maybe<TA> ma)
        // ReSharper restore InconsistentNaming
        {
            return (Maybe<Unit>)MonadCombinators.ReplicateM_(n, ma);
        }

        public static Maybe<TA> Join<TA>(Maybe<Maybe<TA>> mma)
        {
            // Ideally, we would like to use MonadCombinators.Join(mma) but there
            // is a casting issue that I have figured out how to fix.
            var monadAdapter = mma.GetMonadAdapter();
            return (Maybe<TA>)monadAdapter.Bind(mma, MonadHelpers.Identity);
        }

        public static Maybe<TA> MFilter<TA>(this Maybe<TA> ma, Func<TA, bool> p)
        {
            return (Maybe<TA>)MonadPlusCombinators.MFilter(p, ma);
        }

        public static Maybe<TA> MFilter<TA>(Func<TA, bool> p, Maybe<TA> ma)
        {
            return (Maybe<TA>)MonadPlusCombinators.MFilter(p, ma);
        }

        public static Maybe<TA> FoldM<TA, TB>(Func<TA, TB, Maybe<TA>> f, TA a, IEnumerable<TB> bs)
        {
            return (Maybe<TA>)MonadCombinators.FoldMInternal(f, a, bs, new MaybeMonadPlusAdapter<TA>());
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> FoldM_<TA, TB>(Func<TA, TB, Maybe<TA>> f, TA a, IEnumerable<TB> bs)
        // ReSharper restore InconsistentNaming
        {
            return (Maybe<Unit>)MonadCombinators.FoldMInternal_(f, a, bs, new MaybeMonadPlusAdapter<TA>());
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
