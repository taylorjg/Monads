using System;
using System.Collections.Generic;
using MonadLib.Registries;

namespace MonadLib
{
    public static class MonadPlusAgnosticFunctions
    {
        public static TMonadPlus LookupM<TMonadPlus, TA, TB>(TA k, IEnumerable<Tuple<TA, TB>> alist)
            where TMonadPlus : IMonadPlus<TB>
        {
            var monadPlusAdapter = MonadPlusAdapterRegistry.Get<TB>(typeof (TMonadPlus));
            return LookupMInternal<TMonadPlus, TA, TB>(k, alist, monadPlusAdapter);
        }

        private static TMonadPlus LookupMInternal<TMonadPlus, TA, TB>(TA k, IEnumerable<Tuple<TA, TB>> alist, MonadPlusAdapter<TB> monadPlusAdapter)
            where TMonadPlus : IMonadPlus<TB>
        {
            return (TMonadPlus) alist.HeadAndTail().Match(
                tuple =>
                    {
                        var xy = tuple.Item1;
                        var xys = tuple.Item2;
                        var x = xy.Item1;
                        var y = xy.Item2;

                        return (EqualityComparer<TA>.Default.Equals(x, k))
                                   ? monadPlusAdapter.MPlus((TMonadPlus) monadPlusAdapter.Return(y), LookupMInternal<TMonadPlus, TA, TB>(k, xys, monadPlusAdapter))
                                   : LookupMInternal<TMonadPlus, TA, TB>(k, xys, monadPlusAdapter);
                    },
                () => monadPlusAdapter.MZero);
        }
    }
}
