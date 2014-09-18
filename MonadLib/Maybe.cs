using System;
using System.Collections.Generic;

namespace MonadLib
{
    public sealed class Maybe<TA> : IMonad<TA>
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

        private IMonadAdapter _monadAdapter;

        public IMonadAdapter GetMonadAdapter()
        {
            return _monadAdapter ?? (_monadAdapter = new MaybeMonadAdapter());
        }

        public void Match()
        {
            throw new NotImplementedException();
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

        public static Maybe<TA> Unit<TA>(TA a)
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

        public static Maybe<TC> LiftM2<TA, TB, TC>(this Maybe<TA> ma, Maybe<TB> mb, Func<TA, TB, TC> f)
        {
            return (Maybe<TC>)MonadCombinators.LiftM2(f, ma, mb);
        }

        public static Maybe<TD> LiftM3<TA, TB, TC, TD>(this Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Func<TA, TB, TC, TD> f)
        {
            return (Maybe<TD>)MonadCombinators.LiftM3(f, ma, mb, mc);
        }

        public static Maybe<TB> LiftM<TA, TB>(Func<TA, TB> f, Maybe<TA> ma)
        {
            return (Maybe<TB>)MonadCombinators.LiftM(f, ma);
        }

        public static Maybe<TC> LiftM2<TA, TB, TC>(Func<TA, TB, TC> f, Maybe<TA> ma, Maybe<TB> mb)
        {
            return (Maybe<TC>)MonadCombinators.LiftM2(f, ma, mb);
        }

        public static Maybe<TD> LiftM3<TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc)
        {
            return (Maybe<TD>)MonadCombinators.LiftM3(f, ma, mb, mc);
        }

        public static Maybe<IEnumerable<TA>> Sequence<TA>(IEnumerable<Maybe<TA>> ms)
        {
            return (Maybe<IEnumerable<TA>>)MonadCombinators.Sequence(ms);
        }
    }

    internal class MaybeMonadAdapter : IMonadAdapter
    {
        public IMonad<TA> Unit<TA>(TA a)
        {
            return Maybe.Just(a);
        }

        public IMonad<TB> Bind<TA, TB>(IMonad<TA> ma, Func<TA, IMonad<TB>> f)
        {
            var maybeA = (Maybe<TA>)ma;
            return maybeA.IsJust ? f(maybeA.FromJust) : Maybe.Nothing<TB>();
        }
    }
}
