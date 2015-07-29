using MonadLib;
using NUnit.Framework;

namespace MonadLibTests.MaybeTests
{
    // ReSharper disable InconsistentNaming

    [TestFixture]
    internal class MonadPlusLaws
    {
        [Test]
        public void MPlusOfMZeroAndMIsM()
        {
            // mzero `mplus` m  =  m
            var m = Maybe.Just(42);
            var actual = Maybe.MZero<int>().MPlus(m);
            Assert.That(actual, Is.EqualTo(m));
        }

        [Test]
        public void MPlusOfMAndMZeroIsM()
        {
            // m `mplus` mzero  =  m
            var m = Maybe.Just(42);
            var actual = m.MPlus(Maybe.MZero<int>());
            Assert.That(actual, Is.EqualTo(m));
        }

        [Test]
        public void MPlusAssociativity()
        {
            // m `mplus` (n `mplus` o)  =  (m `mplus` n) `mplus` o
            var m = Maybe.Just(42);
            var n = Maybe.Just(43);
            var o = Maybe.Just(44);
            var actual1 = m.MPlus(n.MPlus(o));
            var actual2 = m.MPlus(n).MPlus(o);
            Assert.That(actual1, Is.EqualTo(actual2));
        }
    }
}
