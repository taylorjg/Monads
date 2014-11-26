using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class ListMonoidTests
    {
        private ListMonoidAdapter<int> _listMonoidAdapter;

        [SetUp]
        public void SetUp()
        {
            _listMonoidAdapter = new ListMonoidAdapter<int>();
        }

        [Test]
        public void MEmpty()
        {
            var actual = (ListMonoid<int>) _listMonoidAdapter.MEmpty;
            Assert.That(actual.List, Is.Empty);
        }

        [Test]
        public void MAppend()
        {
            var lm1 = new ListMonoid<int>(new[] {1, 2, 3});
            var lm2 = new ListMonoid<int>(new[] {4, 5, 6});
            var actual = (ListMonoid<int>) _listMonoidAdapter.MAppend(lm1, lm2);
            Assert.That(actual.List, Is.EqualTo(new[] {1, 2, 3, 4, 5, 6}));
        }

        [Test]
        public void MConcat()
        {
            var lm1 = new ListMonoid<int>(new[] {1, 2, 3});
            var lm2 = new ListMonoid<int>(new[] {4, 5, 6});
            var lm3 = new ListMonoid<int>(new[] {7, 8, 9});
            var actual = (ListMonoid<int>) _listMonoidAdapter.MConcat(new[] {lm1, lm2, lm3});
            Assert.That(actual.List, Is.EqualTo(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9}));
        }
    }
}
