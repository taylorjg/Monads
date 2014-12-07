using System.Collections.Generic;
using Flinq;

namespace MonadLib
{
    public static partial class MonadCombinators
    {
        public static TMonad Sequence<TMonad, TA>(IEnumerable<IMonad<TA>> ms)
            where TMonad : IMonad<IEnumerable<TA>>
        {
            var monadAdapter = MonadAdapterRegistry.Get(typeof(TMonad));
            var z = monadAdapter.Return(MonadHelpers.Nil<TA>());
            return (TMonad)ms.FoldRight(
                z, (m, mtick) => monadAdapter.Bind(
                    m, x => monadAdapter.Bind(
                        mtick, xs => monadAdapter.Return(MonadHelpers.Cons(x, xs)))));
        }

        public static TMonad Join<TMonadMonad, TMonad, TA>(TMonadMonad mma)
            where TMonadMonad : IMonad<TMonad>
            where TMonad : IMonad<TA>
        {
            var monadAdapter = MonadAdapterRegistry.Get(typeof(TMonad));
            return (TMonad)monadAdapter.Bind(mma, ma => ma);
        }
    }
}
