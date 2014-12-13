using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class MaybeFunctorTests
    {
        [Test]
        public void FMapJust()
        {
            var ma = Maybe.Just(42);
            var actual = ma.FMap(Convert.ToString);
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual, Is.EqualTo(Maybe.Just("42")));
        }

        [Test]
        public void FMapNothing()
        {
            var ma = Maybe.Nothing<int>();
            var actual = ma.FMap(Convert.ToString);
            Assert.That(actual.IsNothing, Is.True);
        }
    }
}
