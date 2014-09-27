using System;

namespace MonadLib
{
    public interface IMonadPlus<TA> : IMonad<TA>
    {
        MonadPlusAdapter<TA> GetMonadPlusAdapter();
    }

    public abstract class MonadPlusAdapter<TA> : MonadAdapter
    {
        public abstract IMonadPlus<TA> MZero { get; }
        public abstract IMonadPlus<TA> MPlus(IMonadPlus<TA> xs, IMonadPlus<TA> ys);
    }

    internal static class MonadPlusCombinators
    {
        public static IMonadPlus<TA> MFilter<TA>(Func<TA, bool> p, IMonadPlus<TA> ma)
        {
            var monadPlusAdapter = ma.GetMonadPlusAdapter();
            return (IMonadPlus<TA>)monadPlusAdapter.Bind(
                ma, a => p(a) ? monadPlusAdapter.Return(a) : monadPlusAdapter.MZero);
        }
    }
}
