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
        public abstract IFunctor<TResult> FMap<TA, TResult>(Func<TA, TResult> f, IFunctor<TA> fa);
    }
}
