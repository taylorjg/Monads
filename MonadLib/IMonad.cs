using System;
using System.Collections.Generic;
using System.Linq;
using Flinq;

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
        public static IMonad<TB> LiftM<TA, TB>(Func<TA, TB> f, IMonad<TA> ma)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(ma, a =>
            {
                var b = f(a);
                var mb = monadAdapter.Unit(b);
                return mb;
            });
        }

        public static IMonad<TC> LiftM2<TA, TB, TC>(Func<TA, TB, TC> f, IMonad<TA> ma, IMonad<TB> mb)
        {
            var monadAdapter = ma.GetMonadAdapter();

            return monadAdapter.Bind(ma, a => monadAdapter.Bind(mb, b =>
                {
                    var c = f(a, b);
                    var mc = monadAdapter.Unit(c);
                    return mc;
                }));
        }

        public static IMonad<TD> LiftM3<TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, IMonad<TA> ma, IMonad<TB> mb, IMonad<TC> mc)
        {
            var monadAdapter = ma.GetMonadAdapter();

            return monadAdapter.Bind(ma, a => monadAdapter.Bind(mb, b => monadAdapter.Bind(mc, c =>
                {
                    var d = f(a, b, c);
                    var md = monadAdapter.Unit(d);
                    return md;
                })));
        }

        // TODO: add further combinators:
        // http://en.wikibooks.org/wiki/Haskell/YAHT/Monads#Monadic_Combinators
        // https://hackage.haskell.org/package/base-4.7.0.1/docs/Control-Monad.html
        // sequence
        // filterM
        // foldM
        // mapM
        // forM
        // when
        // join

        // ReSharper disable PossibleMultipleEnumeration
        public static IMonad<IEnumerable<TA>> Sequence<TA>(IEnumerable<IMonad<TA>> ms)
        {
            // DESIGN PROBLEM: what if ms contains no items ?
            var monadAdapter = ms.ElementAt(0).GetMonadAdapter();
            var z = monadAdapter.Unit(System.Linq.Enumerable.Empty<TA>());
            return ms.FoldRight(z, (m, mtick) => monadAdapter.Bind(m, x => monadAdapter.Bind(mtick, xs => monadAdapter.Unit(System.Linq.Enumerable.Repeat(x, 1).Concat(xs)))));
        }
        // ReSharper restore PossibleMultipleEnumeration
    }

    internal static class MonadCombinators<T1>
    {
        public static IMonad<T1, TB> LiftM<TA, TB>(Func<TA, TB> f, IMonad<T1, TA> ma)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(ma, a =>
            {
                var b = f(a);
                var mb = monadAdapter.Unit(b);
                return mb;
            });
        }

        public static IMonad<T1, TC> LiftM2<TA, TB, TC>(Func<TA, TB, TC> f, IMonad<T1, TA> ma, IMonad<T1, TB> mb)
        {
            var monadAdapter = ma.GetMonadAdapter();

            return monadAdapter.Bind(ma, a => monadAdapter.Bind(mb, b =>
                {
                    var c = f(a, b);
                    var mc = monadAdapter.Unit(c);
                    return mc;
                }));
        }

        public static IMonad<T1, TD> LiftM3<TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, IMonad<T1, TA> ma, IMonad<T1, TB> mb, IMonad<T1, TC> mc)
        {
            var monadAdapter = ma.GetMonadAdapter();

            return monadAdapter.Bind(ma, a => monadAdapter.Bind(mb, b => monadAdapter.Bind(mc, c =>
                {
                    var d = f(a, b, c);
                    var md = monadAdapter.Unit(d);
                    return md;
                })));
        }

        // ReSharper disable PossibleMultipleEnumeration
        public static IMonad<T1, IEnumerable<TA>> Sequence<TA>(IEnumerable<IMonad<T1, TA>> ms)
        {
            // DESIGN PROBLEM: what if ms contains no items ?
            var monadAdapter = ms.ElementAt(0).GetMonadAdapter();
            var z = monadAdapter.Unit(System.Linq.Enumerable.Empty<TA>());
            return ms.FoldRight(z, (m, mtick) => monadAdapter.Bind(m, x => monadAdapter.Bind(mtick, xs => monadAdapter.Unit(System.Linq.Enumerable.Repeat(x, 1).Concat(xs)))));
        }
        // ReSharper restore PossibleMultipleEnumeration
    }
}
