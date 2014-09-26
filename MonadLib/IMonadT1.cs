using System;
using System.Collections.Generic;
using System.Linq;
using Flinq;

namespace MonadLib
{
    // ReSharper disable UnusedTypeParameter
    public interface IMonad<T1, TA>
    {
        MonadAdapter<T1> GetMonadAdapter();
    }
    // ReSharper restore UnusedTypeParameter

    public abstract class MonadAdapter<T1>
    {
        public abstract IMonad<T1, TA> Return<TA>(TA a);
        public abstract IMonad<T1, TB> Bind<TA, TB>(IMonad<T1, TA> ma, Func<TA, IMonad<T1, TB>> f);

        public virtual IMonad<T1, TB> BindIgnoringLeft<TA, TB>(IMonad<T1, TA> ma, IMonad<T1, TB> mb)
        {
            return Bind(ma, _ => mb);
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
            var z = monadAdapter.Return(MonadHelpers.Nil<TA>());
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

        public static IMonad<T1, TA> FoldMInternal<TA, TB>(Func<TA, TB, IMonad<T1, TA>> f, TA a, IEnumerable<TB> bs, MonadAdapter<T1> monadAdapter)
        {
            // TODO: fix ReSharper grumble: Implicitly captured closure: f
            return bs.HeadAndTail().Match(
                tuple =>
                {
                    var x = tuple.Item1;
                    var xs = tuple.Item2;
                    var m = f(a, x);
                    return monadAdapter.Bind(m, acc => FoldMInternal(f, acc, xs, monadAdapter));
                },
                () => monadAdapter.Return(a));
        }

        // ReSharper disable InconsistentNaming
        public static IMonad<T1, Unit> FoldMInternal_<TA, TB>(Func<TA, TB, IMonad<T1, TA>> f, TA a, IEnumerable<TB> bs, MonadAdapter<T1> monadAdapter)
        // ReSharper restore InconsistentNaming
        {
            var m = FoldMInternal(f, a, bs, monadAdapter);
            var unit = monadAdapter.Return(new Unit());
            return monadAdapter.BindIgnoringLeft(m, unit);
        }

        public static IMonad<T1, IEnumerable<TC>> ZipWithMInternal<TA, TB, TC>(Func<TA, TB, IMonad<T1, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs, MonadAdapter<T1> monadAdapter)
        {
            return SequenceInternal(@as.Zip(bs, f), monadAdapter);
        }

        // ReSharper disable InconsistentNaming
        public static IMonad<T1, Unit> ZipWithMInternal_<TA, TB, TC>(Func<TA, TB, IMonad<T1, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs, MonadAdapter<T1> monadAdapter)
        // ReSharper restore InconsistentNaming
        {
            return SequenceInternal_(@as.Zip(bs, f), monadAdapter);
        }

        public static IMonad<T1, IEnumerable<TA>> FilterMInternal<TA>(Func<TA, IMonad<T1, bool>> p, IEnumerable<TA> @as, MonadAdapter<T1> monadAdapter)
        {
            // TODO: fix ReSharper grumble: Implicitly captured closure: p
            return @as.HeadAndTail().Match(
                tuple =>
                {
                    var x = tuple.Item1;
                    var xs = tuple.Item2;
                    return monadAdapter.Bind(
                        p(x), flg => monadAdapter.Bind(
                            FilterMInternal(p, xs, monadAdapter),
                            ys => monadAdapter.Return(flg ? MonadHelpers.Cons(x, ys) : ys)));
                },
                () => monadAdapter.Return(MonadHelpers.Nil<TA>()));
        }

        public static IMonad<T1, Unit> When(bool b, IMonad<T1, Unit> m)
        {
            var monadAdapter = m.GetMonadAdapter();
            return b ? m : monadAdapter.Return(new Unit());
        }

        public static IMonad<T1, Unit> Unless(bool b, IMonad<T1, Unit> m)
        {
            return When(!b, m);
        }

        // ReSharper disable FunctionRecursiveOnAllPaths
        public static IMonad<T1, TB> Forever<TA, TB>(IMonad<T1, TA> m)
        {
            var monadAdapter = m.GetMonadAdapter();
            return monadAdapter.BindIgnoringLeft(m, Forever<TA, TB>(m));
        }
        // ReSharper restore FunctionRecursiveOnAllPaths

        public static IMonad<T1, Unit> Void<TA>(IMonad<T1, TA> m)
        {
            var monadAdapter = m.GetMonadAdapter();
            return monadAdapter.BindIgnoringLeft(m, monadAdapter.Return(new Unit()));
        }
    }
}
