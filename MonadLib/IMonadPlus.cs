using System;
using System.Collections.Generic;
using Flinq;

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
        public static TMonadPlus MFilter<TMonadPlus, TA>(Func<TA, bool> p, IMonadPlus<TA> ma)
            where TMonadPlus : IMonadPlus<TA>
        {
            var monadPlusAdapter = MonadPlusAdapterRegistry.Get<TA>(typeof(TMonadPlus));
            return (TMonadPlus)monadPlusAdapter.Bind(
                ma, a => p(a) ? monadPlusAdapter.Return(a) : monadPlusAdapter.MZero);
        }

        public static TMonadPlus MSum<TMonadPlus, TA>(IEnumerable<IMonadPlus<TA>> ms)
            where TMonadPlus : IMonadPlus<TA>
        {
            var monadPlusAdapter = MonadPlusAdapterRegistry.Get<TA>(typeof(TMonadPlus));
            return (TMonadPlus)ms.FoldRight(monadPlusAdapter.MZero, monadPlusAdapter.MPlus);
        }

        public static TMonadPlus Guard<TMonadPlus>(bool b)
            where TMonadPlus : IMonadPlus<Unit>
        {
            var monadPlusAdapter = MonadPlusAdapterRegistry.Get<Unit>(typeof(TMonadPlus));
            return (TMonadPlus) (b ? monadPlusAdapter.Return(new Unit()) : monadPlusAdapter.MZero);
        }
    }
}
