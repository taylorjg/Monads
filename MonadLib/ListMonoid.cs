using System.Collections.Generic;
using System.Linq;
using MonadLib.Registries;

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

        public MonoidAdapter<TA> GetMonoidAdapter()
        {
            return MonoidAdapterRegistry.Get<TA>(typeof(ListMonoid<>));
        }
    }

    public static class ListMonoid
    {
        public static ListMonoid<TA> MEmpty<TA>()
        {
            return (ListMonoid<TA>)GetMonoidAdapter<TA>().MEmpty;
        }

        public static ListMonoid<TA> MAppend<TA>(this ListMonoid<TA> a1, ListMonoid<TA> a2)
        {
            return (ListMonoid<TA>)GetMonoidAdapter<TA>().MAppend(a1, a2);
        }

        public static ListMonoid<TA> MConcat<TA>(IEnumerable<ListMonoid<TA>> @as)
        {
            return (ListMonoid<TA>)GetMonoidAdapter<TA>().MConcat(@as);
        }

        public static ListMonoid<TA> MConcat<TA>(params ListMonoid<TA>[] @as)
        {
            return MConcat(@as.AsEnumerable());
        }

        private static ListMonoidAdapter<TA> GetMonoidAdapter<TA>()
        {
            return (ListMonoidAdapter<TA>)MonoidAdapterRegistry.Get<TA>(typeof(ListMonoid<>));
        }
    }

    internal class ListMonoidAdapter<TA> : MonoidAdapter<TA>
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
