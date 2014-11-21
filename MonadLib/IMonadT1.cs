using System;

namespace MonadLib
{
    // ReSharper disable UnusedTypeParameter
    public interface IMonad<T1, TA>
    {
        MonadAdapter<T1> GetMonadAdapter();
    }
    // ReSharper restore UnusedTypeParameter

    public abstract class MonadAdapter<T1>
    {
        public abstract IMonad<T1, TA> Return<TA>(TA a);
        public abstract IMonad<T1, TB> Bind<TA, TB>(IMonad<T1, TA> ma, Func<TA, IMonad<T1, TB>> f);

        public virtual IMonad<T1, TB> BindIgnoringLeft<TA, TB>(IMonad<T1, TA> ma, IMonad<T1, TB> mb)
        {
            return Bind(ma, _ => mb);
        }
    }
}
