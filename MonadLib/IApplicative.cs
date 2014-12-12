using System;

namespace MonadLib
{
    // ReSharper disable UnusedTypeParameter
    public interface IApplicative<TA>
    {
        ApplicativeAdapter GetApplicativeAdapter();
    }
    // ReSharper restore UnusedTypeParameter

    public abstract class ApplicativeAdapter
    {
        public abstract IApplicative<TA> Pure<TA>(TA a);
        public abstract IApplicative<TResult> Apply<TA, TResult>(IApplicative<Func<TA, TResult>> ff, IApplicative<TA> fa);
        public abstract IApplicative<Func<TB, TResult>> Apply<TA, TB, TResult>(IApplicative<Func<TA, TB, TResult>> ff, IApplicative<TA> fa);
        public abstract IApplicative<Func<TB, TC, TResult>> Apply<TA, TB, TC, TResult>(IApplicative<Func<TA, TB, TC, TResult>> ff, IApplicative<TA> fa);
        public abstract IApplicative<Func<TB, TC, TD, TResult>> Apply<TA, TB, TC, TD, TResult>(IApplicative<Func<TA, TB, TC, TD, TResult>> ff, IApplicative<TA> fa);
        public abstract IApplicative<Func<TB, TC, TD, TE, TResult>> Apply<TA, TB, TC, TD, TE, TResult>(IApplicative<Func<TA, TB, TC, TD, TE, TResult>> ff, IApplicative<TA> fa);
    }
}
