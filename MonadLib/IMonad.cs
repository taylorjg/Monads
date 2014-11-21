using System;

namespace MonadLib
{
    // ReSharper disable UnusedTypeParameter
    public interface IMonad<TA>
    {
        MonadAdapter GetMonadAdapter();
    }
    // ReSharper restore UnusedTypeParameter

    public abstract class MonadAdapter
    {
        public abstract IMonad<TA> Return<TA>(TA a);
        public abstract IMonad<TB> Bind<TA, TB>(IMonad<TA> ma, Func<TA, IMonad<TB>> f);

        public virtual IMonad<TB> BindIgnoringLeft<TA, TB>(IMonad<TA> ma, IMonad<TB> mb)
        {
            return Bind(ma, _ => mb);
        }
    }
}
