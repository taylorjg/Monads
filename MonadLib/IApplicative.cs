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
        public abstract IApplicative<TB> Apply<TA, TB>(IApplicative<Func<TA, TB>> ff, IApplicative<TA> fa);
    }
}
