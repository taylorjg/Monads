using System;

namespace Monads
{
    public sealed class Maybe<TA>
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

        private readonly TA _a;
        private readonly bool _isNothing;

        //public Maybe<TA> Unit(TA a)
        //{
        //    return Maybe.Just(a);
        //}

        //public Maybe<TB> Bind<TB>(Func<TA, Maybe<TB>> f)
        //{
        //    return IsJust ? f(FromJust()) : Maybe.Nothing<TB>();
        //}
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

        public static Maybe<TB> Bind<TA, TB>(this Maybe<TA> ma, Func<TA, Maybe<TB>> f)
        {
            return ma.IsJust ? f(ma.FromJust()) : Nothing<TB>();
        }

        public static Maybe<TB> LiftM<TA, TB>(this Maybe<TA> ma, Func<TA, TB> f)
        {
            return ma.Bind(a => Unit(f(a)));
        }
    }
}
