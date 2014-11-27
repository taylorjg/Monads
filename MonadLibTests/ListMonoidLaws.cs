using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    // ReSharper disable InconsistentNaming

    [TestFixture]
    public class ListMonoidLaws
    {
        private ListMonoidAdapter<int> _listMonoidAdapter;

        [SetUp]
        public void SetUp()
        {
            _listMonoidAdapter = new ListMonoidAdapter<int>();
        }

        [Test]
        public void MAppendOfMEmptyAndXIsX()
        {
            // mempty <> x = x
            var x = new ListMonoid<int>(new[] { 1, 2, 3 });
            var actual = (ListMonoid<int>)_listMonoidAdapter.MAppend(_listMonoidAdapter.MEmpty, x);
            Assert.That(actual.List, Is.EqualTo(x.List));
        }

        [Test]
        public void MAppendOfXAndMEmptyIsX()
        {
            // x <> mempty = x
            var x = new ListMonoid<int>(new[] {1, 2, 3});
            var actual = (ListMonoid<int>) _listMonoidAdapter.MAppend(x, _listMonoidAdapter.MEmpty);
            Assert.That(actual.List, Is.EqualTo(x.List));
        }

        [Test]
        public void MAppendAssociativity()
        {
            // x <> (y <> z) = (x <> y) <> z
            var x = new ListMonoid<int>(new[] {1, 2, 3});
            var y = new ListMonoid<int>(new[] {4, 5, 6});
            var z = new ListMonoid<int>(new[] {7, 8, 9});
            var actual1 = (ListMonoid<int>) _listMonoidAdapter.MAppend(x, _listMonoidAdapter.MAppend(y, z));
            var actual2 = (ListMonoid<int>) _listMonoidAdapter.MAppend(_listMonoidAdapter.MAppend(x, y), z);
            Assert.That(actual1.List, Is.EqualTo(actual2.List));
        }
    }
}
