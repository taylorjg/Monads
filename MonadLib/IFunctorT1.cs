using System;

namespace MonadLib
{
    // ReSharper disable UnusedTypeParameter
    public interface IFunctor<T1, TA>
    {
        FunctorAdapter<T1> GetFunctorAdapter();
    }
    // ReSharper restore UnusedTypeParameter

    public abstract class FunctorAdapter<T1>
    {
        public abstract IFunctor<T1, TB> FMap<TA, TB>(Func<TA, TB> f, IFunctor<T1, TA> fa);
    }
}
