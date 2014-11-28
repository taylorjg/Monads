using System;

namespace MonadLib
{
    public class Writer<TMonoid, TW, TA> : IMonad<TMonoid, TW, TA> where TMonoid : IMonoid<TW>
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

        public Writer<TMonoid, TW, Tuple<TA, TMonoid>> Listen()
        {
            return new Writer<TMonoid, TW, Tuple<TA, TMonoid>>(Tuple.Create(_a, _w), _w);
        }

        public Writer<TMonoid, TW, Tuple<TA, TW2>> Listens<TW2>(Func<TMonoid, TW2> f)
        {
            return Listen().Bind(tuple =>
                {
                    var a = tuple.Item1;
                    var w = tuple.Item2;
                    return Writer<TMonoid, TW>.Return(Tuple.Create(a, f(w)));
                });
        }

        private WriterMonadAdapter<TMonoid, TW> _monadAdapter;

        public MonadAdapter<TMonoid, TW> GetMonadAdapter()
        {
            return _monadAdapter ?? (_monadAdapter = new WriterMonadAdapter<TMonoid, TW>());
        }
    }

    public static partial class Writer
    {
    }

    public static class Writer<TMonoid, TW> where TMonoid : IMonoid<TW>
    {
        public static Writer<TMonoid, TW, Unit> Tell(TMonoid s)
        {
            return new Writer<TMonoid, TW, Unit>(new Unit(), s);
        }

        public static Writer<TMonoid, TW, TA> Pass<TA>(Writer<TMonoid, TW, Tuple<TA, Func<TMonoid, TMonoid>>> m)
        {
            var tuple = m.RunWriter;
            var a = tuple.Item1.Item1;
            var f = tuple.Item1.Item2;
            var w = tuple.Item2;
            return new Writer<TMonoid, TW, TA>(a, f(w));
        }

        public static Writer<TMonoid, TW, TA> Censor<TA>(Func<TMonoid, TMonoid> f, Writer<TMonoid, TW, TA> m)
        {
            return Pass(m.Bind(a => Return(Tuple.Create(a, f))));
        }

        public static Writer<TMonoid, TW, TA> Return<TA>(TA a)
        {
            var monoidAdapter = MonoidAdapterRegistry.Get<TW>(typeof(TMonoid));
            var w = (TMonoid)monoidAdapter.MEmpty;
            return new Writer<TMonoid, TW, TA>(a, w);
        }
    }

    internal class WriterMonadAdapter<TMonoid, TW> : MonadAdapter<TMonoid, TW> where TMonoid : IMonoid<TW>
    {
        public override IMonad<TMonoid, TW, TA> Return<TA>(TA a)
        {
            return Writer<TMonoid, TW>.Return(a);
        }

        public override IMonad<TMonoid, TW, TB> Bind<TA, TB>(IMonad<TMonoid, TW, TA> ma, Func<TA, IMonad<TMonoid, TW, TB>> f)
        {
            var writerA = (Writer<TMonoid, TW, TA>)ma;
            var t1 = writerA.RunWriter;
            var a = t1.Item1;
            var w1 = t1.Item2;
            var mb = f(a);
            var writerB = (Writer<TMonoid, TW, TB>)mb;
            var t2 = writerB.RunWriter;
            var b = t2.Item1;
            var w2 = t2.Item2;
            var monoidAdapter = w1.GetMonoidAdapter();
            var w = (TMonoid)monoidAdapter.MAppend(w1, w2);
            return new Writer<TMonoid, TW, TB>(b, w);
        }
    }
}
