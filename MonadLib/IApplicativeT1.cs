using System;

namespace MonadLib
{
    // ReSharper disable UnusedTypeParameter
    public interface IApplicative<T1, TA> : IFunctor<T1, TA>
    {
        ApplicativeAdapter<T1> GetApplicativeAdapter();
    }
    // ReSharper restore UnusedTypeParameter

    public abstract class ApplicativeAdapter<T1> : FunctorAdapter<T1>
    {
        public abstract IApplicative<T1, TA> Pure<TA>(TA a);
        public abstract IApplicative<T1, TResult> Apply<TA, TResult>(IApplicative<T1, Func<TA, TResult>> ff, IApplicative<T1, TA> fa);
    }
}
