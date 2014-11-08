﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace StateBinTreeBuild
{
    public static class NonMonadicBuilder2
    {
        public static BinTree Build<TA>(IImmutableList<TA> xs)
        {
            return Build2(xs.Count, Tuple.Create(xs as IReadOnlyList<TA>, 0)).Item1;
        }

        private static Tuple<BinTree, Tuple<IReadOnlyList<TA>, int>> Build2<TA>(int n, Tuple<IReadOnlyList<TA>, int> state)
        {
            if (n == 1)
            {
                var xs = state.Item1;
                var index = state.Item2;
                var head = xs[index];
                BinTree leaf = new Leaf<TA>(head);
                return Tuple.Create(leaf, Tuple.Create(xs, index + 1));
            }

            var m = n / 2;
            var tuple1 = Build2(m, state);
            var u = tuple1.Item1;
            var state1 = tuple1.Item2;
            var tuple2 = Build2(n - m, state1);
            var v = tuple2.Item1;
            var state2 = tuple2.Item2;
            BinTree fork = new Fork(u, v);
            return Tuple.Create(fork, state2);
        }
    }
}
