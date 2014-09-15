using System;

namespace Monads
{
    public sealed class Maybe<TA> : IMonad<TA>
    {
        private Maybe()
        {
        }

        internal static Maybe<T> MakeJust<T>(T a)
        {
            return new Maybe<T>
                {
                    _a = a,
                    IsNothing = false
                };
        }

        internal static Maybe<T> MakeNothing<T>()
        {
            return new Maybe<T>
                {
                    _a = default(T),
                    IsNothing = true
                };
        }

        public bool IsNothing { get; private set; }

        public bool IsJust
        {
            get { return !IsNothing; }
        }

        public TA FromJust()
        {
            if (IsNothing) throw new InvalidOperationException("FromJust called on Maybe containing Nothing.");
            return _a;
        }

        public TA FromMaybe(TA defaultValue)
        {
            return IsJust ? _a : defaultValue;
        }

        public Maybe<TB> Bind<TB>(Func<TA, Maybe<TB>> f)
        {
            var monadAdapter = GetMonadAdapter();
            var mb = monadAdapter.Bind(this, f);
            return (Maybe<TB>)mb;
        }

        public Maybe<TB> LiftM<TB>(Func<TA, TB> f)
        {
            return (Maybe<TB>)MonadExtensions.LiftM(this, f);
        }

        private IMonadAdapter _monadAdapter;

        public IMonadAdapter GetMonadAdapter()
        {
            return _monadAdapter ?? (_monadAdapter = new MaybeMonadAdapter());
        }

        public IMonadAdapter<T1> GetMonadAdapter<T1>()
        {
            return null;
        }

        private TA _a;
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
            return maybeA.IsJust ? f(maybeA.FromJust()) : Maybe.Nothing<TB>();
        }
    }

    public static class Maybe
    {
        public static Maybe<T> Nothing<T>()
        {
            return Maybe<T>.MakeNothing<T>();
        }

        public static Maybe<T> Just<T>(T a)
        {
            return Maybe<T>.MakeJust(a);
        }

        public static Maybe<TA> Unit<TA>(TA a)
        {
            return Just(a);
        }
    }
}
