using System;

namespace Monads
{
    // ReSharper disable UnusedTypeParameter
    public interface IMonad<out TA>
    {
        IMonadAdapter MonadAdapter { get; }
    }
    // ReSharper restore UnusedTypeParameter

    public interface IMonadAdapter
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

        public static IMonad<TB> Bind<TA, TB>(IMonadAdapter monadAdapter, IMonad<TA> ma, Func<TA, IMonad<TB>> f)
        {
            return monadAdapter.Bind(ma, f);
        }
    }

    public static class MonadExtensions
    {
        public static IMonad<TB> LiftM<TA, TB>(this IMonad<TA> ma, Func<TA, TB> f)
        {
            var monadAdapter = ma.MonadAdapter;
            return monadAdapter.Bind(ma, a =>
                {
                    var b = f(a);
                    var mb = monadAdapter.Unit(b);
                    return mb;
                });
        }
    }
}
