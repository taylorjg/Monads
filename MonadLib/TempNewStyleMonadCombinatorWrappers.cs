using System;
using System.Collections.Generic;
using System.Linq;

namespace MonadLib
{
    public static partial class Maybe
    {
        public static Maybe<IEnumerable<TA>> Sequence<TA>(IEnumerable<Maybe<TA>> ms)
        {
            return MonadCombinators.Sequence<Maybe<IEnumerable<TA>>, TA>(ms);
        }

        public static Maybe<TA> Join<TA>(Maybe<Maybe<TA>> mma)
        {
            return MonadCombinators.Join<Maybe<Maybe<TA>>, Maybe<TA>, TA>(mma);
        }

        /* MonadPlus combinators */

        public static Maybe<TA> MZero<TA>()
        {
            var monadPlusAdapter = MonadPlusAdapterRegistry.Get<TA>(typeof(Maybe<TA>));
            return (Maybe<TA>)monadPlusAdapter.MZero;
        }

        public static Maybe<TA> MPlus<TA>(this Maybe<TA> xs, Maybe<TA> ys)
        {
            var monadPlusAdapter = MonadPlusAdapterRegistry.Get<TA>(typeof(Maybe<TA>));
            return (Maybe<TA>)monadPlusAdapter.MPlus(xs, ys);
        }

        public static Maybe<TA> MFilter<TA>(Func<TA, bool> p, Maybe<TA> ma)
        {
            return MonadPlusCombinators.MFilter<Maybe<TA>, TA>(p, ma);
        }

        public static Maybe<TA> MFilter<TA>(this Maybe<TA> ma, Func<TA, bool> p)
        {
            return MonadPlusCombinators.MFilter<Maybe<TA>, TA>(p, ma);
        }

        public static Maybe<TA> MSum<TA>(IEnumerable<Maybe<TA>> ms)
        {
            return MonadPlusCombinators.MSum<Maybe<TA>, TA>(ms);
        }

        public static Maybe<TA> MSum<TA>(params Maybe<TA>[] ms)
        {
            return MSum(ms.AsEnumerable());
        }

        public static Maybe<Unit> Guard(bool b)
        {
            return MonadPlusCombinators.Guard<Maybe<Unit>>(b);
        }
    }
}
