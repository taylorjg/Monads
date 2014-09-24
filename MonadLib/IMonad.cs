using System;
using System.Collections.Generic;
using System.Linq;
using Flinq;

namespace MonadLib
{
    public sealed class Unit
    {
        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    // ReSharper disable UnusedTypeParameter
    public interface IMonad<TA>
    {
        MonadAdapter GetMonadAdapter();
    }
    // ReSharper restore UnusedTypeParameter

    // ReSharper disable UnusedTypeParameter
    public interface IMonad<T1, TA>
    {
        MonadAdapter<T1> GetMonadAdapter();
    }
    // ReSharper restore UnusedTypeParameter

    public interface IMonadPlus<TA> : IMonad<TA>
    {
        MonadPlusAdapter<TA> GetMonadPlusAdapter();
    }

    public abstract class MonadAdapter
    {
        public abstract IMonad<TA> Return<TA>(TA a);
        public abstract IMonad<TB> Bind<TA, TB>(IMonad<TA> ma, Func<TA, IMonad<TB>> f);

        public virtual IMonad<TB> BindIgnoringLeft<TA, TB>(IMonad<TA> ma, IMonad<TB> mb)
        {
            return Bind(ma, _ => mb);
        }
    }

    public abstract class MonadPlusAdapter<TA> : MonadAdapter
    {
        public abstract IMonadPlus<TA> MZero { get; }
        public abstract IMonadPlus<TA> MPlus(IMonadPlus<TA> xs, IMonadPlus<TA> ys);
    }

    public abstract class MonadAdapter<T1>
    {
        public abstract IMonad<T1, TA> Return<TA>(TA a);
        public abstract IMonad<T1, TB> Bind<TA, TB>(IMonad<T1, TA> ma, Func<TA, IMonad<T1, TB>> f);

        public virtual IMonad<T1, TB> BindIgnoringLeft<TA, TB>(IMonad<T1, TA> ma, IMonad<T1, TB> mb)
        {
            return Bind(ma, _ => mb);
        }
    }

    internal static class MonadCombinators
    {
        public static IMonad<TB> LiftM<TA, TB>(Func<TA, TB> f, IMonad<TA> ma)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Return(f(a)));
        }

        public static IMonad<TC> LiftM2<TA, TB, TC>(Func<TA, TB, TC> f, IMonad<TA> ma, IMonad<TB> mb)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Bind(
                    mb, b => monadAdapter.Return(f(a, b))));
        }

        public static IMonad<TD> LiftM3<TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, IMonad<TA> ma, IMonad<TB> mb, IMonad<TC> mc)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Bind(
                    mb, b => monadAdapter.Bind(
                        mc, c => monadAdapter.Return(f(a, b, c)))));
        }

        public static IMonad<TE> LiftM4<TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, IMonad<TA> ma, IMonad<TB> mb, IMonad<TC> mc, IMonad<TD> md)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Bind(
                    mb, b => monadAdapter.Bind(
                        mc, c => monadAdapter.Bind(
                            md, d => monadAdapter.Return(f(a, b, c, d))))));
        }

        public static IMonad<TF> LiftM5<TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, IMonad<TA> ma, IMonad<TB> mb, IMonad<TC> mc, IMonad<TD> md, IMonad<TE> me)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Bind(
                    mb, b => monadAdapter.Bind(
                        mc, c => monadAdapter.Bind(
                            md, d => monadAdapter.Bind(
                                me, e => monadAdapter.Return(f(a, b, c, d, e)))))));
        }

        public static IMonad<IEnumerable<TA>> SequenceInternal<TA>(IEnumerable<IMonad<TA>> ms, MonadAdapter monadAdapter)
        {
            var z = monadAdapter.Return(System.Linq.Enumerable.Empty<TA>());
            return ms.FoldRight(
                z, (m, mtick) => monadAdapter.Bind(
                    m, x => monadAdapter.Bind(
                        mtick, xs => monadAdapter.Return(MonadHelpers.Cons(x, xs)))));
        }

        // ReSharper disable InconsistentNaming
        public static IMonad<Unit> SequenceInternal_<TA>(IEnumerable<IMonad<TA>> ms, MonadAdapter monadAdapter)
        // ReSharper restore InconsistentNaming
        {
            var z = monadAdapter.Return(new Unit());
            return ms.FoldRight(z, monadAdapter.BindIgnoringLeft);
        }

        public static IMonad<IEnumerable<TB>> MapMInternal<TA, TB>(Func<TA, IMonad<TB>> f, IEnumerable<TA> @as, MonadAdapter monadAdapter)
        {
            return SequenceInternal(@as.Map(f), monadAdapter);
        }

        // ReSharper disable InconsistentNaming
        public static IMonad<Unit> MapMInternal_<TA, TB>(Func<TA, IMonad<TB>> f, IEnumerable<TA> @as, MonadAdapter monadAdapter)
        // ReSharper restore InconsistentNaming
        {
            return SequenceInternal_(@as.Map(f), monadAdapter);
        }

        public static IMonad<IEnumerable<TA>> ReplicateM<TA>(int n, IMonad<TA> ma)
        {
            return SequenceInternal(System.Linq.Enumerable.Repeat(ma, n), ma.GetMonadAdapter());
        }

        // ReSharper disable InconsistentNaming
        public static IMonad<Unit> ReplicateM_<TA>(int n, IMonad<TA> ma)
        // ReSharper restore InconsistentNaming
        {
            return SequenceInternal_(System.Linq.Enumerable.Repeat(ma, n), ma.GetMonadAdapter());
        }

        public static IMonad<TA> Join<TA>(IMonad<IMonad<TA>> mma)
        {
            var monadAdapter = mma.GetMonadAdapter();
            return monadAdapter.Bind(mma, MonadHelpers.Identity);
        }
    }

    internal static class MonadPlusCombinators
    {
        public static IMonadPlus<TA> MFilter<TA>(Func<TA, bool> p, IMonadPlus<TA> ma)
        {
            var monadPlusAdapter = ma.GetMonadPlusAdapter();
            return (IMonadPlus<TA>)monadPlusAdapter.Bind(ma, a => p(a) ? monadPlusAdapter.Return(a) : monadPlusAdapter.MZero);
        }
    }

    internal static class MonadCombinators<T1>
    {
        public static IMonad<T1, TB> LiftM<TA, TB>(Func<TA, TB> f, IMonad<T1, TA> ma)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Return(f(a)));
        }

        public static IMonad<T1, TC> LiftM2<TA, TB, TC>(Func<TA, TB, TC> f, IMonad<T1, TA> ma, IMonad<T1, TB> mb)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Bind(
                    mb, b => monadAdapter.Return(f(a, b))));
        }

        public static IMonad<T1, TD> LiftM3<TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, IMonad<T1, TA> ma, IMonad<T1, TB> mb, IMonad<T1, TC> mc)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Bind(
                    mb, b => monadAdapter.Bind(
                        mc, c => monadAdapter.Return(f(a, b, c)))));
        }

        public static IMonad<T1, TE> LiftM4<TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, IMonad<T1, TA> ma, IMonad<T1, TB> mb, IMonad<T1, TC> mc, IMonad<T1, TD> md)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Bind(
                    mb, b => monadAdapter.Bind(
                        mc, c => monadAdapter.Bind(
                            md, d => monadAdapter.Return(f(a, b, c, d))))));
        }

        public static IMonad<T1, TF> LiftM5<TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, IMonad<T1, TA> ma, IMonad<T1, TB> mb, IMonad<T1, TC> mc, IMonad<T1, TD> md, IMonad<T1, TE> me)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Bind(
                    mb, b => monadAdapter.Bind(
                        mc, c => monadAdapter.Bind(
                            md, d => monadAdapter.Bind(
                                me, e => monadAdapter.Return(f(a, b, c, d, e)))))));
        }

        // ReSharper disable InconsistentNaming
        public static IMonad<T1, IEnumerable<TA>> SequenceInternal<TA>(IEnumerable<IMonad<T1, TA>> ms, MonadAdapter<T1> monadAdapter)
        // ReSharper restore InconsistentNaming
        {
            var z = monadAdapter.Return(System.Linq.Enumerable.Empty<TA>());
            return ms.FoldRight(
                z, (m, mtick) => monadAdapter.Bind(
                    m, x => monadAdapter.Bind(
                        mtick, xs => monadAdapter.Return(MonadHelpers.Cons(x, xs)))));
        }

        // ReSharper disable InconsistentNaming
        public static IMonad<T1, Unit> SequenceInternal_<TA>(IEnumerable<IMonad<T1, TA>> ms, MonadAdapter<T1> monadAdapter)
        // ReSharper restore InconsistentNaming
        {
            var z = monadAdapter.Return(new Unit());
            return ms.FoldRight(z, monadAdapter.BindIgnoringLeft);
        }

        public static IMonad<T1, IEnumerable<TB>> MapMInternal<TA, TB>(Func<TA, IMonad<T1, TB>> f, IEnumerable<TA> @as, MonadAdapter<T1> monadAdapter)
        {
            return SequenceInternal(@as.Map(f), monadAdapter);
        }

        // ReSharper disable InconsistentNaming
        public static IMonad<T1, Unit> MapMInternal_<TA, TB>(Func<TA, IMonad<T1, TB>> f, IEnumerable<TA> @as, MonadAdapter<T1> monadAdapter)
        // ReSharper restore InconsistentNaming
        {
            return SequenceInternal_(@as.Map(f), monadAdapter);
        }

        public static IMonad<T1, IEnumerable<TA>> ReplicateM<TA>(int n, IMonad<T1, TA> ma)
        {
            return SequenceInternal(System.Linq.Enumerable.Repeat(ma, n), ma.GetMonadAdapter());
        }

        // ReSharper disable InconsistentNaming
        public static IMonad<T1, Unit> ReplicateM_<TA>(int n, IMonad<T1, TA> ma)
        // ReSharper restore InconsistentNaming
        {
            return SequenceInternal_(System.Linq.Enumerable.Repeat(ma, n), ma.GetMonadAdapter());
        }

        public static IMonad<T1, TA> Join<TA>(IMonad<T1, IMonad<T1, TA>> mma)
        {
            var monadAdapter = mma.GetMonadAdapter();
            return monadAdapter.Bind(mma, MonadHelpers.Identity);
        }
    }

    internal static class MonadHelpers
    {
        public static TA Identity<TA>(TA a)
        {
            return a;
        }

        public static IEnumerable<TA> Cons<TA>(TA x, IEnumerable<TA> xs)
        {
            return System.Linq.Enumerable.Repeat(x, 1).Concat(xs);
        }
    }
}
