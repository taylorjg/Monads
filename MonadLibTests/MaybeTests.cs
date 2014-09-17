using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class MaybeTests
    {
        [Test]
        public void Nothing()
        {
            var maybe = Maybe.Nothing<int>();
            Assert.That(maybe.IsJust, Is.False);
            Assert.That(maybe.IsNothing, Is.True);
        }

        [Test]
        public void Just()
        {
            var maybe = Maybe.Just(42);
            Assert.That(maybe.IsJust, Is.True);
            Assert.That(maybe.IsNothing, Is.False);
            Assert.That(maybe.FromJust(), Is.EqualTo(42));
        }

        [Test]
        public void FromJustAppliedToNothingThrowsException()
        {
            var maybe = Maybe.Nothing<int>();
            Assert.Throws<InvalidOperationException>(() => maybe.FromJust());
        }

        // TODO: add tests re monadic behaviour:
        // Maybe.Unit
        // Maybe.Bind x 1 with nothing/just
        // Maybe.Bind x 2 with nothing/just combinations
        // Maybe.Unit => Maybe.Bind x 2 with nothing/just combinations
        // Maybe.LiftM2 with left/right
        // Maybe.LiftM3 with left/right

        [Test]
        public void LiftMAppliedToJust()
        {
            var maybe = Maybe.Unit(10).LiftM(a => Convert.ToString(a * a));
            Assert.That(maybe.IsJust, Is.True);
            Assert.That(maybe.IsNothing, Is.False);
            Assert.That(maybe.FromJust(), Is.EqualTo("100"));
        }

        [Test]
        public void LiftMAppliedToNothing()
        {
            var maybe = Maybe.Nothing<int>().LiftM(a => Convert.ToString(a * a));
            Assert.That(maybe.IsJust, Is.False);
            Assert.That(maybe.IsNothing, Is.True);
        }
    }
}
