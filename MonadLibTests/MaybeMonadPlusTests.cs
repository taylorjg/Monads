using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class MaybeMonadPlusTests
    {
        [Test]
        public void MFilter()
        {
            Func<int, bool> isEven = n => n % 2 == 0;

            var actual1 = Maybe.MFilter(isEven, Maybe.Just(8));
            Assert.That(actual1.IsJust, Is.True);
            Assert.That(actual1.FromJust, Is.EqualTo(8));

            var actual2 = Maybe.MFilter(isEven, Maybe.Just(13));
            Assert.That(actual2.IsNothing, Is.True);

            var actual3 = Maybe.Just(8).MFilter(isEven);
            Assert.That(actual3.IsJust, Is.True);
            Assert.That(actual3.FromJust, Is.EqualTo(8));

            var actual4 = Maybe.Just(13).MFilter(isEven);
            Assert.That(actual4.IsNothing, Is.True);
        }

        [Test]
        public void MSum123()
        {
            var actual = Maybe.MSum(Maybe.Just(1), Maybe.Just(2), Maybe.Just(3));
            Assert.That(actual.FromJust, Is.EqualTo(1));
        }

        [Test]
        public void MSum1Nothing()
        {
            var actual = Maybe.MSum(Maybe.Just(1), Maybe.Nothing<int>());
            Assert.That(actual.FromJust, Is.EqualTo(1));
        }

        [Test]
        public void MSumNothing1()
        {
            var actual = Maybe.MSum(Maybe.Nothing<int>(), Maybe.Just(1));
            Assert.That(actual.FromJust, Is.EqualTo(1));
        }

        [Test]
        public void MSumNothingNothing()
        {
            var actual = Maybe.MSum(Maybe.Nothing<int>(), Maybe.Nothing<int>());
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void GuardTrue()
        {
            var actual = Maybe.Guard(true);
            Assert.That(actual, Is.EqualTo(Maybe.Just(new Unit())));
        }

        [Test]
        public void GuardFalse()
        {
            var actual = Maybe.Guard(false);
            Assert.That(actual, Is.EqualTo(Maybe.Nothing<Unit>()));
        }
    }
}
