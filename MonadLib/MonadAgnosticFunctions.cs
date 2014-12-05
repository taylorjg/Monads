using System;
using System.Collections.Generic;

namespace MonadLib
{
    public static class MonadAgnosticFunctions
    {
        public static TMonadPlus LookupM<TMonadPlus, TA, TB>(TA k, IEnumerable<Tuple<TA, TB>> alist)
            where TMonadPlus : IMonadPlus<TB>
        {
            var monadPlusAdapter = MonadPlusAdapterRegistry.Get<TB>(typeof (TMonadPlus));

            var result = alist.HeadAndTail().Match(
                tuple =>
                    {
                        var xy = tuple.Item1;
                        var xys = tuple.Item2;
                        var x = xy.Item1;
                        var y = xy.Item2;

                        return (EqualityComparer<TA>.Default.Equals(x, k))
                                   ? monadPlusAdapter.MPlus((TMonadPlus) monadPlusAdapter.Return(y), LookupM<TMonadPlus, TA, TB>(k, xys))
                                   : LookupM<TMonadPlus, TA, TB>(k, xys);
                    },
                () => monadPlusAdapter.MZero);

            return (TMonadPlus) result;
        }
    }
}
