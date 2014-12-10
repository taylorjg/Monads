using System;
using MonadLib.Registries;

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

        public State<TS, TS2> Gets<TS2>(Func<TS, TS2> f)
        {
            return State<TS>.Get().Bind(s => State<TS>.Return(f(s)));
        }

        public MonadAdapter<TS> GetMonadAdapter()
        {
            return MonadAdapterRegistry.Get<TS>(typeof(State<,>));
        }
    }

    public static partial class State
    {
    }

    public static class State<TS>
    {
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
            return Get().Bind(s => Put(f(s)));
        }

        public static State<TS, TA> Return<TA>(TA a)
        {
            return new State<TS, TA>(s => Tuple.Create(a, s));
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
