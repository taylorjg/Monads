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

        [Test]
        public void FromMaybeAppliedToNothing()
        {
            var maybe = Maybe.Nothing<int>();
            var actual = maybe.FromMaybe(-1);
            Assert.That(actual, Is.EqualTo(-1));
        }

        [Test]
        public void FromMaybeAppliedToJust()
        {
            var maybe = Maybe.Just(42);
            var actual = maybe.FromMaybe(-1);
            Assert.That(actual, Is.EqualTo(42));
        }

        [Test]
        public void MatchAppliedToNothing()
        {
            var maybe = Maybe.Nothing<int>();
            var justActionCalled = false;
            var nothingActionCalled = false;
            maybe.Match(_ => { justActionCalled = true; }, () => { nothingActionCalled = true; });
            Assert.That(justActionCalled, Is.False);
            Assert.That(nothingActionCalled, Is.True);
        }

        [Test]
        public void MatchAppliedToJust()
        {
            var maybe = Maybe.Just(42);
            var justActionCalled = false;
            var justActionParam = default(int);
            var nothingActionCalled = false;
            maybe.Match(
                a =>
                    {
                        justActionCalled = true;
                        justActionParam = a;
                    },
                () => { nothingActionCalled = true; });
            Assert.That(justActionCalled, Is.True);
            Assert.That(justActionParam, Is.EqualTo(42));
            Assert.That(nothingActionCalled, Is.False);
        }

        [Test]
        public void MatchOfTAppliedToNothing()
        {
            var maybe = Maybe.Nothing<int>();
            var actual = maybe.Match(_ => "just", () => "nothing");
            Assert.That(actual, Is.EqualTo("nothing"));
        }

        [Test]
        public void MatchOfTAppliedToJust()
        {
            var maybe = Maybe.Just(42);
            var justFuncParam = default(int);
            var actual = maybe.Match(
                a =>
                    {
                        justFuncParam = a;
                        return "just";
                    },
                () => "nothing");
            Assert.That(actual, Is.EqualTo("just"));
            Assert.That(justFuncParam, Is.EqualTo(42));
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
