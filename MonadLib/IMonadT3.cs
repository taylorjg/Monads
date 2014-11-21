using System;

namespace MonadLib
{
    // ReSharper disable UnusedTypeParameter
    public interface IMonad<T1, T2, T3, TA>
    {
        MonadAdapter<T1, T2, T3> GetMonadAdapter();
    }
    // ReSharper restore UnusedTypeParameter

    public abstract class MonadAdapter<T1, T2, T3>
    {
        public abstract IMonad<T1, T2, T3, TA> Return<TA>(TA a);
        public abstract IMonad<T1, T2, T3, TB> Bind<TA, TB>(IMonad<T1, T2, T3, TA> ma, Func<TA, IMonad<T1, T2, T3, TB>> f);

        public virtual IMonad<T1, T2, T3, TB> BindIgnoringLeft<TA, TB>(IMonad<T1, T2, T3, TA> ma, IMonad<T1, T2, T3, TB> mb)
        {
            return Bind(ma, _ => mb);
        }
    }
}
