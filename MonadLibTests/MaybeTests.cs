#pragma warning disable 168

using System;
using System.Linq;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    // ReSharper disable InconsistentNaming

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
            Assert.That(maybe.FromJust, Is.EqualTo(42));
        }

        [Test]
        public void FromJustAppliedToNothingThrowsException()
        {
            var maybe = Maybe.Nothing<int>();
            Assert.Throws<InvalidOperationException>(() => { var dummy = maybe.FromJust; });
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

        [Test]
        public void ListToMaybeAppliedToEmptyList()
        {
            var @as = Enumerable.Range(1, 0);
            var actual = Maybe.ListToMaybe(@as);
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void ListToMaybeAppliedToListWithOneItem()
        {
            var @as = Enumerable.Range(1, 1);
            var actual = Maybe.ListToMaybe(@as);
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(1));
        }

        [Test]
        public void ListToMaybeAppliedToListWithManyItems()
        {
            var @as = Enumerable.Range(1, 10);
            var actual = Maybe.ListToMaybe(@as);
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(1));
        }

        [Test]
        public void MapMaybe()
        {
            var @as = Enumerable.Range(1, 10);
            Func<int, bool> isEven = n => n % 2 == 0;
            var actual = Maybe.MapMaybe(a => isEven(a) ? Maybe.Just(a) : Maybe.Nothing<int>(), @as);
            Assert.That(actual, Is.EqualTo(new[] {2, 4, 6, 8, 10}));
        }

        [Test]
        public void CatMaybes()
        {
            var maybes = new[]
                {
                    Maybe.Just(1),
                    Maybe.Just(2),
                    Maybe.Nothing<int>(),
                    Maybe.Just(4),
                    Maybe.Nothing<int>(),
                    Maybe.Just(6)
                };
            var actual = Maybe.CatMaybes(maybes);
            Assert.That(actual, Is.EqualTo(new[] {1, 2, 4, 6}));
        }

        [Test]
        public void MatchWithDefaultAppliedToNothing()
        {
            var maybe = Maybe.Nothing<int>();
            var actual = Maybe.MatchWithDefault("my-default-value", Convert.ToString, maybe);
            Assert.That(actual, Is.EqualTo("my-default-value"));
        }

        [Test]
        public void MatchWithDefaultAppliedToJust()
        {
            var maybe = Maybe.Just(42);
            var actual = Maybe.MatchWithDefault("my-default-value", Convert.ToString, maybe);
            Assert.That(actual, Is.EqualTo("42"));
        }

        [Test]
        public void ToListAppliedToJust()
        {
            var maybe = Maybe.Nothing<int>();
            var actual = maybe.ToList();
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void ToListAppliedToNothing()
        {
            var maybe = Maybe.Just(42);
            var actual = maybe.ToList();
            Assert.That(actual, Is.EqualTo(new[] {42}));
        }

        // TODO: add tests to cover the following:
        // Maybe.Return
        // Maybe.Bind
        // Maybe.LiftM2
        // Maybe.LiftM3
        // Maybe.LiftM4
        // Maybe.LiftM5

        [Test]
        public void LiftMAppliedToJust()
        {
            var maybe = Maybe.Return(10).LiftM(a => Convert.ToString(a * a));
            Assert.That(maybe.IsJust, Is.True);
            Assert.That(maybe.IsNothing, Is.False);
            Assert.That(maybe.FromJust, Is.EqualTo("100"));
        }

        [Test]
        public void LiftMAppliedToNothing()
        {
            var maybe = Maybe.Nothing<int>().LiftM(a => Convert.ToString(a * a));
            Assert.That(maybe.IsJust, Is.False);
            Assert.That(maybe.IsNothing, Is.True);
        }

        [Test, TestCaseSource("TestCaseSourceForSequenceTests")]
        public void Sequence(Tuple<string, Maybe<int>[], bool, int[]> tuple)
        {
            var maybes = tuple.Item2;
            var expectedIsJust = tuple.Item3;
            var expectedFromJust = tuple.Item4;
            var actual = Maybe.Sequence(maybes);
            Assert.That(actual.IsJust, Is.EqualTo(expectedIsJust));
            if (expectedIsJust)
            {
                Assert.That(actual.FromJust, Is.EqualTo(expectedFromJust));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForSequenceTests")]
        public void Sequence_(Tuple<string, Maybe<int>[], bool, int[]> tuple)
        {
            var maybes = tuple.Item2;
            var expectedIsJust = tuple.Item3;
            var actual = Maybe.Sequence_(maybes);
            Assert.That(actual.IsJust, Is.EqualTo(expectedIsJust));
            if (expectedIsJust)
            {
                Assert.That(actual.FromJust, Is.EqualTo(new Unit()));
            }
        }

        [Test]
        public void MapMWithFuncReturningJusts()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            var actual = Maybe.MapM(n => Maybe.Just(Convert.ToString(n)), ints);
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(new[] { "1", "2", "3", "4", "5" }));
        }

        [Test]
        public void MapMWithFuncReturningMixtureOfJustsAndNothings()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            var actual = Maybe.MapM(n => n < 4 ? Maybe.Just(Convert.ToString(n)) : Maybe.Nothing<string>(), ints);
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void MapM_WithFuncReturningJusts()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            var actual = Maybe.MapM_(n => Maybe.Just(Convert.ToString(n)), ints);
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(new Unit()));
        }

        [Test]
        public void MapM_WithFuncReturningMixtureOfJustsAndNothings()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            var actual = Maybe.MapM_(n => n < 4 ? Maybe.Just(Convert.ToString(n)) : Maybe.Nothing<string>(), ints);
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void ReplicateMAppliedToJust()
        {
            var actual = Maybe.ReplicateM(5, Maybe.Just(42));
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(new[] { 42, 42, 42, 42, 42 }));
        }

        [Test]
        public void ReplicateMAppliedToNothing()
        {
            var actual = Maybe.ReplicateM(5, Maybe.Nothing<int>());
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void ReplicateM_AppliedToJust()
        {
            var actual = Maybe.ReplicateM_(5, Maybe.Just(42));
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(new Unit()));
        }

        [Test]
        public void ReplicateM_AppliedToNothing()
        {
            var actual = Maybe.ReplicateM_(5, Maybe.Nothing<int>());
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void JoinAppliedToNothing()
        {
            var actual = Maybe.Join(Maybe.Nothing<Maybe<int>>());
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void JoinAppliedToJustOfJust()
        {
            var actual = Maybe.Join(Maybe.Just(Maybe.Just(42)));
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(42));
        }

        [Test]
        public void JoinAppliedToJustOfNothing()
        {
            var actual = Maybe.Join(Maybe.Just(Maybe.Nothing<int>()));
            Assert.That(actual.IsNothing, Is.True);
        }

        private static readonly object[] TestCaseSourceForSequenceTests =
            {
                Tuple.Create(
                    "4 Justs",
                    new[] {Maybe.Just(1), Maybe.Just(2), Maybe.Just(3), Maybe.Just(4)},
                    true,
                    new[] {1, 2, 3, 4}),

                Tuple.Create(
                    "3 Justs and 1 Nothing",
                    new[] {Maybe.Just(1), Maybe.Just(2), Maybe.Nothing<int>(), Maybe.Just(4)},
                    false,
                    null as int[]),

                Tuple.Create(
                    "Empty list of maybes",
                    new Maybe<int>[] {},
                    true,
                    new int[] {})
            };
    }
}
