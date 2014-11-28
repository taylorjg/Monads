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

        public ListMonoid(params TA[] collection)
            : this(collection.AsEnumerable())
        {
        }

        public IReadOnlyList<TA> List { get { return _list; } }

        private readonly List<TA> _list;

        private static MonoidAdapter<TA> _monoidAdapter;

        public MonoidAdapter<TA> GetMonoidAdapter()
        {
            return _monoidAdapter ?? (_monoidAdapter = new ListMonoidAdapter<TA>());
        }
    }

    public static class ListMonoid
    {
        public static ListMonoid<TA> MEmpty<TA>()
        {
            var listMonoidAdapter = new ListMonoidAdapter<TA>();
            return (ListMonoid<TA>)listMonoidAdapter.MEmpty;
        }

        public static ListMonoid<TA> MAppend<TA>(this ListMonoid<TA> a1, ListMonoid<TA> a2)
        {
            var listMonoidAdapter = new ListMonoidAdapter<TA>();
            return (ListMonoid<TA>)listMonoidAdapter.MAppend(a1, a2);
        }

        public static ListMonoid<TA> MConcat<TA>(IEnumerable<ListMonoid<TA>> @as)
        {
            var listMonoidAdapter = new ListMonoidAdapter<TA>();
            return (ListMonoid<TA>)listMonoidAdapter.MConcat(@as);
        }

        public static ListMonoid<TA> MConcat<TA>(params ListMonoid<TA>[] @as)
        {
            return MConcat(@as.AsEnumerable());
        }
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
