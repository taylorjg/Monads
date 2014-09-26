using System;
using System.Collections.Generic;

namespace MonadLib
{
    public sealed class State<TS, TA> : IMonad<TS, TA>
    {
        internal State(Func<TS, Tuple<TA, TS>> runState)
        {
            RunState = runState;
        }

        public Func<TS, Tuple<TA, TS>> RunState { get; private set; }

        public TA EvalState(TS s)
        {
            return RunState(s).Item1;
        }
 
        public TS ExecState(TS s)
        {
            return RunState(s).Item2;
        }

        private MonadAdapter<TS> _monadAdapter;

        public MonadAdapter<TS> GetMonadAdapter()
        {
            return _monadAdapter ?? (_monadAdapter = new StateMonadAdapter<TS>());
        }
    }

    public static class State
    {
        public static State<TS, TB> Bind<TS, TA, TB>(this State<TS, TA> ma, Func<TA, State<TS, TB>> f)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (State<TS, TB>)monadAdapter.Bind(ma, f);
        }

        public static State<TS, TB> LiftM<TS, TA, TB>(this State<TS, TA> ma, Func<TA, TB> f)
        {
            return (State<TS, TB>)MonadCombinators<TS>.LiftM(f, ma);
        }

        public static State<TS, TB> LiftM<TS, TA, TB>(Func<TA, TB> f, State<TS, TA> ma)
        {
            return (State<TS, TB>)MonadCombinators<TS>.LiftM(f, ma);
        }

        public static State<TS, TC> LiftM2<TS, TA, TB, TC>(this State<TS, TA> ma, State<TS, TB> mb, Func<TA, TB, TC> f)
        {
            return (State<TS, TC>)MonadCombinators<TS>.LiftM2(f, ma, mb);
        }

        public static State<TS, TC> LiftM2<TS, TA, TB, TC>(Func<TA, TB, TC> f, State<TS, TA> ma, State<TS, TB> mb)
        {
            return (State<TS, TC>)MonadCombinators<TS>.LiftM2(f, ma, mb);
        }

        public static State<TS, TD> LiftM3<TS, TA, TB, TC, TD>(this State<TS, TA> ma, State<TS, TB> mb, State<TS, TC> mc, Func<TA, TB, TC, TD> f)
        {
            return (State<TS, TD>)MonadCombinators<TS>.LiftM3(f, ma, mb, mc);
        }

        public static State<TS, TD> LiftM3<TS, TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, State<TS, TA> ma, State<TS, TB> mb, State<TS, TC> mc)
        {
            return (State<TS, TD>)MonadCombinators<TS>.LiftM3(f, ma, mb, mc);
        }

        public static State<TS, TE> LiftM4<TS, TA, TB, TC, TD, TE>(this State<TS, TA> ma, State<TS, TB> mb, State<TS, TC> mc, State<TS, TD> md, Func<TA, TB, TC, TD, TE> f)
        {
            return (State<TS, TE>)MonadCombinators<TS>.LiftM4(f, ma, mb, mc, md);
        }

        public static State<TS, TE> LiftM4<TS, TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, State<TS, TA> ma, State<TS, TB> mb, State<TS, TC> mc, State<TS, TD> md)
        {
            return (State<TS, TE>)MonadCombinators<TS>.LiftM4(f, ma, mb, mc, md);
        }

        public static State<TS, TF> LiftM5<TS, TA, TB, TC, TD, TE, TF>(this State<TS, TA> ma, State<TS, TB> mb, State<TS, TC> mc, State<TS, TD> md, State<TS, TE> me, Func<TA, TB, TC, TD, TE, TF> f)
        {
            return (State<TS, TF>)MonadCombinators<TS>.LiftM5(f, ma, mb, mc, md, me);
        }

        public static State<TS, TF> LiftM5<TS, TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, State<TS, TA> ma, State<TS, TB> mb, State<TS, TC> mc, State<TS, TD> md, State<TS, TE> me)
        {
            return (State<TS, TF>)MonadCombinators<TS>.LiftM5(f, ma, mb, mc, md, me);
        }

        public static State<TS, IEnumerable<TA>> Sequence<TS, TA>(IEnumerable<State<TS, TA>> ms)
        {
            return (State<TS, IEnumerable<TA>>)MonadCombinators<TS>.SequenceInternal(ms, new EitherMonadAdapter<TS>());
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> Sequence_<TS, TA>(IEnumerable<State<TS, TA>> ms)
        // ReSharper restore InconsistentNaming
        {
            return (State<TS, Unit>)MonadCombinators<TS>.SequenceInternal_(ms, new EitherMonadAdapter<TS>());
        }

        public static State<TS, IEnumerable<TB>> MapM<TS, TA, TB>(Func<TA, State<TS, TB>> f, IEnumerable<TA> @as)
        {
            return (State<TS, IEnumerable<TB>>)MonadCombinators<TS>.MapMInternal(f, @as, new EitherMonadAdapter<TS>());
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> MapM_<TS, TA, TB>(Func<TA, State<TS, TB>> f, IEnumerable<TA> @as)
        // ReSharper restore InconsistentNaming
        {
            return (State<TS, Unit>)MonadCombinators<TS>.MapMInternal_(f, @as, new EitherMonadAdapter<TS>());
        }

        public static State<TS, IEnumerable<TB>> ForM<TS, TA, TB>(IEnumerable<TA> @as, Func<TA, State<TS, TB>> f)
        {
            return (State<TS, IEnumerable<TB>>)MonadCombinators<TS>.MapMInternal(f, @as, new EitherMonadAdapter<TS>());
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> ForM_<TS, TA, TB>(IEnumerable<TA> @as, Func<TA, State<TS, TB>> f)
        // ReSharper restore InconsistentNaming
        {
            return (State<TS, Unit>)MonadCombinators<TS>.MapMInternal_(f, @as, new EitherMonadAdapter<TS>());
        }

        public static State<TS, IEnumerable<TA>> ReplicateM<TS, TA>(int n, State<TS, TA> ma)
        {
            return (State<TS, IEnumerable<TA>>)MonadCombinators<TS>.ReplicateM(n, ma);
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> ReplicateM_<TS, TA>(int n, State<TS, TA> ma)
        // ReSharper restore InconsistentNaming
        {
            return (State<TS, Unit>)MonadCombinators<TS>.ReplicateM_(n, ma);
        }

        public static State<TS, TA> Join<TS, TA>(State<TS, State<TS, TA>> mma)
        {
            // Ideally, we would like to use MonadCombinators<TS>.Join(mma) but there
            // is a casting issue that I have not figured out how to fix.
            var monadAdapter = mma.GetMonadAdapter();
            return (State<TS, TA>)monadAdapter.Bind(mma, MonadHelpers.Identity);
        }

        public static State<TS, TA> FoldM<TS, TA, TB>(Func<TA, TB, State<TS, TA>> f, TA a, IEnumerable<TB> bs)
        {
            return (State<TS, TA>)MonadCombinators<TS>.FoldMInternal(f, a, bs, new StateMonadAdapter<TS>());
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> FoldM_<TS, TA, TB>(Func<TA, TB, State<TS, TA>> f, TA a, IEnumerable<TB> bs)
        // ReSharper restore InconsistentNaming
        {
            return (State<TS, Unit>)MonadCombinators<TS>.FoldMInternal_(f, a, bs, new StateMonadAdapter<TS>());
        }

        public static State<TS, IEnumerable<TC>> ZipWithM<TS, TA, TB, TC>(Func<TA, TB, State<TS, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs)
        {
            return (State<TS, IEnumerable<TC>>)MonadCombinators<TS>.ZipWithMInternal(f, @as, bs, new StateMonadAdapter<TS>());
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> ZipWithM_<TS, TA, TB, TC>(Func<TA, TB, State<TS, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs)
        // ReSharper restore InconsistentNaming
        {
            return (State<TS, Unit>)MonadCombinators<TS>.ZipWithMInternal_(f, @as, bs, new StateMonadAdapter<TS>());
        }

        public static State<TS, IEnumerable<TA>> FilterM<TS, TA>(Func<TA, State<TS, bool>> p, IEnumerable<TA> @as)
        {
            return (State<TS, IEnumerable<TA>>)MonadCombinators<TS>.FilterMInternal(p, @as, new StateMonadAdapter<TS>());
        }

        public static State<TS, Unit> When<TS>(bool b, State<TS, Unit> m)
        {
            return (State<TS, Unit>)MonadCombinators<TS>.When(b, m);
        }

        public static State<TS, Unit> Unless<TS>(bool b, State<TS, Unit> m)
        {
            return (State<TS, Unit>)MonadCombinators<TS>.Unless(b, m);
        }

        public static State<TS, TB> Forever<TS, TA, TB>(State<TS, TA> m)
        {
            return (State<TS, TB>)MonadCombinators<TS>.Forever<TA, TB>(m);
        }

        public static State<TS, Unit> Void<TS, TA>(Either<TS, TA> m)
        {
            return (State<TS, Unit>)MonadCombinators<TS>.Void(m);
        }
    }

    public static class State<TS>
    {
        public static State<TS, TA> Return<TA>(TA a)
        {
            return new State<TS, TA>(s => Tuple.Create(a, s));
        }

        public static State<TS, TS> Get()
        {
            return new State<TS, TS>(s => Tuple.Create(s, s));
        }

        public static State<TS, Unit> Put(TS s)
        {
            return new State<TS, Unit>(_ => Tuple.Create(new Unit(), s));
        }

        public static State<TS, Unit> Modify(Func<TS, TS> f)
        {
            return new State<TS, Unit>(s => Get().Bind(a => Put(f(a))).RunState(s));
        }
    }

    internal class StateMonadAdapter<TS> : MonadAdapter<TS>
    {
        public override IMonad<TS, TA> Return<TA>(TA a)
        {
            return State<TS>.Return(a);
        }

        public override IMonad<TS, TB> Bind<TA, TB>(IMonad<TS, TA> ma, Func<TA, IMonad<TS, TB>> f)
        {
            return new State<TS, TB>(s =>
                {
                    var stateA = (State<TS, TA>) ma;
                    var pair = stateA.RunState(s);
                    var a = pair.Item1;
                    var stick = pair.Item2;
                    var mb = f(a);
                    var stateB = (State<TS, TB>) mb;
                    return stateB.RunState(stick);
                });
        }
    }
}
