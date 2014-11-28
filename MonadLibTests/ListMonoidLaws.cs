using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    // ReSharper disable InconsistentNaming

    [TestFixture]
    public class ListMonoidLaws
    {
        [Test]
        public void MAppendOfMEmptyAndXIsX()
        {
            // mempty <> x = x
            var x = new ListMonoid<int>(1, 2, 3);
            var actual = ListMonoid.MEmpty<int>().MAppend(x);
            Assert.That(actual.List, Is.EqualTo(x.List));
        }

        [Test]
        public void MAppendOfXAndMEmptyIsX()
        {
            // x <> mempty = x
            var x = new ListMonoid<int>(1, 2, 3);
            var actual = x.MAppend(ListMonoid.MEmpty<int>());
            Assert.That(actual.List, Is.EqualTo(x.List));
        }

        [Test]
        public void MAppendAssociativity()
        {
            // x <> (y <> z) = (x <> y) <> z
            var x = new ListMonoid<int>(1, 2, 3);
            var y = new ListMonoid<int>(4, 5, 6);
            var z = new ListMonoid<int>(7, 8, 9);
            var actual1 = x.MAppend(y.MAppend(z));
            var actual2 = x.MAppend(y).MAppend(z);
            Assert.That(actual1.List, Is.EqualTo(actual2.List));
        }
    }
}
