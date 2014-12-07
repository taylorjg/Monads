using System.Collections.Generic;

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
    }
}
