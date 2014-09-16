using System;

namespace MonadLib
{
    // ReSharper disable UnusedTypeParameter
    public interface IMonad<TA>
    {
        IMonadAdapter GetMonadAdapter();
    }
    // ReSharper restore UnusedTypeParameter

    // ReSharper disable UnusedTypeParameter
    public interface IMonad<T1, TA>
    {
        IMonadAdapter<T1> GetMonadAdapter();
    }
    // ReSharper restore UnusedTypeParameter

    public interface IMonadAdapter
    {
        IMonad<TA> Unit<TA>(TA a);
        IMonad<TB> Bind<TA, TB>(IMonad<TA> ma, Func<TA, IMonad<TB>> f);
    }

    // ReSharper disable UnusedTypeParameter
    public interface IMonadAdapter<T1>
    {
        IMonad<T1, TA> Unit<TA>(TA a);
        IMonad<T1, TB> Bind<TA, TB>(IMonad<T1, TA> ma, Func<TA, IMonad<T1, TB>> f);
    }
    // ReSharper restore UnusedTypeParameter

    internal static class Monad
    {
        public static IMonad<TA> Unit<TA>(IMonadAdapter monadAdapter, TA a)
        {
            return monadAdapter.Unit(a);
        }

        public static IMonad<TB> Bind<TA, TB>(IMonadAdapter monadAdapter, IMonad<TA> ma, Func<TA, IMonad<TB>> f)
        {
            return monadAdapter.Bind(ma, f);
        }
    }

    internal static class Monad<T1>
    {
        public static IMonad<T1, TA> Unit<TA>(IMonadAdapter<T1> monadAdapter, TA a)
        {
            return monadAdapter.Unit(a);
        }

        public static IMonad<T1, TB> Bind<TA, TB>(IMonadAdapter<T1> monadAdapter, IMonad<T1, TA> ma, Func<TA, IMonad<T1, TB>> f)
        {
            return monadAdapter.Bind(ma, f);
        }
    }

    internal static class MonadCombinators
    {
        public static IMonad<TB> LiftM<TA, TB>(IMonad<TA> ma, Func<TA, TB> f)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(ma, a =>
            {
                var b = f(a);
                var mb = monadAdapter.Unit(b);
                return mb;
            });
        }
    }

    internal static class MonadCombinators<T1>
    {
        public static IMonad<T1, TB> LiftM<TA, TB>(IMonad<T1, TA> ma, Func<TA, TB> f)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(ma, a =>
            {
                var b = f(a);
                var mb = monadAdapter.Unit(b);
                return mb;
            });
        }
    }
}
