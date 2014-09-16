using System;
using MonadLib;

// ReSharper disable CheckNamespace
namespace Monads
// ReSharper restore CheckNamespace
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

        public TA FromJust()
        {
            if (IsNothing) throw new InvalidOperationException("FromJust called on Maybe containing Nothing.");
            return _a;
        }

        public TA FromMaybe(TA defaultValue)
        {
            return IsJust ? _a : defaultValue;
        }

        private readonly TA _a;
        private readonly bool _isNothing;

        private IMonadAdapter _monadAdapter;

        public IMonadAdapter GetMonadAdapter()
        {
            return _monadAdapter ?? (_monadAdapter = new MaybeMonadAdapter());
        }

        public Maybe<TB> Bind<TB>(Func<TA, Maybe<TB>> f)
        {
            var monadAdapter = GetMonadAdapter();
            var mb = monadAdapter.Bind(this, f);
            return (Maybe<TB>)mb;
        }

        public Maybe<TB> LiftM<TB>(Func<TA, TB> f)
        {
            return (Maybe<TB>)MonadCombinators.LiftM(this, f);
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
}
