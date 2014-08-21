using System;
using System.Collections.Generic;
using System.Linq;

namespace Monads
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TA> Unit<TA>(TA a)
        {
            yield return a;
        }

        public static IEnumerable<TB> Bind<TA, TB>(this IEnumerable<TA> ma, Func<TA, IEnumerable<TB>> f)
        {
            return ma.SelectMany(f);
        }
    }
}
