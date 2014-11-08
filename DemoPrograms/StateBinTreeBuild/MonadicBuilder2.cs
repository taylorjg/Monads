using System;
using System.Collections.Generic;
using MonadLib;

namespace StateBinTreeBuild
{
    public static class MonadicBuilder2
    {
        public static BinTree Build<TA>(IReadOnlyList<TA> xs)
        {
            return Build2<TA>(xs.Count).EvalState(Tuple.Create(xs, 0));
        }

        private static State<Tuple<IReadOnlyList<TA>, int>, BinTree> Build2<TA>(int n)
        {
            if (n == 1)
            {
                return State<Tuple<IReadOnlyList<TA>, int>>.Get().Bind(
                    t1 =>
                        {
                            var xs = t1.Item1;
                            var index = t1.Item2;
                            var t2 = Tuple.Create(xs, index + 1);
                            var head = xs[index];
                            BinTree leaf = new Leaf<TA>(head);
                            return State<Tuple<IReadOnlyList<TA>, int>>.Put(t2).BindIgnoringLeft(
                                State<Tuple<IReadOnlyList<TA>, int>>.Return(leaf));
                        });
            }

            var m = n / 2;
            return Build2<TA>(m).Bind(
                u => Build2<TA>(n - m).Bind(
                    v =>
                        {
                            BinTree fork = new Fork(u, v);
                            return State<Tuple<IReadOnlyList<TA>, int>>.Return(fork);
                        }));
        }
    }
}
