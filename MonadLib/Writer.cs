using System;

namespace MonadLib
{
    public class Writer<TMonoid, TMonoidAdapter, TW, TA> : IMonad<TMonoid, TMonoidAdapter, TW, TA>
        where TMonoid : IMonoid<TW>
        where TMonoidAdapter : MonoidAdapter<TW>, new()
    {
        private readonly TA _a;
        private readonly TMonoid _w;

        internal Writer(TA a, TMonoid w)
        {
            _a = a;
            _w = w;
        }

        public Tuple<TA, TMonoid> RunWriter
        {
            get { return Tuple.Create(_a, _w); }
        }

        private WriterMonadAdapter<TMonoid, TMonoidAdapter, TW> _monadAdapter;

        public MonadAdapter<TMonoid, TMonoidAdapter, TW> GetMonadAdapter()
        {
            return _monadAdapter ?? (_monadAdapter = new WriterMonadAdapter<TMonoid, TMonoidAdapter, TW>());
        }
    }

    public static partial class Writer
    {
    }

    public static class Writer<TMonoid, TMonoidAdapter, TW>
        where TMonoid : IMonoid<TW>
        where TMonoidAdapter : MonoidAdapter<TW>, new()
    {
        public static Writer<TMonoid, TMonoidAdapter, TW, Unit> Tell(TMonoid s)
        {
            return new Writer<TMonoid, TMonoidAdapter, TW, Unit>(new Unit(), s);
        }

        public static Writer<TMonoid, TMonoidAdapter, TW, TA> Return<TA>(TA a)
        {
            var w = (TMonoid) new TMonoidAdapter().MEmpty;
            return new Writer<TMonoid, TMonoidAdapter, TW, TA>(a, w);
        }
    }

    internal class WriterMonadAdapter<TMonoid, TMonoidAdapter, TW> : MonadAdapter<TMonoid, TMonoidAdapter, TW>
        where TMonoid : IMonoid<TW>
        where TMonoidAdapter : MonoidAdapter<TW>, new()
    {
        public override IMonad<TMonoid, TMonoidAdapter, TW, TA> Return<TA>(TA a)
        {
            return Writer<TMonoid, TMonoidAdapter, TW>.Return(a);
        }

        public override IMonad<TMonoid, TMonoidAdapter, TW, TB> Bind<TA, TB>(IMonad<TMonoid, TMonoidAdapter, TW, TA> ma, Func<TA, IMonad<TMonoid, TMonoidAdapter, TW, TB>> f)
        {
            var writerA = (Writer<TMonoid, TMonoidAdapter, TW, TA>)ma;
            var t1 = writerA.RunWriter;
            var a = t1.Item1;
            var w1 = t1.Item2;
            var mb = f(a);
            var writerB = (Writer<TMonoid, TMonoidAdapter, TW, TB>)mb;
            var t2 = writerB.RunWriter;
            var b = t2.Item1;
            var w2 = t2.Item2;
            var monoidAdapter = w1.GetMonoidAdapter();
            var w = (TMonoid)monoidAdapter.MAppend(w1, w2);
            return new Writer<TMonoid, TMonoidAdapter, TW, TB>(b, w);
        }
    }
}
