using System.Collections.Generic;
using System.Linq;

namespace MonadLib
{
    public class ListMonoid<TA> : IMonoid<TA>
    {
        public ListMonoid()
            : this(Enumerable.Empty<TA>())
        {
        }

        public ListMonoid(IEnumerable<TA> collection)
        {
            _list = new List<TA>(collection);
        }

        private readonly List<TA> _list = new List<TA> ();

        private static MonoidAdapter<TA> _monoidAdapter;

        public MonoidAdapter<TA> GetMonoidAdapter()
        {
            return _monoidAdapter ?? (_monoidAdapter = new ListMonoidAdapter<TA>());
        }

        public List<TA> List { get { return _list; } } 
    }

    public class ListMonoidAdapter<TA> : MonoidAdapter<TA>
    {
        public override IMonoid<TA> MEmpty
        {
            get
            {
                return new ListMonoid<TA>();
            }
        }

        public override IMonoid<TA> MAppend(IMonoid<TA> a1, IMonoid<TA> a2)
        {
            var listMonoid1 = (ListMonoid<TA>)a1;
            var listMonoid2 = (ListMonoid<TA>)a2;
            return new ListMonoid<TA>(listMonoid1.List.Concat(listMonoid2.List));
        }
    }
}
