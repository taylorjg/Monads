using System;

namespace MonadLib
{
    // ReSharper disable UnusedTypeParameter
    public interface IFunctor<TA>
    {
        FunctorAdapter GetFunctorAdapter();
    }
    // ReSharper restore UnusedTypeParameter

    public abstract class FunctorAdapter
    {
        public abstract IFunctor<TB> FMap<TA, TB>(Func<TA, TB> f, IFunctor<TA> fa);
    }
}
