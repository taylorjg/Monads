using System;

namespace MonadLib
{
    internal sealed class State<TS, TA> : IMonad<TS, TA>
    {
        internal State(Func<TS, Tuple<TA, TS>> run)
        {
            Run = run;
        }

        public Func<TS, Tuple<TA, TS>> Run { get; private set; }

        // get :: State s s
        // get s = (s,s)

        // put :: s -> State s ()
        // put x s = ((),x)

        // modify :: (s -> s) -> State s ()
        // modify f = do { x <- get; put (f x) }

        // evalState :: State s a -> s -> a
        // evalState act = fst . runState act
 
        // execState :: State s a -> s -> s
        // execState act = snd . runState act

        private MonadAdapter<TS> _monadAdapter;

        public MonadAdapter<TS> GetMonadAdapter()
        {
            return _monadAdapter ?? (_monadAdapter = new StateMonadAdapter<TS>());
        }
    }

    internal class StateMonadAdapter<TS> : MonadAdapter<TS>
    {
        public override IMonad<TS, TA> Return<TA>(TA a)
        {
            return new State<TS, TA>(s => Tuple.Create(a, s));
        }

        public override IMonad<TS, TB> Bind<TA, TB>(IMonad<TS, TA> ma, Func<TA, IMonad<TS, TB>> f)
        {
            return new State<TS, TB>(s =>
                {
                    var stateA = (State<TS, TA>) ma;
                    var pair = stateA.Run(s);
                    var a = pair.Item1;
                    var stick = pair.Item2;
                    var mb = f(a);
                    var stateB = (State<TS, TB>) mb;
                    return stateB.Run(stick);
                });
        }
    }
}
