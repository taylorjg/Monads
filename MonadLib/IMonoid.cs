using System.Collections.Generic;
using Flinq;

namespace MonadLib
{
    public interface IMonoid<TA>
    {
        MonoidAdapter<TA> GetMonoidAdapter();
    }

    public abstract class MonoidAdapter<TA>
    {
        public abstract IMonoid<TA> MEmpty { get; }
        public abstract IMonoid<TA> MAppend(IMonoid<TA> a1, IMonoid<TA> a2);

        public virtual IMonoid<TA> MConcat(IEnumerable<IMonoid<TA>> @as)
        {
            return @as.FoldRight(MEmpty, MAppend);
        }
    }
}
