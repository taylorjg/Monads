using System;

namespace MonadLib
{
    public class Reader<TR, TA> : IMonad<TR, TA>
    {
        internal Reader(Func<TR, TA> runReader)
        {
            RunReader = runReader;
        }

        public Reader<TR, TR> Ask()
        {
            return new Reader<TR, TR>(MonadHelpers.Identity);
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
        // Monad combinator wrappers will go here e.g. Sequence, LiftM, etc.
    }

    public static class Reader<TR>
    {
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
