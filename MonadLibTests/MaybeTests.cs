using System;
using Monads;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class MaybeTests
    {
        [Test]
        public void Just()
        {
            var maybe = Maybe.Just(42);
            Assert.That(maybe.IsJust, Is.True);
            Assert.That(maybe.IsNothing, Is.False);
            Assert.That(maybe.FromJust(), Is.EqualTo(42));
        }

        [Test]
        public void Nothing()
        {
            var maybe = Maybe.Nothing<int>();
            Assert.That(maybe.IsJust, Is.False);
            Assert.That(maybe.IsNothing, Is.True);
        }

        [Test]
        public void FromJustOfNothingThrowsException()
        {
            var maybe = Maybe.Nothing<int>();
            Assert.Throws<InvalidOperationException>(() => maybe.FromJust());
        }

        // TODO: add tests re monadic behaviour:
        // Maybe.Unit
        // Maybe.Bind x 1 with nothing/just
        // Maybe.Bind x 2 with nothing/just combinations
        // Maybe.Unit => Maybe.Bind x 2 with nothing/just combinations
        // Maybe.LiftM with nothing/just
    }
}
