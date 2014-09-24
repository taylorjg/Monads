using System;
using System.Collections.Generic;
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
#pragma warning disable 168
            Assert.Throws<InvalidOperationException>(() => { var dummy = maybe.FromJust; });
#pragma warning restore 168
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
        public void MapOrDefaultAppliedToNothing()
        {
            var maybe = Maybe.Nothing<int>();
            var actual = Maybe.MapOrDefault("my-default-value", Convert.ToString, maybe);
            Assert.That(actual, Is.EqualTo("my-default-value"));
        }

        [Test]
        public void MapOrDefaultAppliedToJust()
        {
            var maybe = Maybe.Just(42);
            var actual = Maybe.MapOrDefault("my-default-value", Convert.ToString, maybe);
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
        // Maybe.BindIgnoringLeft

        [Test, TestCaseSource("TestCaseSourceForLiftMTests")]
        public void LiftM(Maybe<int> ma, bool expectedIsJust, int expectedFromJust)
        {
            var actual = Maybe.LiftM(a => a, ma);
            Assert.That(actual.IsJust, Is.EqualTo(expectedIsJust));
            if (expectedIsJust)
            {
                Assert.That(actual.FromJust, Is.EqualTo(expectedFromJust));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForLiftM2Tests")]
        public void LiftM2(Maybe<int> ma, Maybe<int> mb, bool expectedIsJust, int expectedFromJust)
        {
            var actual = Maybe.LiftM2((a, b) => a + b, ma, mb);
            Assert.That(actual.IsJust, Is.EqualTo(expectedIsJust));
            if (expectedIsJust)
            {
                Assert.That(actual.FromJust, Is.EqualTo(expectedFromJust));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForLiftM3Tests")]
        public void LiftM3(Maybe<int> ma, Maybe<int> mb, Maybe<int> mc, bool expectedIsJust, int expectedFromJust)
        {
            var actual = Maybe.LiftM3((a, b, c) => a + b + c, ma, mb, mc);
            Assert.That(actual.IsJust, Is.EqualTo(expectedIsJust));
            if (expectedIsJust)
            {
                Assert.That(actual.FromJust, Is.EqualTo(expectedFromJust));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForLiftM4Tests")]
        public void LiftM4(Maybe<int> ma, Maybe<int> mb, Maybe<int> mc, Maybe<int> md, bool expectedIsJust, int expectedFromJust)
        {
            var actual = Maybe.LiftM4((a, b, c, d) => a + b + c + d, ma, mb, mc, md);
            Assert.That(actual.IsJust, Is.EqualTo(expectedIsJust));
            if (expectedIsJust)
            {
                Assert.That(actual.FromJust, Is.EqualTo(expectedFromJust));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForLiftM5Tests")]
        public void LiftM5(Maybe<int> ma, Maybe<int> mb, Maybe<int> mc, Maybe<int> md, Maybe<int> me, bool expectedIsJust, int expectedFromJust)
        {
            var actual = Maybe.LiftM5((a, b, c, d, e) => a + b + c + d + e, ma, mb, mc, md, me);
            Assert.That(actual.IsJust, Is.EqualTo(expectedIsJust));
            if (expectedIsJust)
            {
                Assert.That(actual.FromJust, Is.EqualTo(expectedFromJust));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForSequenceTests")]
        public void Sequence(Maybe<int>[] maybes, bool expectedIsJust, int[] expectedFromJust)
        {
            var actual = Maybe.Sequence(maybes);
            Assert.That(actual.IsJust, Is.EqualTo(expectedIsJust));
            if (expectedIsJust)
            {
                Assert.That(actual.FromJust, Is.EqualTo(expectedFromJust));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForSequenceTests")]
        public void Sequence_(Maybe<int>[] maybes, bool expectedIsJust, int[] expectedFromJust)
        {
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

        // ReSharper disable UnusedMethodReturnValue.Local

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftMTests()
        {
            yield return new TestCaseData(Maybe.Just(1), true, 1).SetName("1 Just");
            yield return new TestCaseData(Maybe.Nothing<int>(), false, default(int)).SetName("1 Nothing");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM2Tests()
        {
            yield return new TestCaseData(Maybe.Just(1), Maybe.Just(2), true, 3).SetName("2 Justs");
            yield return new TestCaseData(Maybe.Just(1), Maybe.Nothing<int>(), false, default(int)).SetName("1 Just and 1 Nothing");
            yield return new TestCaseData(Maybe.Nothing<int>(), Maybe.Nothing<int>(), false, default(int)).SetName("2 Nothings");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM3Tests()
        {
            yield return new TestCaseData(Maybe.Just(1), Maybe.Just(2), Maybe.Just(3), true, 6).SetName("3 Justs");
            yield return new TestCaseData(Maybe.Just(1), Maybe.Just(2), Maybe.Nothing<int>(), false, default(int)).SetName("2 Justs and 1 Nothing");
            yield return new TestCaseData(Maybe.Nothing<int>(), Maybe.Nothing<int>(), Maybe.Nothing<int>(), false, default(int)).SetName("3 Nothings");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM4Tests()
        {
            yield return new TestCaseData(Maybe.Just(1), Maybe.Just(2), Maybe.Just(3), Maybe.Just(4), true, 10).SetName("4 Justs");
            yield return new TestCaseData(Maybe.Just(1), Maybe.Just(2), Maybe.Just(3), Maybe.Nothing<int>(), false, default(int)).SetName("3 Justs and 1 Nothing");
            yield return new TestCaseData(Maybe.Nothing<int>(), Maybe.Nothing<int>(), Maybe.Nothing<int>(), Maybe.Nothing<int>(), false, default(int)).SetName("4 Nothings");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM5Tests()
        {
            yield return new TestCaseData(Maybe.Just(1), Maybe.Just(2), Maybe.Just(3), Maybe.Just(4), Maybe.Just(5), true, 15).SetName("5 Justs");
            yield return new TestCaseData(Maybe.Just(1), Maybe.Just(2), Maybe.Just(3), Maybe.Just(4), Maybe.Nothing<int>(), false, default(int)).SetName("4 Justs and 1 Nothing");
            yield return new TestCaseData(Maybe.Nothing<int>(), Maybe.Nothing<int>(), Maybe.Nothing<int>(), Maybe.Nothing<int>(), Maybe.Nothing<int>(), false, default(int)).SetName("5 Nothings");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForSequenceTests()
        {
            yield return new TestCaseData(
                new[]
                    {
                        Maybe.Just(1),
                        Maybe.Just(2),
                        Maybe.Just(3),
                        Maybe.Just(4)
                    },
                true,
                new[] {1, 2, 3, 4}).SetName("4 Justs");

            yield return new TestCaseData(
                new[]
                    {
                        Maybe.Just(1),
                        Maybe.Just(2),
                        Maybe.Nothing<int>(),
                        Maybe.Just(4)
                    },
                false,
                null).SetName("3 Justs and 1 Nothing");

            yield return new TestCaseData(
                new Maybe<int>[] {},
                true,
                new int[] {}).SetName("Empty list of maybes");
        }
    }
}
