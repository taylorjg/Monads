using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests.MaybeTests
{
    [TestFixture]
    internal class FunctorTests
    {
        [Test]
        public void FMapJust()
        {
            var fa = Maybe.Just(42);
            var fb = fa.FMap(Convert.ToString);
            Assert.That(fb.IsJust, Is.True);
            Assert.That(fb.FromJust, Is.EqualTo("42"));
        }

        [Test]
        public void FMapNothing()
        {
            var fa = Maybe.Nothing<int>();
            var fb = fa.FMap(Convert.ToString);
            Assert.That(fb.IsNothing, Is.True);
        }
    }
}
