using System;
using System.Collections.Generic;

namespace MonadLib
{
    public static partial class Maybe
    {
        public static Maybe<TB> Bind<TA, TB>(this Maybe<TA> ma, Func<TA, Maybe<TB>> f)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (Maybe<TB>)monadAdapter.Bind(ma, f);
        }

        public static Maybe<TB> BindIgnoringLeft<TA, TB>(this Maybe<TA> ma, Maybe<TB> mb)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (Maybe<TB>)monadAdapter.BindIgnoringLeft(ma, mb);
        }

        public static Maybe<TB> LiftM<TA, TB>(this Maybe<TA> ma, Func<TA, TB> f)
        {
            return (Maybe<TB>)MonadCombinators.LiftM(f, ma);
        }

        public static Maybe<TB> LiftM<TA, TB>(Func<TA, TB> f, Maybe<TA> ma)
        {
            return (Maybe<TB>)MonadCombinators.LiftM(f, ma);
        }

        public static Maybe<TC> LiftM2<TA, TB, TC>(this Maybe<TA> ma, Maybe<TB> mb, Func<TA, TB, TC> f)
        {
            return (Maybe<TC>)MonadCombinators.LiftM2(f, ma, mb);
        }

        public static Maybe<TC> LiftM2<TA, TB, TC>(Func<TA, TB, TC> f, Maybe<TA> ma, Maybe<TB> mb)
        {
            return (Maybe<TC>)MonadCombinators.LiftM2(f, ma, mb);
        }

        public static Maybe<TD> LiftM3<TA, TB, TC, TD>(this Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Func<TA, TB, TC, TD> f)
        {
            return (Maybe<TD>)MonadCombinators.LiftM3(f, ma, mb, mc);
        }

        public static Maybe<TD> LiftM3<TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc)
        {
            return (Maybe<TD>)MonadCombinators.LiftM3(f, ma, mb, mc);
        }

        public static Maybe<TE> LiftM4<TA, TB, TC, TD, TE>(this Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Maybe<TD> md, Func<TA, TB, TC, TD, TE> f)
        {
            return (Maybe<TE>)MonadCombinators.LiftM4(f, ma, mb, mc, md);
        }

        public static Maybe<TE> LiftM4<TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Maybe<TD> md)
        {
            return (Maybe<TE>)MonadCombinators.LiftM4(f, ma, mb, mc, md);
        }

        public static Maybe<TF> LiftM5<TA, TB, TC, TD, TE, TF>(this Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Maybe<TD> md, Maybe<TE> me, Func<TA, TB, TC, TD, TE, TF> f)
        {
            return (Maybe<TF>)MonadCombinators.LiftM5(f, ma, mb, mc, md, me);
        }

        public static Maybe<TF> LiftM5<TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Maybe<TD> md, Maybe<TE> me)
        {
            return (Maybe<TF>)MonadCombinators.LiftM5(f, ma, mb, mc, md, me);
        }

        public static Maybe<IEnumerable<TA>> Sequence<TA>(IEnumerable<Maybe<TA>> ms)
        {
            return (Maybe<IEnumerable<TA>>)MonadCombinators.SequenceInternal(ms, new MaybeMonadPlusAdapter<TA>());
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> Sequence_<TA>(IEnumerable<Maybe<TA>> ms)
        // ReSharper restore InconsistentNaming
        {
            return (Maybe<Unit>)MonadCombinators.SequenceInternal_(ms, new MaybeMonadPlusAdapter<TA>());
        }

        public static Maybe<IEnumerable<TB>> MapM<TA, TB>(Func<TA, Maybe<TB>> f, IEnumerable<TA> @as)
        {
            return (Maybe<IEnumerable<TB>>)MonadCombinators.MapMInternal(f, @as, new MaybeMonadPlusAdapter<TA>());
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> MapM_<TA, TB>(Func<TA, Maybe<TB>> f, IEnumerable<TA> @as)
        // ReSharper restore InconsistentNaming
        {
            return (Maybe<Unit>)MonadCombinators.MapMInternal_(f, @as, new MaybeMonadPlusAdapter<TA>());
        }

        public static Maybe<IEnumerable<TB>> ForM<TA, TB>(IEnumerable<TA> @as, Func<TA, Maybe<TB>> f)
        {
            return (Maybe<IEnumerable<TB>>)MonadCombinators.MapMInternal(f, @as, new MaybeMonadPlusAdapter<TA>());
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> ForM_<TA, TB>(IEnumerable<TA> @as, Func<TA, Maybe<TB>> f)
        // ReSharper restore InconsistentNaming
        {
            return (Maybe<Unit>)MonadCombinators.MapMInternal_(f, @as, new MaybeMonadPlusAdapter<TA>());
        }

        public static Maybe<IEnumerable<TA>> ReplicateM<TA>(int n, Maybe<TA> ma)
        {
            return (Maybe<IEnumerable<TA>>)MonadCombinators.ReplicateM(n, ma);
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> ReplicateM_<TA>(int n, Maybe<TA> ma)
        // ReSharper restore InconsistentNaming
        {
            return (Maybe<Unit>)MonadCombinators.ReplicateM_(n, ma);
        }

        public static Maybe<TA> Join<TA>(Maybe<Maybe<TA>> mma)
        {
            // Ideally, we would like to use MonadCombinators.Join(mma) but there
            // is a casting issue that I have not figured out how to fix.
            var monadAdapter = mma.GetMonadAdapter();
            return (Maybe<TA>)monadAdapter.Bind(mma, MonadHelpers.Identity);
        }

        public static Maybe<TA> MFilter<TA>(this Maybe<TA> ma, Func<TA, bool> p)
        {
            return (Maybe<TA>)MonadPlusCombinators.MFilter(p, ma);
        }

        public static Maybe<TA> MFilter<TA>(Func<TA, bool> p, Maybe<TA> ma)
        {
            return (Maybe<TA>)MonadPlusCombinators.MFilter(p, ma);
        }

        public static Maybe<TA> FoldM<TA, TB>(Func<TA, TB, Maybe<TA>> f, TA a, IEnumerable<TB> bs)
        {
            return (Maybe<TA>)MonadCombinators.FoldMInternal(f, a, bs, new MaybeMonadPlusAdapter<TA>());
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> FoldM_<TA, TB>(Func<TA, TB, Maybe<TA>> f, TA a, IEnumerable<TB> bs)
        // ReSharper restore InconsistentNaming
        {
            return (Maybe<Unit>)MonadCombinators.FoldMInternal_(f, a, bs, new MaybeMonadPlusAdapter<TA>());
        }

        public static Maybe<IEnumerable<TC>> ZipWithM<TA, TB, TC>(Func<TA, TB, Maybe<TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs)
        {
            return (Maybe<IEnumerable<TC>>)MonadCombinators.ZipWithMInternal(f, @as, bs, new MaybeMonadPlusAdapter<TA>());
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> ZipWithM_<TA, TB, TC>(Func<TA, TB, Maybe<TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs)
        // ReSharper restore InconsistentNaming
        {
            return (Maybe<Unit>)MonadCombinators.ZipWithMInternal_(f, @as, bs, new MaybeMonadPlusAdapter<TA>());
        }

        public static Maybe<IEnumerable<TA>> FilterM<TA>(Func<TA, Maybe<bool>> p, IEnumerable<TA> @as)
        {
            return (Maybe<IEnumerable<TA>>)MonadCombinators.FilterMInternal(p, @as, new MaybeMonadPlusAdapter<TA>());
        }

        public static Maybe<Unit> When(bool b, Maybe<Unit> m)
        {
            return (Maybe<Unit>)MonadCombinators.When(b, m);
        }

        public static Maybe<Unit> Unless(bool b, Maybe<Unit> m)
        {
            return (Maybe<Unit>)MonadCombinators.Unless(b, m);
        }

        public static Maybe<TB> Forever<TA, TB>(Maybe<TA> m)
        {
            return (Maybe<TB>)MonadCombinators.Forever<TA, TB>(m);
        }

        public static Maybe<Unit> Void<TA>(Maybe<TA> m)
        {
            return (Maybe<Unit>)MonadCombinators.Void(m);
        }

        public static Maybe<TB> Ap<TA, TB>(Maybe<Func<TA, TB>> mf, Maybe<TA> ma)
        {
            return (Maybe<TB>)MonadCombinators.Ap(mf, ma);
        }

        public static Func<TA, Maybe<TC>> Compose<TA, TB, TC>(Func<TA, Maybe<TB>> f, Func<TB, Maybe<TC>> g)
        {
            return a => (Maybe<TC>)MonadCombinators.Compose(f, g)(a);
        }
    }
}
