using System;

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

    public static partial class Reader
    {
        public static Reader<TR, TA> Local<TR, TA>(Func<TR, TR> f, Reader<TR, TA> ma)
        {
            return ma.Local(f);
        }

        public static Reader<TR, TA> Local<TR, TA>(this Reader<TR, TA> ma, Func<TR, TR> f)
        {
            return new Reader<TR, TA>(r => ma.RunReader(f(r)));
        }

        public static Reader<TR, TA> Asks<TR, TA>(Func<TR, TA> f)
        {
            return Reader<TR>.Ask().Bind(r => Reader<TR>.Return(f(r)));
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
