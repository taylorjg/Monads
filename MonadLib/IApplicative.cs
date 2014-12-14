using System;

namespace MonadLib
{
    // ReSharper disable UnusedTypeParameter
    public interface IApplicative<TA> : IFunctor<TA>
    {
        ApplicativeAdapter GetApplicativeAdapter();
    }
    // ReSharper restore UnusedTypeParameter

    public abstract class ApplicativeAdapter : FunctorAdapter
    {
        public abstract IApplicative<TA> Pure<TA>(TA a);
        public abstract IApplicative<TResult> Apply<TA, TResult>(IApplicative<Func<TA, TResult>> ff, IApplicative<TA> fa);
    }
}
