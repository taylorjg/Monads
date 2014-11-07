using System;
using System.Collections.Immutable;
using System.Linq;

namespace BinTreeBuild
{
    public static class NonMonadicBuilder
    {
        public static BinTree Build<TA>(IImmutableList<TA> xs)
        {
            return Build2(xs.Count, xs).Item1;
        }

        private static Tuple<BinTree, IImmutableList<TA>> Build2<TA>(int n, IImmutableList<TA> xs)
        {
            if (n == 1)
            {
                var head = xs.First();
                var tail = (IImmutableList<TA>) ImmutableList.CreateRange(xs.Skip(1));
                BinTree leaf = new Leaf<TA>(head);
                return Tuple.Create(leaf, tail);
            }

            var m = n / 2;
            var t1 = Build2(m, xs);
            var u = t1.Item1;
            var xs1 = t1.Item2;
            var t2 = Build2(n - m, xs1);
            var v = t2.Item1;
            var xs2 = t2.Item2;
            BinTree fork = new Fork(u, v);
            return Tuple.Create(fork, xs2);
        }
    }
}
