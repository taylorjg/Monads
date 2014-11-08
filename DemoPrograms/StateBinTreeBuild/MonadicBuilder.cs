using System.Collections.Immutable;
using System.Linq;
using MonadLib;

namespace StateBinTreeBuild
{
    public static class MonadicBuilder
    {
        public static BinTree Build<TA>(IImmutableList<TA> xs)
        {
            return Build2<TA>(xs.Count).EvalState(xs);
        }

        private static State<IImmutableList<TA>, BinTree> Build2<TA>(int n)
        {
            if (n == 1)
            {
                return State<IImmutableList<TA>>.Get().Bind(xs =>
                    {
                        var head = xs.First();
                        var tail = (IImmutableList<TA>) ImmutableList.CreateRange(xs.Skip(1));
                        return State<IImmutableList<TA>>.Put(tail).Bind(_ =>
                            {
                                BinTree leaf = new Leaf<TA>(head);
                                return State<IImmutableList<TA>>.Return(leaf);
                            });
                    });
            }

            var m = n / 2;
            return Build2<TA>(m).Bind(
                u => Build2<TA>(n - m).Bind(v =>
                    {
                        BinTree fork = new Fork(u, v);
                        return State<IImmutableList<TA>>.Return(fork);
                    }));
        }
    }
}
