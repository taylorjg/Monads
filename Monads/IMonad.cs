using System;

namespace Monads
{
    // ReSharper disable UnusedTypeParameter
    public interface IMonad<out TA>
    {
        IMonadAdapter GetMonadAdapter();
        IMonadAdapter<T1> GetMonadAdapter<T1>();
    }
    // ReSharper restore UnusedTypeParameter

    public interface IMonadAdapter
    {
        IMonad<TA> Unit<TA>(TA a);
        IMonad<TB> Bind<TA, TB>(IMonad<TA> ma, Func<TA, IMonad<TB>> f);
    }

    public interface IMonadAdapter<T1>
    {
        IMonad<TA> Unit<TA>(TA a);
        IMonad<TB> Bind<TA, TB>(IMonad<TA> ma, Func<TA, IMonad<TB>> f);
    }

    public static class Monad
    {
        public static IMonad<TA> Unit<TA>(IMonadAdapter monadAdapter, TA a)
        {
            return monadAdapter.Unit(a);
        }

        public static IMonad<TA> Unit<T1, TA>(IMonadAdapter<T1> monadAdapter, TA a)
        {
            return monadAdapter.Unit(a);
        }

        public static IMonad<TB> Bind<TA, TB>(IMonadAdapter monadAdapter, IMonad<TA> ma, Func<TA, IMonad<TB>> f)
        {
            return monadAdapter.Bind(ma, f);
        }

        public static IMonad<TB> Bind<T1, TA, TB>(IMonadAdapter<T1> monadAdapter, IMonad<TA> ma, Func<TA, IMonad<TB>> f)
        {
            return monadAdapter.Bind(ma, f);
        }
    }

    public static class MonadExtensions
    {
        public static IMonad<TB> LiftM<TA, TB>(this IMonad<TA> ma, Func<TA, TB> f)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(ma, a =>
            {
                var b = f(a);
                var mb = monadAdapter.Unit(b);
                return mb;
            });
        }

        public static IMonad<TB> LiftM<T1, TA, TB>(this IMonad<TA> ma, Func<TA, TB> f)
        {
            var monadAdapter = ma.GetMonadAdapter<T1>();
            return monadAdapter.Bind(ma, a =>
            {
                var b = f(a);
                var mb = monadAdapter.Unit(b);
                return mb;
            });
        }
    }
}
