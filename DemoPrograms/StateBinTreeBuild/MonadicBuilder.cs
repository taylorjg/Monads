using System.Collections.Generic;
using System.Linq;
using MonadLib;

namespace StateBinTreeBuild
{
    public static class MonadicBuilder
    {
        public static BinTree Build<TA>(IReadOnlyList<TA> xs)
        {
            return Build2<TA>(xs.Count).EvalState(xs);
        }

        private static State<IReadOnlyList<TA>, BinTree> Build2<TA>(int n)
        {
            if (n == 1)
            {
                return State<IReadOnlyList<TA>>.Get().Bind(xs =>
                    {
                        var head = xs.First();
                        var tail = (IReadOnlyList<TA>)xs.Skip(1).ToList();
                        return State<IReadOnlyList<TA>>.Put(tail).Bind(_ =>
                            {
                                BinTree leaf = new Leaf<TA>(head);
                                return State<IReadOnlyList<TA>>.Return(leaf);
                            });
                    });
            }

            var m = n / 2;
            return Build2<TA>(m).Bind(
                u => Build2<TA>(n - m).Bind(
                    v =>
                        {
                            BinTree fork = new Fork(u, v);
                            return State<IReadOnlyList<TA>>.Return(fork);
                        }));
        }
    }
}
