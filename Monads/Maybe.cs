using System;

namespace Monads
{
    public sealed class Maybe<TA> : IMonad<TA>
    {
        public Maybe()
        {
            _a = default(TA);
            _isNothing = true;
        }

        public Maybe(TA a)
        {
            _a = a;
            _isNothing = false;
        }

        public bool IsNothing
        {
            get { return _isNothing; }
        }

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
            var monadAdapter = MonadAdapter;
            var mb = monadAdapter.Bind(this, f);
            return (Maybe<TB>)mb;
        }

        private IMonadAdapter _monadAdapter;

        public IMonadAdapter MonadAdapter
        {
            get { return _monadAdapter ?? (_monadAdapter = new MaybeMonadAdapter()); }
        }

        private readonly TA _a;
        private readonly bool _isNothing;
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
            return new Maybe<T>();
        }

        public static Maybe<T> Just<T>(T a)
        {
            return new Maybe<T>(a);
        }

        public static Maybe<TA> Unit<TA>(TA a)
        {
            return Just(a);
        }
    }
}
