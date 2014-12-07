using MonadLib.Registries;

namespace MonadLib
{
    public static partial class MonadCombinators
    {
        public static TMonad Join<TMonadMonad, TMonad, TA>(TMonadMonad mma)
            where TMonadMonad : IMonad<TMonad>
            where TMonad : IMonad<TA>
        {
            var monadAdapter = MonadAdapterRegistry.Get(typeof(TMonad));
            return (TMonad)monadAdapter.Bind(mma, ma => ma);
        }
    }
}
