using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class ListMonoidTests
    {
        [Test]
        public void MEmpty()
        {
            var actual = ListMonoid.MEmpty<int>();
            Assert.That(actual.List, Is.Empty);
        }

        [Test]
        public void MAppend()
        {
            var lm1 = new ListMonoid<int>(1, 2, 3);
            var lm2 = new ListMonoid<int>(4, 5, 6);
            var actual = lm1.MAppend(lm2);
            Assert.That(actual.List, Is.EqualTo(new[] {1, 2, 3, 4, 5, 6}));
        }

        [Test]
        public void MConcat()
        {
            var lm1 = new ListMonoid<int>(1, 2, 3);
            var lm2 = new ListMonoid<int>(4, 5, 6);
            var lm3 = new ListMonoid<int>(7, 8, 9);
            var actual = ListMonoid.MConcat(lm1, lm2, lm3);
            Assert.That(actual.List, Is.EqualTo(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9}));
        }
    }
}
