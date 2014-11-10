using System;
using System.Collections.Generic;

namespace MonadLib
{
    public class Reader<TR, TA> : IMonad<TR, TA>
    {
        internal Reader(Func<TR, TA> runReader)
        {
            RunReader = runReader;
        }

        public Func<TR, TA> RunReader { get; private set; }

        private ReaderMonadAdapter<TR> _monadAdapter;

        public MonadAdapter<TR> GetMonadAdapter()
        {
            return _monadAdapter ?? (_monadAdapter = new ReaderMonadAdapter<TR>());
        }
    }

    public static class Reader
    {
        public static Reader<TR, TA> Local<TR, TA>(Func<TR, TR> f, Reader<TR, TA> ma)
        {
            return new Reader<TR, TA>(r => ma.RunReader(f(r)));
        }

        public static Reader<TR, TA> Local<TR, TA>(this Reader<TR, TA> ma, Func<TR, TR> f)
        {
            return new Reader<TR, TA>(r => ma.RunReader(f(r)));
        }

        public static Reader<TR, TA> Asks<TR, TA>(Func<TR, TA> f)
        {
            return Reader<TR>.Ask().Bind(r => Reader<TR>.Return(f(r)));
        }

        public static Reader<TR, TB> Bind<TR, TA, TB>(this Reader<TR, TA> ma, Func<TA, Reader<TR, TB>> f)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (Reader<TR, TB>)monadAdapter.Bind(ma, f);
        }

        public static Reader<TR, TB> BindIgnoringLeft<TR, TA, TB>(this Reader<TR, TA> ma, Reader<TR, TB> mb)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (Reader<TR, TB>)monadAdapter.BindIgnoringLeft(ma, mb);
        }

        public static Reader<TR, TB> LiftM<TR, TA, TB>(this Reader<TR, TA> ma, Func<TA, TB> f)
        {
            return (Reader<TR, TB>)MonadCombinators<TR>.LiftM(f, ma);
        }

        public static Either<TLeft, TB> LiftM<TLeft, TA, TB>(Func<TA, TB> f, Either<TLeft, TA> ma)
        {
            return (Either<TLeft, TB>)MonadCombinators<TLeft>.LiftM(f, ma);
        }

        public static Reader<TR, TC> LiftM2<TR, TA, TB, TC>(this Reader<TR, TA> ma, Reader<TR, TB> mb, Func<TA, TB, TC> f)
        {
            return (Reader<TR, TC>)MonadCombinators<TR>.LiftM2(f, ma, mb);
        }

        public static Reader<TR, TC> LiftM2<TR, TA, TB, TC>(Func<TA, TB, TC> f, Reader<TR, TA> ma, Reader<TR, TB> mb)
        {
            return (Reader<TR, TC>)MonadCombinators<TR>.LiftM2(f, ma, mb);
        }

        public static Reader<TR, TD> LiftM3<TR, TA, TB, TC, TD>(this Reader<TR, TA> ma, Reader<TR, TB> mb, Reader<TR, TC> mc, Func<TA, TB, TC, TD> f)
        {
            return (Reader<TR, TD>)MonadCombinators<TR>.LiftM3(f, ma, mb, mc);
        }

        public static Reader<TR, TD> LiftM3<TR, TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, Reader<TR, TA> ma, Reader<TR, TB> mb, Reader<TR, TC> mc)
        {
            return (Reader<TR, TD>)MonadCombinators<TR>.LiftM3(f, ma, mb, mc);
        }

        public static Reader<TR, TE> LiftM4<TR, TA, TB, TC, TD, TE>(this Reader<TR, TA> ma, Reader<TR, TB> mb, Reader<TR, TC> mc, Reader<TR, TD> md, Func<TA, TB, TC, TD, TE> f)
        {
            return (Reader<TR, TE>)MonadCombinators<TR>.LiftM4(f, ma, mb, mc, md);
        }

        public static Reader<TR, TE> LiftM4<TR, TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, Reader<TR, TA> ma, Reader<TR, TB> mb, Reader<TR, TC> mc, Reader<TR, TD> md)
        {
            return (Reader<TR, TE>)MonadCombinators<TR>.LiftM4(f, ma, mb, mc, md);
        }

        public static Reader<TR, TF> LiftM5<TR, TA, TB, TC, TD, TE, TF>(this Reader<TR, TA> ma, Reader<TR, TB> mb, Reader<TR, TC> mc, Reader<TR, TD> md, Reader<TR, TE> me, Func<TA, TB, TC, TD, TE, TF> f)
        {
            return (Reader<TR, TF>)MonadCombinators<TR>.LiftM5(f, ma, mb, mc, md, me);
        }

        public static Reader<TR, TF> LiftM5<TR, TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, Reader<TR, TA> ma, Reader<TR, TB> mb, Reader<TR, TC> mc, Reader<TR, TD> md, Reader<TR, TE> me)
        {
            return (Reader<TR, TF>)MonadCombinators<TR>.LiftM5(f, ma, mb, mc, md, me);
        }

        public static Reader<TR, IEnumerable<TA>> Sequence<TR, TA>(IEnumerable<Reader<TR, TA>> ms)
        {
            return (Reader<TR, IEnumerable<TA>>)MonadCombinators<TR>.SequenceInternal(ms, new ReaderMonadAdapter<TR>());
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> Sequence_<TR, TA>(IEnumerable<Reader<TR, TA>> ms)
        // ReSharper restore InconsistentNaming
        {
            return (Reader<TR, Unit>)MonadCombinators<TR>.SequenceInternal_(ms, new ReaderMonadAdapter<TR>());
        }

        public static Reader<TR, IEnumerable<TB>> MapM<TR, TA, TB>(Func<TA, Reader<TR, TB>> f, IEnumerable<TA> @as)
        {
            return (Reader<TR, IEnumerable<TB>>)MonadCombinators<TR>.MapMInternal(f, @as, new ReaderMonadAdapter<TR>());
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> MapM_<TR, TA, TB>(Func<TA, Reader<TR, TB>> f, IEnumerable<TA> @as)
        // ReSharper restore InconsistentNaming
        {
            return (Reader<TR, Unit>)MonadCombinators<TR>.MapMInternal_(f, @as, new ReaderMonadAdapter<TR>());
        }

        public static Reader<TR, IEnumerable<TB>> ForM<TR, TA, TB>(IEnumerable<TA> @as, Func<TA, Reader<TR, TB>> f)
        {
            return (Reader<TR, IEnumerable<TB>>)MonadCombinators<TR>.MapMInternal(f, @as, new ReaderMonadAdapter<TR>());
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> ForM_<TR, TA, TB>(IEnumerable<TA> @as, Func<TA, Reader<TR, TB>> f)
        // ReSharper restore InconsistentNaming
        {
            return (Reader<TR, Unit>)MonadCombinators<TR>.MapMInternal_(f, @as, new ReaderMonadAdapter<TR>());
        }

        public static Reader<TR, IEnumerable<TA>> ReplicateM<TR, TA>(int n, Reader<TR, TA> ma)
        {
            return (Reader<TR, IEnumerable<TA>>)MonadCombinators<TR>.ReplicateM(n, ma);
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> ReplicateM_<TR, TA>(int n, Reader<TR, TA> ma)
        // ReSharper restore InconsistentNaming
        {
            return (Reader<TR, Unit>)MonadCombinators<TR>.ReplicateM_(n, ma);
        }

        public static Reader<TR, TA> Join<TR, TA>(Reader<TR, Reader<TR, TA>> mma)
        {
            // Ideally, we would like to use MonadCombinators<TR>.Join(mma) but there
            // is a casting issue that I have not figured out how to fix.
            var monadAdapter = mma.GetMonadAdapter();
            return (Reader<TR, TA>)monadAdapter.Bind(mma, MonadHelpers.Identity);
        }

        public static Reader<TR, TA> FoldM<TR, TA, TB>(Func<TA, TB, Reader<TR, TA>> f, TA a, IEnumerable<TB> bs)
        {
            return (Reader<TR, TA>)MonadCombinators<TR>.FoldMInternal(f, a, bs, new ReaderMonadAdapter<TR>());
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> FoldM_<TR, TA, TB>(Func<TA, TB, Reader<TR, TA>> f, TA a, IEnumerable<TB> bs)
        // ReSharper restore InconsistentNaming
        {
            return (Reader<TR, Unit>)MonadCombinators<TR>.FoldMInternal_(f, a, bs, new ReaderMonadAdapter<TR>());
        }

        public static Reader<TR, IEnumerable<TC>> ZipWithM<TR, TA, TB, TC>(Func<TA, TB, Reader<TR, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs)
        {
            return (Reader<TR, IEnumerable<TC>>)MonadCombinators<TR>.ZipWithMInternal(f, @as, bs, new ReaderMonadAdapter<TR>());
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> ZipWithM_<TR, TA, TB, TC>(Func<TA, TB, Reader<TR, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs)
        // ReSharper restore InconsistentNaming
        {
            return (Reader<TR, Unit>)MonadCombinators<TR>.ZipWithMInternal_(f, @as, bs, new ReaderMonadAdapter<TR>());
        }

        public static Reader<TR, IEnumerable<TA>> FilterM<TR, TA>(Func<TA, Reader<TR, bool>> p, IEnumerable<TA> @as)
        {
            return (Reader<TR, IEnumerable<TA>>)MonadCombinators<TR>.FilterMInternal(p, @as, new ReaderMonadAdapter<TR>());
        }

        public static Reader<TR, Unit> When<TR>(bool b, Reader<TR, Unit> m)
        {
            return (Reader<TR, Unit>)MonadCombinators<TR>.When(b, m);
        }

        public static Reader<TR, Unit> Unless<TR>(bool b, Reader<TR, Unit> m)
        {
            return (Reader<TR, Unit>)MonadCombinators<TR>.Unless(b, m);
        }

        public static Reader<TR, TB> Forever<TR, TA, TB>(Reader<TR, TA> m)
        {
            return (Reader<TR, TB>)MonadCombinators<TR>.Forever<TA, TB>(m);
        }

        public static Reader<TR, Unit> Void<TR, TA>(Reader<TR, TA> m)
        {
            return (Reader<TR, Unit>)MonadCombinators<TR>.Void(m);
        }

        public static Reader<TR, TB> Ap<TR, TA, TB>(Reader<TR, Func<TA, TB>> mf, Reader<TR, TA> ma)
        {
            return (Reader<TR, TB>)MonadCombinators<TR>.Ap(mf, ma);
        }
    }

    public static class Reader<TR>
    {
        public static Reader<TR, TR> Ask()
        {
            return new Reader<TR, TR>(MonadHelpers.Identity);
        }

        public static Reader<TR, TA> Return<TA>(TA a)
        {
            return new Reader<TR, TA>(_ => a);
        }
    }

    internal class ReaderMonadAdapter<TR> : MonadAdapter<TR>
    {
        public override IMonad<TR, TA> Return<TA>(TA a)
        {
            return Reader<TR>.Return(a);
        }

        public override IMonad<TR, TB> Bind<TA, TB>(IMonad<TR, TA> ma, Func<TA, IMonad<TR, TB>> f)
        {
            return new Reader<TR, TB>(r =>
                {
                    var readerA = (Reader<TR, TA>) ma;
                    var a = readerA.RunReader(r);
                    var mb = f(a);
                    var readerB = (Reader<TR, TB>) mb;
                    var b = readerB.RunReader(r);
                    return b;
                });
        }
    }
}
