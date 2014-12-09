using System;
using System.Collections.Generic;
using System.Linq;
using Flinq;
using MonadLib.Registries;

namespace MonadLib
{
    internal static partial class MonadCominatorsQueryExtensions
    {
        public static IMonad<TB> Select<TA, TB>(this IMonad<TA> ma, Func<TA, TB> f)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Return(f(a)));
        }

        public static IMonad<TB> SelectMany<TA, TB>(this IMonad<TA> ma, Func<TA, IMonad<TB>> f) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(ma, f);
        }

        public static IMonad<TC> SelectMany<TA, TB, TC>(this IMonad<TA> ma, Func<TA, IMonad<TB>> f1, Func<TA, TB, TC> f2) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Bind(
                    f1(a), b => monadAdapter.Return(f2(a, b))));
        }
    }

    public static partial class MonadCombinators
    {
        public static TMonad LiftM<TMonad, TA, TB>(Func<TA, TB> f, IMonad<TA> ma)
            where TMonad : IMonad<TB>
        {
            return (TMonad)
                from a in ma
                select f(a);
        }

        public static TMonad LiftM2<TMonad, TA, TB, TC>(Func<TA, TB, TC> f, IMonad<TA> ma, IMonad<TB> mb)
            where TMonad : IMonad<TC>
        {
            return (TMonad)
                from a in ma
                from b in mb
                select f(a, b);
        }

        public static TMonad LiftM3<TMonad, TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, IMonad<TA> ma, IMonad<TB> mb, IMonad<TC> mc)
            where TMonad : IMonad<TD>
        {
            return (TMonad)
                from a in ma
                from b in mb
                from c in mc
                select f(a, b, c);
        }

        public static TMonad LiftM4<TMonad, TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, IMonad<TA> ma, IMonad<TB> mb, IMonad<TC> mc, IMonad<TD> md)
            where TMonad : IMonad<TE>
        {
            return (TMonad)
                from a in ma
                from b in mb
                from c in mc
                from d in md
                select f(a, b, c, d);
        }

        public static TMonad LiftM5<TMonad, TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, IMonad<TA> ma, IMonad<TB> mb, IMonad<TC> mc, IMonad<TD> md, IMonad<TE> me)
            where TMonad : IMonad<TF>
        {
            return (TMonad)
                from a in ma
                from b in mb
                from c in mc
                from d in md
                from e in me
                select f(a, b, c, d, e);
        }

        public static TMonad Join<TMonadMonad, TMonad, TA>(TMonadMonad mma)
            where TMonadMonad : IMonad<TMonad>
            where TMonad : IMonad<TA>
        {
            var monadAdapter = MonadAdapterRegistry.Get(typeof(TMonad));
            return (TMonad)monadAdapter.Bind(mma, ma => ma);
        }

        public static TMonad Sequence<TMonad, TA>(IEnumerable<IMonad<TA>> ms)
            where TMonad : IMonad<IEnumerable<TA>>
        {
            var monadAdapter = MonadAdapterRegistry.Get(typeof(TMonad));
            var z = monadAdapter.Return(MonadHelpers.Nil<TA>());
            return (TMonad)ms.FoldRight(
                z, (m, mtick) => monadAdapter.Bind(
                    m, x => monadAdapter.Bind(
                        mtick, xs => monadAdapter.Return(MonadHelpers.Cons(x, xs)))));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad Sequence_<TMonad, TA>(IEnumerable<IMonad<TA>> ms)
            where TMonad : IMonad<Unit>
        // ReSharper restore InconsistentNaming
        {
            var monadAdapter = MonadAdapterRegistry.Get(typeof(TMonad));
            var z = monadAdapter.Return(new Unit());
            return (TMonad)ms.FoldRight(z, monadAdapter.BindIgnoringLeft);
        }

        public static TMonad MapM<TMonad, TA, TB>(Func<TA, IMonad<TB>> f, IEnumerable<TA> @as)
            where TMonad : IMonad<IEnumerable<TB>>
        {
            return Sequence<TMonad, TB>(@as.Map(f));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad MapM_<TMonad, TA, TB>(Func<TA, IMonad<TB>> f, IEnumerable<TA> @as)
            where TMonad : IMonad<Unit>
        // ReSharper restore InconsistentNaming
        {
            return Sequence_<TMonad, TB>(@as.Map(f));
        }

        public static TMonad ReplicateM<TMonad, TA>(int n, IMonad<TA> ma)
            where TMonad : IMonad<IEnumerable<TA>>
        {
            return Sequence<TMonad, TA>(System.Linq.Enumerable.Repeat(ma, n));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad ReplicateM_<TMonad, TA>(int n, IMonad<TA> ma)
            where TMonad : IMonad<Unit>
        // ReSharper restore InconsistentNaming
        {
            return Sequence_<TMonad, TA>(System.Linq.Enumerable.Repeat(ma, n));
        }

        public static TMonad FoldM<TMonad, TA, TB>(Func<TA, TB, IMonad<TA>> f, TA a, IEnumerable<TB> bs)
            where TMonad : IMonad<TA>
        {
            var monadAdapter = MonadAdapterRegistry.Get(typeof(TMonad));
            return (TMonad)bs.HeadAndTail().Match(
                tuple =>
                {
                    var x = tuple.Item1;
                    var xs = tuple.Item2;
                    var m = f(a, x);
                    return monadAdapter.Bind(m, acc => FoldM<TMonad, TA, TB>(f, acc, xs));
                },
                () => monadAdapter.Return(a));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad FoldM_<TMonad, TA, TB>(Func<TA, TB, IMonad<TA>> f, TA a, IEnumerable<TB> bs)
            where TMonad : IMonad<Unit>
        // ReSharper restore InconsistentNaming
        {
            var monadAdapter = MonadAdapterRegistry.Get(typeof(TMonad));
            return (TMonad)monadAdapter.BindIgnoringLeft(FoldM<IMonad<TA>, TA, TB>(f, a, bs), monadAdapter.Return(new Unit()));
        }

        public static TMonad ZipWithM<TMonad, TA, TB, TC>(Func<TA, TB, IMonad<TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs)
            where TMonad : IMonad<IEnumerable<TC>>
        {
            return Sequence<TMonad, TC>(@as.Zip(bs, f));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad ZipWithM_<TMonad, TA, TB, TC>(Func<TA, TB, IMonad<TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs)
            where TMonad : IMonad<Unit>
        // ReSharper restore InconsistentNaming
        {
            return Sequence_<TMonad, TC>(@as.Zip(bs, f));
        }

        public static IMonad<IEnumerable<TA>> FilterMInternal<TA>(Func<TA, IMonad<bool>> p, IEnumerable<TA> @as, MonadAdapter monadAdapter)
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

        public static TMonad When<TMonad>(bool b, TMonad m)
			where TMonad : IMonad<Unit>
        {
            var monadAdapter = MonadAdapterRegistry.Get(typeof(TMonad));
            return (TMonad)(b ? m : monadAdapter.Return(new Unit()));
        }

        public static TMonad Unless<TMonad>(bool b, TMonad m)
			where TMonad : IMonad<Unit>
        {
            return When(!b, m);
        }

        // ReSharper disable FunctionRecursiveOnAllPaths
        public static TMonad Forever<TMonad, TA, TB>(IMonad<TA> m)
			where TMonad : IMonad<TB>
        {
            var monadAdapter = MonadAdapterRegistry.Get(typeof(TMonad));
            return (TMonad)monadAdapter.BindIgnoringLeft(m, Forever<TMonad, TA, TB>(m));
        }
        // ReSharper restore FunctionRecursiveOnAllPaths

        public static TMonad Void<TMonad, TA>(IMonad<TA> m)
			where TMonad : IMonad<Unit>
        {
            var monadAdapter = MonadAdapterRegistry.Get(typeof(TMonad));
            return (TMonad)monadAdapter.BindIgnoringLeft(m, monadAdapter.Return(new Unit()));
        }

        public static TMonad Ap<TMonad, TA, TB>(IMonad<Func<TA, TB>> mf, IMonad<TA> ma)
            where TMonad : IMonad<TB>
        {
            return LiftM2<TMonad, Func<TA, TB>, TA, TB>((f, a) => f(a), mf, ma);
        }

        public static Func<TA, IMonad<TC>> Compose<TA, TB, TC>(Func<TA, IMonad<TB>> f, Func<TB, IMonad<TC>> g)
        {
            return a =>
            {
                var mb = f(a);
                var monadAdapter = mb.GetMonadAdapter();
                return monadAdapter.Bind(mb, g);
            };
        }
    }

    internal static partial class MonadCominatorsQueryExtensions
    {
        public static IMonad<T1, TB> Select<T1, TA, TB>(this IMonad<T1, TA> ma, Func<TA, TB> f)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Return(f(a)));
        }

        public static IMonad<T1, TB> SelectMany<T1, TA, TB>(this IMonad<T1, TA> ma, Func<TA, IMonad<T1, TB>> f) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(ma, f);
        }

        public static IMonad<T1, TC> SelectMany<T1, TA, TB, TC>(this IMonad<T1, TA> ma, Func<TA, IMonad<T1, TB>> f1, Func<TA, TB, TC> f2) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Bind(
                    f1(a), b => monadAdapter.Return(f2(a, b))));
        }
    }

    public static partial class MonadCombinators<T1>
    {
        public static TMonad LiftM<TMonad, TA, TB>(Func<TA, TB> f, IMonad<T1, TA> ma)
            where TMonad : IMonad<T1, TB>
        {
            return (TMonad)
                from a in ma
                select f(a);
        }

        public static TMonad LiftM2<TMonad, TA, TB, TC>(Func<TA, TB, TC> f, IMonad<T1, TA> ma, IMonad<T1, TB> mb)
            where TMonad : IMonad<T1, TC>
        {
            return (TMonad)
                from a in ma
                from b in mb
                select f(a, b);
        }

        public static TMonad LiftM3<TMonad, TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, IMonad<T1, TA> ma, IMonad<T1, TB> mb, IMonad<T1, TC> mc)
            where TMonad : IMonad<T1, TD>
        {
            return (TMonad)
                from a in ma
                from b in mb
                from c in mc
                select f(a, b, c);
        }

        public static TMonad LiftM4<TMonad, TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, IMonad<T1, TA> ma, IMonad<T1, TB> mb, IMonad<T1, TC> mc, IMonad<T1, TD> md)
            where TMonad : IMonad<T1, TE>
        {
            return (TMonad)
                from a in ma
                from b in mb
                from c in mc
                from d in md
                select f(a, b, c, d);
        }

        public static TMonad LiftM5<TMonad, TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, IMonad<T1, TA> ma, IMonad<T1, TB> mb, IMonad<T1, TC> mc, IMonad<T1, TD> md, IMonad<T1, TE> me)
            where TMonad : IMonad<T1, TF>
        {
            return (TMonad)
                from a in ma
                from b in mb
                from c in mc
                from d in md
                from e in me
                select f(a, b, c, d, e);
        }

        public static TMonad Join<TMonadMonad, TMonad, TA>(TMonadMonad mma)
            where TMonadMonad : IMonad<T1, TMonad>
            where TMonad : IMonad<T1, TA>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1>(typeof(TMonad));
            return (TMonad)monadAdapter.Bind(mma, ma => ma);
        }

        public static TMonad Sequence<TMonad, TA>(IEnumerable<IMonad<T1, TA>> ms)
            where TMonad : IMonad<T1, IEnumerable<TA>>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1>(typeof(TMonad));
            var z = monadAdapter.Return(MonadHelpers.Nil<TA>());
            return (TMonad)ms.FoldRight(
                z, (m, mtick) => monadAdapter.Bind(
                    m, x => monadAdapter.Bind(
                        mtick, xs => monadAdapter.Return(MonadHelpers.Cons(x, xs)))));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad Sequence_<TMonad, TA>(IEnumerable<IMonad<T1, TA>> ms)
            where TMonad : IMonad<T1, Unit>
        // ReSharper restore InconsistentNaming
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1>(typeof(TMonad));
            var z = monadAdapter.Return(new Unit());
            return (TMonad)ms.FoldRight(z, monadAdapter.BindIgnoringLeft);
        }

        public static TMonad MapM<TMonad, TA, TB>(Func<TA, IMonad<T1, TB>> f, IEnumerable<TA> @as)
            where TMonad : IMonad<T1, IEnumerable<TB>>
        {
            return Sequence<TMonad, TB>(@as.Map(f));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad MapM_<TMonad, TA, TB>(Func<TA, IMonad<T1, TB>> f, IEnumerable<TA> @as)
            where TMonad : IMonad<T1, Unit>
        // ReSharper restore InconsistentNaming
        {
            return Sequence_<TMonad, TB>(@as.Map(f));
        }

        public static TMonad ReplicateM<TMonad, TA>(int n, IMonad<T1, TA> ma)
            where TMonad : IMonad<T1, IEnumerable<TA>>
        {
            return Sequence<TMonad, TA>(System.Linq.Enumerable.Repeat(ma, n));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad ReplicateM_<TMonad, TA>(int n, IMonad<T1, TA> ma)
            where TMonad : IMonad<T1, Unit>
        // ReSharper restore InconsistentNaming
        {
            return Sequence_<TMonad, TA>(System.Linq.Enumerable.Repeat(ma, n));
        }

        public static TMonad FoldM<TMonad, TA, TB>(Func<TA, TB, IMonad<T1, TA>> f, TA a, IEnumerable<TB> bs)
            where TMonad : IMonad<T1, TA>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1>(typeof(TMonad));
            return (TMonad)bs.HeadAndTail().Match(
                tuple =>
                {
                    var x = tuple.Item1;
                    var xs = tuple.Item2;
                    var m = f(a, x);
                    return monadAdapter.Bind(m, acc => FoldM<TMonad, TA, TB>(f, acc, xs));
                },
                () => monadAdapter.Return(a));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad FoldM_<TMonad, TA, TB>(Func<TA, TB, IMonad<T1, TA>> f, TA a, IEnumerable<TB> bs)
            where TMonad : IMonad<T1, Unit>
        // ReSharper restore InconsistentNaming
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1>(typeof(TMonad));
            return (TMonad)monadAdapter.BindIgnoringLeft(FoldM<IMonad<T1, TA>, TA, TB>(f, a, bs), monadAdapter.Return(new Unit()));
        }

        public static TMonad ZipWithM<TMonad, TA, TB, TC>(Func<TA, TB, IMonad<T1, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs)
            where TMonad : IMonad<T1, IEnumerable<TC>>
        {
            return Sequence<TMonad, TC>(@as.Zip(bs, f));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad ZipWithM_<TMonad, TA, TB, TC>(Func<TA, TB, IMonad<T1, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs)
            where TMonad : IMonad<T1, Unit>
        // ReSharper restore InconsistentNaming
        {
            return Sequence_<TMonad, TC>(@as.Zip(bs, f));
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

        public static TMonad When<TMonad>(bool b, TMonad m)
			where TMonad : IMonad<T1, Unit>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1>(typeof(TMonad));
            return (TMonad)(b ? m : monadAdapter.Return(new Unit()));
        }

        public static TMonad Unless<TMonad>(bool b, TMonad m)
			where TMonad : IMonad<T1, Unit>
        {
            return When(!b, m);
        }

        // ReSharper disable FunctionRecursiveOnAllPaths
        public static TMonad Forever<TMonad, TA, TB>(IMonad<T1, TA> m)
			where TMonad : IMonad<T1, TB>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1>(typeof(TMonad));
            return (TMonad)monadAdapter.BindIgnoringLeft(m, Forever<TMonad, TA, TB>(m));
        }
        // ReSharper restore FunctionRecursiveOnAllPaths

        public static TMonad Void<TMonad, TA>(IMonad<T1, TA> m)
			where TMonad : IMonad<T1, Unit>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1>(typeof(TMonad));
            return (TMonad)monadAdapter.BindIgnoringLeft(m, monadAdapter.Return(new Unit()));
        }

        public static TMonad Ap<TMonad, TA, TB>(IMonad<T1, Func<TA, TB>> mf, IMonad<T1, TA> ma)
            where TMonad : IMonad<T1, TB>
        {
            return LiftM2<TMonad, Func<TA, TB>, TA, TB>((f, a) => f(a), mf, ma);
        }

        public static Func<TA, IMonad<T1, TC>> Compose<TA, TB, TC>(Func<TA, IMonad<T1, TB>> f, Func<TB, IMonad<T1, TC>> g)
        {
            return a =>
            {
                var mb = f(a);
                var monadAdapter = mb.GetMonadAdapter();
                return monadAdapter.Bind(mb, g);
            };
        }
    }

    internal static partial class MonadCominatorsQueryExtensions
    {
        public static IMonad<T1, T2, TB> Select<T1, T2, TA, TB>(this IMonad<T1, T2, TA> ma, Func<TA, TB> f)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Return(f(a)));
        }

        public static IMonad<T1, T2, TB> SelectMany<T1, T2, TA, TB>(this IMonad<T1, T2, TA> ma, Func<TA, IMonad<T1, T2, TB>> f) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(ma, f);
        }

        public static IMonad<T1, T2, TC> SelectMany<T1, T2, TA, TB, TC>(this IMonad<T1, T2, TA> ma, Func<TA, IMonad<T1, T2, TB>> f1, Func<TA, TB, TC> f2) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Bind(
                    f1(a), b => monadAdapter.Return(f2(a, b))));
        }
    }

    public static partial class MonadCombinators<T1, T2>
    {
        public static TMonad LiftM<TMonad, TA, TB>(Func<TA, TB> f, IMonad<T1, T2, TA> ma)
            where TMonad : IMonad<T1, T2, TB>
        {
            return (TMonad)
                from a in ma
                select f(a);
        }

        public static TMonad LiftM2<TMonad, TA, TB, TC>(Func<TA, TB, TC> f, IMonad<T1, T2, TA> ma, IMonad<T1, T2, TB> mb)
            where TMonad : IMonad<T1, T2, TC>
        {
            return (TMonad)
                from a in ma
                from b in mb
                select f(a, b);
        }

        public static TMonad LiftM3<TMonad, TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, IMonad<T1, T2, TA> ma, IMonad<T1, T2, TB> mb, IMonad<T1, T2, TC> mc)
            where TMonad : IMonad<T1, T2, TD>
        {
            return (TMonad)
                from a in ma
                from b in mb
                from c in mc
                select f(a, b, c);
        }

        public static TMonad LiftM4<TMonad, TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, IMonad<T1, T2, TA> ma, IMonad<T1, T2, TB> mb, IMonad<T1, T2, TC> mc, IMonad<T1, T2, TD> md)
            where TMonad : IMonad<T1, T2, TE>
        {
            return (TMonad)
                from a in ma
                from b in mb
                from c in mc
                from d in md
                select f(a, b, c, d);
        }

        public static TMonad LiftM5<TMonad, TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, IMonad<T1, T2, TA> ma, IMonad<T1, T2, TB> mb, IMonad<T1, T2, TC> mc, IMonad<T1, T2, TD> md, IMonad<T1, T2, TE> me)
            where TMonad : IMonad<T1, T2, TF>
        {
            return (TMonad)
                from a in ma
                from b in mb
                from c in mc
                from d in md
                from e in me
                select f(a, b, c, d, e);
        }

        public static TMonad Join<TMonadMonad, TMonad, TA>(TMonadMonad mma)
            where TMonadMonad : IMonad<T1, T2, TMonad>
            where TMonad : IMonad<T1, T2, TA>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2>(typeof(TMonad));
            return (TMonad)monadAdapter.Bind(mma, ma => ma);
        }

        public static TMonad Sequence<TMonad, TA>(IEnumerable<IMonad<T1, T2, TA>> ms)
            where TMonad : IMonad<T1, T2, IEnumerable<TA>>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2>(typeof(TMonad));
            var z = monadAdapter.Return(MonadHelpers.Nil<TA>());
            return (TMonad)ms.FoldRight(
                z, (m, mtick) => monadAdapter.Bind(
                    m, x => monadAdapter.Bind(
                        mtick, xs => monadAdapter.Return(MonadHelpers.Cons(x, xs)))));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad Sequence_<TMonad, TA>(IEnumerable<IMonad<T1, T2, TA>> ms)
            where TMonad : IMonad<T1, T2, Unit>
        // ReSharper restore InconsistentNaming
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2>(typeof(TMonad));
            var z = monadAdapter.Return(new Unit());
            return (TMonad)ms.FoldRight(z, monadAdapter.BindIgnoringLeft);
        }

        public static TMonad MapM<TMonad, TA, TB>(Func<TA, IMonad<T1, T2, TB>> f, IEnumerable<TA> @as)
            where TMonad : IMonad<T1, T2, IEnumerable<TB>>
        {
            return Sequence<TMonad, TB>(@as.Map(f));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad MapM_<TMonad, TA, TB>(Func<TA, IMonad<T1, T2, TB>> f, IEnumerable<TA> @as)
            where TMonad : IMonad<T1, T2, Unit>
        // ReSharper restore InconsistentNaming
        {
            return Sequence_<TMonad, TB>(@as.Map(f));
        }

        public static TMonad ReplicateM<TMonad, TA>(int n, IMonad<T1, T2, TA> ma)
            where TMonad : IMonad<T1, T2, IEnumerable<TA>>
        {
            return Sequence<TMonad, TA>(System.Linq.Enumerable.Repeat(ma, n));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad ReplicateM_<TMonad, TA>(int n, IMonad<T1, T2, TA> ma)
            where TMonad : IMonad<T1, T2, Unit>
        // ReSharper restore InconsistentNaming
        {
            return Sequence_<TMonad, TA>(System.Linq.Enumerable.Repeat(ma, n));
        }

        public static TMonad FoldM<TMonad, TA, TB>(Func<TA, TB, IMonad<T1, T2, TA>> f, TA a, IEnumerable<TB> bs)
            where TMonad : IMonad<T1, T2, TA>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2>(typeof(TMonad));
            return (TMonad)bs.HeadAndTail().Match(
                tuple =>
                {
                    var x = tuple.Item1;
                    var xs = tuple.Item2;
                    var m = f(a, x);
                    return monadAdapter.Bind(m, acc => FoldM<TMonad, TA, TB>(f, acc, xs));
                },
                () => monadAdapter.Return(a));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad FoldM_<TMonad, TA, TB>(Func<TA, TB, IMonad<T1, T2, TA>> f, TA a, IEnumerable<TB> bs)
            where TMonad : IMonad<T1, T2, Unit>
        // ReSharper restore InconsistentNaming
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2>(typeof(TMonad));
            return (TMonad)monadAdapter.BindIgnoringLeft(FoldM<IMonad<T1, T2, TA>, TA, TB>(f, a, bs), monadAdapter.Return(new Unit()));
        }

        public static TMonad ZipWithM<TMonad, TA, TB, TC>(Func<TA, TB, IMonad<T1, T2, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs)
            where TMonad : IMonad<T1, T2, IEnumerable<TC>>
        {
            return Sequence<TMonad, TC>(@as.Zip(bs, f));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad ZipWithM_<TMonad, TA, TB, TC>(Func<TA, TB, IMonad<T1, T2, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs)
            where TMonad : IMonad<T1, T2, Unit>
        // ReSharper restore InconsistentNaming
        {
            return Sequence_<TMonad, TC>(@as.Zip(bs, f));
        }

        public static IMonad<T1, T2, IEnumerable<TA>> FilterMInternal<TA>(Func<TA, IMonad<T1, T2, bool>> p, IEnumerable<TA> @as, MonadAdapter<T1, T2> monadAdapter)
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

        public static TMonad When<TMonad>(bool b, TMonad m)
			where TMonad : IMonad<T1, T2, Unit>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2>(typeof(TMonad));
            return (TMonad)(b ? m : monadAdapter.Return(new Unit()));
        }

        public static TMonad Unless<TMonad>(bool b, TMonad m)
			where TMonad : IMonad<T1, T2, Unit>
        {
            return When(!b, m);
        }

        // ReSharper disable FunctionRecursiveOnAllPaths
        public static TMonad Forever<TMonad, TA, TB>(IMonad<T1, T2, TA> m)
			where TMonad : IMonad<T1, T2, TB>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2>(typeof(TMonad));
            return (TMonad)monadAdapter.BindIgnoringLeft(m, Forever<TMonad, TA, TB>(m));
        }
        // ReSharper restore FunctionRecursiveOnAllPaths

        public static TMonad Void<TMonad, TA>(IMonad<T1, T2, TA> m)
			where TMonad : IMonad<T1, T2, Unit>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2>(typeof(TMonad));
            return (TMonad)monadAdapter.BindIgnoringLeft(m, monadAdapter.Return(new Unit()));
        }

        public static TMonad Ap<TMonad, TA, TB>(IMonad<T1, T2, Func<TA, TB>> mf, IMonad<T1, T2, TA> ma)
            where TMonad : IMonad<T1, T2, TB>
        {
            return LiftM2<TMonad, Func<TA, TB>, TA, TB>((f, a) => f(a), mf, ma);
        }

        public static Func<TA, IMonad<T1, T2, TC>> Compose<TA, TB, TC>(Func<TA, IMonad<T1, T2, TB>> f, Func<TB, IMonad<T1, T2, TC>> g)
        {
            return a =>
            {
                var mb = f(a);
                var monadAdapter = mb.GetMonadAdapter();
                return monadAdapter.Bind(mb, g);
            };
        }
    }

    internal static partial class MonadCominatorsQueryExtensions
    {
        public static IMonad<T1, T2, T3, TB> Select<T1, T2, T3, TA, TB>(this IMonad<T1, T2, T3, TA> ma, Func<TA, TB> f)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Return(f(a)));
        }

        public static IMonad<T1, T2, T3, TB> SelectMany<T1, T2, T3, TA, TB>(this IMonad<T1, T2, T3, TA> ma, Func<TA, IMonad<T1, T2, T3, TB>> f) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(ma, f);
        }

        public static IMonad<T1, T2, T3, TC> SelectMany<T1, T2, T3, TA, TB, TC>(this IMonad<T1, T2, T3, TA> ma, Func<TA, IMonad<T1, T2, T3, TB>> f1, Func<TA, TB, TC> f2) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return monadAdapter.Bind(
                ma, a => monadAdapter.Bind(
                    f1(a), b => monadAdapter.Return(f2(a, b))));
        }
    }

    public static partial class MonadCombinators<T1, T2, T3>
    {
        public static TMonad LiftM<TMonad, TA, TB>(Func<TA, TB> f, IMonad<T1, T2, T3, TA> ma)
            where TMonad : IMonad<T1, T2, T3, TB>
        {
            return (TMonad)
                from a in ma
                select f(a);
        }

        public static TMonad LiftM2<TMonad, TA, TB, TC>(Func<TA, TB, TC> f, IMonad<T1, T2, T3, TA> ma, IMonad<T1, T2, T3, TB> mb)
            where TMonad : IMonad<T1, T2, T3, TC>
        {
            return (TMonad)
                from a in ma
                from b in mb
                select f(a, b);
        }

        public static TMonad LiftM3<TMonad, TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, IMonad<T1, T2, T3, TA> ma, IMonad<T1, T2, T3, TB> mb, IMonad<T1, T2, T3, TC> mc)
            where TMonad : IMonad<T1, T2, T3, TD>
        {
            return (TMonad)
                from a in ma
                from b in mb
                from c in mc
                select f(a, b, c);
        }

        public static TMonad LiftM4<TMonad, TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, IMonad<T1, T2, T3, TA> ma, IMonad<T1, T2, T3, TB> mb, IMonad<T1, T2, T3, TC> mc, IMonad<T1, T2, T3, TD> md)
            where TMonad : IMonad<T1, T2, T3, TE>
        {
            return (TMonad)
                from a in ma
                from b in mb
                from c in mc
                from d in md
                select f(a, b, c, d);
        }

        public static TMonad LiftM5<TMonad, TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, IMonad<T1, T2, T3, TA> ma, IMonad<T1, T2, T3, TB> mb, IMonad<T1, T2, T3, TC> mc, IMonad<T1, T2, T3, TD> md, IMonad<T1, T2, T3, TE> me)
            where TMonad : IMonad<T1, T2, T3, TF>
        {
            return (TMonad)
                from a in ma
                from b in mb
                from c in mc
                from d in md
                from e in me
                select f(a, b, c, d, e);
        }

        public static TMonad Join<TMonadMonad, TMonad, TA>(TMonadMonad mma)
            where TMonadMonad : IMonad<T1, T2, T3, TMonad>
            where TMonad : IMonad<T1, T2, T3, TA>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2, T3>(typeof(TMonad));
            return (TMonad)monadAdapter.Bind(mma, ma => ma);
        }

        public static TMonad Sequence<TMonad, TA>(IEnumerable<IMonad<T1, T2, T3, TA>> ms)
            where TMonad : IMonad<T1, T2, T3, IEnumerable<TA>>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2, T3>(typeof(TMonad));
            var z = monadAdapter.Return(MonadHelpers.Nil<TA>());
            return (TMonad)ms.FoldRight(
                z, (m, mtick) => monadAdapter.Bind(
                    m, x => monadAdapter.Bind(
                        mtick, xs => monadAdapter.Return(MonadHelpers.Cons(x, xs)))));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad Sequence_<TMonad, TA>(IEnumerable<IMonad<T1, T2, T3, TA>> ms)
            where TMonad : IMonad<T1, T2, T3, Unit>
        // ReSharper restore InconsistentNaming
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2, T3>(typeof(TMonad));
            var z = monadAdapter.Return(new Unit());
            return (TMonad)ms.FoldRight(z, monadAdapter.BindIgnoringLeft);
        }

        public static TMonad MapM<TMonad, TA, TB>(Func<TA, IMonad<T1, T2, T3, TB>> f, IEnumerable<TA> @as)
            where TMonad : IMonad<T1, T2, T3, IEnumerable<TB>>
        {
            return Sequence<TMonad, TB>(@as.Map(f));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad MapM_<TMonad, TA, TB>(Func<TA, IMonad<T1, T2, T3, TB>> f, IEnumerable<TA> @as)
            where TMonad : IMonad<T1, T2, T3, Unit>
        // ReSharper restore InconsistentNaming
        {
            return Sequence_<TMonad, TB>(@as.Map(f));
        }

        public static TMonad ReplicateM<TMonad, TA>(int n, IMonad<T1, T2, T3, TA> ma)
            where TMonad : IMonad<T1, T2, T3, IEnumerable<TA>>
        {
            return Sequence<TMonad, TA>(System.Linq.Enumerable.Repeat(ma, n));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad ReplicateM_<TMonad, TA>(int n, IMonad<T1, T2, T3, TA> ma)
            where TMonad : IMonad<T1, T2, T3, Unit>
        // ReSharper restore InconsistentNaming
        {
            return Sequence_<TMonad, TA>(System.Linq.Enumerable.Repeat(ma, n));
        }

        public static TMonad FoldM<TMonad, TA, TB>(Func<TA, TB, IMonad<T1, T2, T3, TA>> f, TA a, IEnumerable<TB> bs)
            where TMonad : IMonad<T1, T2, T3, TA>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2, T3>(typeof(TMonad));
            return (TMonad)bs.HeadAndTail().Match(
                tuple =>
                {
                    var x = tuple.Item1;
                    var xs = tuple.Item2;
                    var m = f(a, x);
                    return monadAdapter.Bind(m, acc => FoldM<TMonad, TA, TB>(f, acc, xs));
                },
                () => monadAdapter.Return(a));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad FoldM_<TMonad, TA, TB>(Func<TA, TB, IMonad<T1, T2, T3, TA>> f, TA a, IEnumerable<TB> bs)
            where TMonad : IMonad<T1, T2, T3, Unit>
        // ReSharper restore InconsistentNaming
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2, T3>(typeof(TMonad));
            return (TMonad)monadAdapter.BindIgnoringLeft(FoldM<IMonad<T1, T2, T3, TA>, TA, TB>(f, a, bs), monadAdapter.Return(new Unit()));
        }

        public static TMonad ZipWithM<TMonad, TA, TB, TC>(Func<TA, TB, IMonad<T1, T2, T3, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs)
            where TMonad : IMonad<T1, T2, T3, IEnumerable<TC>>
        {
            return Sequence<TMonad, TC>(@as.Zip(bs, f));
        }

        // ReSharper disable InconsistentNaming
        public static TMonad ZipWithM_<TMonad, TA, TB, TC>(Func<TA, TB, IMonad<T1, T2, T3, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs)
            where TMonad : IMonad<T1, T2, T3, Unit>
        // ReSharper restore InconsistentNaming
        {
            return Sequence_<TMonad, TC>(@as.Zip(bs, f));
        }

        public static IMonad<T1, T2, T3, IEnumerable<TA>> FilterMInternal<TA>(Func<TA, IMonad<T1, T2, T3, bool>> p, IEnumerable<TA> @as, MonadAdapter<T1, T2, T3> monadAdapter)
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

        public static TMonad When<TMonad>(bool b, TMonad m)
			where TMonad : IMonad<T1, T2, T3, Unit>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2, T3>(typeof(TMonad));
            return (TMonad)(b ? m : monadAdapter.Return(new Unit()));
        }

        public static TMonad Unless<TMonad>(bool b, TMonad m)
			where TMonad : IMonad<T1, T2, T3, Unit>
        {
            return When(!b, m);
        }

        // ReSharper disable FunctionRecursiveOnAllPaths
        public static TMonad Forever<TMonad, TA, TB>(IMonad<T1, T2, T3, TA> m)
			where TMonad : IMonad<T1, T2, T3, TB>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2, T3>(typeof(TMonad));
            return (TMonad)monadAdapter.BindIgnoringLeft(m, Forever<TMonad, TA, TB>(m));
        }
        // ReSharper restore FunctionRecursiveOnAllPaths

        public static TMonad Void<TMonad, TA>(IMonad<T1, T2, T3, TA> m)
			where TMonad : IMonad<T1, T2, T3, Unit>
        {
            var monadAdapter = MonadAdapterRegistry.Get<T1, T2, T3>(typeof(TMonad));
            return (TMonad)monadAdapter.BindIgnoringLeft(m, monadAdapter.Return(new Unit()));
        }

        public static TMonad Ap<TMonad, TA, TB>(IMonad<T1, T2, T3, Func<TA, TB>> mf, IMonad<T1, T2, T3, TA> ma)
            where TMonad : IMonad<T1, T2, T3, TB>
        {
            return LiftM2<TMonad, Func<TA, TB>, TA, TB>((f, a) => f(a), mf, ma);
        }

        public static Func<TA, IMonad<T1, T2, T3, TC>> Compose<TA, TB, TC>(Func<TA, IMonad<T1, T2, T3, TB>> f, Func<TB, IMonad<T1, T2, T3, TC>> g)
        {
            return a =>
            {
                var mb = f(a);
                var monadAdapter = mb.GetMonadAdapter();
                return monadAdapter.Bind(mb, g);
            };
        }
    }
}
