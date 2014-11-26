using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

            const string myDefaultValue = "my-default-value";

            var actual1 = Maybe.MapOrDefault(myDefaultValue, Convert.ToString, maybe);
            Assert.That(actual1, Is.EqualTo(myDefaultValue));

            var actual2 = maybe.MapOrDefault(myDefaultValue, Convert.ToString);
            Assert.That(actual2, Is.EqualTo(myDefaultValue));
        }

        [Test]
        public void MapOrDefaultAppliedToJust()
        {
            var maybe = Maybe.Just(42);

            const string myDefaultValue = "my-default-value";

            var actual1 = Maybe.MapOrDefault(myDefaultValue, Convert.ToString, maybe);
            Assert.That(actual1, Is.EqualTo("42"));

            var actual2 = maybe.MapOrDefault(myDefaultValue, Convert.ToString);
            Assert.That(actual2, Is.EqualTo("42"));
        }

        [Test, TestCaseSource("TestCaseSourceForEquals")]
        public void Equals(Maybe<int> m1, Maybe<int> m2, bool expected)
        {
            var actual1 = m1.Equals(m2);
            Assert.That(actual1, Is.EqualTo(expected));

            var actual2 = m2.Equals(m1);
            Assert.That(actual2, Is.EqualTo(expected));
        }

        // ReSharper disable UnusedMethodReturnValue.Local
        private static IEnumerable<ITestCaseData> TestCaseSourceForEquals()
        {
            yield return new TestCaseData(Maybe.Nothing<int>(), Maybe.Nothing<int>(), true).SetName("Nothing and Nothing");
            yield return new TestCaseData(Maybe.Just(42), Maybe.Just(42), true).SetName("Just(42) and Just(42)");
            yield return new TestCaseData(Maybe.Just(42), Maybe.Just(43), false).SetName("Just(42) and Just(43)");
            yield return new TestCaseData(Maybe.Nothing<int>(), Maybe.Just(42), false).SetName("Nothing and Just(43)");
        }
        // ReSharper restore UnusedMethodReturnValue.Local

        [Test]
        public void ToEnumerableAppliedToJust()
        {
            var maybe = Maybe.Nothing<int>();
            var actual = maybe.ToEnumerable();
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void ToEnumerableAppliedToNothing()
        {
            var maybe = Maybe.Just(42);
            var actual = maybe.ToEnumerable();
            Assert.That(actual, Is.EqualTo(new[] {42}));
        }

        [Test]
        public void GetValueWhenKeyIsInDictionary()
        {
            var dictionary = new Dictionary<int, string>
                {
                    {1, "one"},
                    {2, "two"},
                    {4, "four"}
                };
            var actual = dictionary.GetValue(2);
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo("two"));
        }

        [Test]
        public void GetValueWhenKeyIsNotInDictionary()
        {
            var dictionary = new Dictionary<int, string>
                {
                    {1, "one"},
                    {2, "two"},
                    {4, "four"}
                };
            var actual = dictionary.GetValue(3);
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void GetValueWhenNameIsInNameValueCollection()
        {
            var nameValueCollection = new NameValueCollection
                {
                    {"Go", "Green"},
                    {"Stop", "Red"},
                };
            var actual = nameValueCollection.GetValue("Go");
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo("Green"));
        }

        [Test]
        public void GetValueWhenNameIsNotInNameValueCollection()
        {
            var nameValueCollection = new NameValueCollection
                {
                    {"Go", "Green"},
                    {"Stop", "Red"},
                };
            var actual = nameValueCollection.GetValue("GetSet");
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void ToMaybeAppliedToNullableWithAValue()
        {
            int? n = 10;
            var actual = n.ToMaybe();
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(10));
        }

        [Test]
        public void ToMaybeAppliedToNullableWithoutAValue()
        {
            Func<int?> f = () => null;
            var n = f();
            var actual = n.ToMaybe();
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void ToNullableAppliedToJust()
        {
            var maybe = Maybe.Just(10);
            var n = maybe.ToNullable();
            Assert.That(n.HasValue, Is.True);
            // ReSharper disable PossibleInvalidOperationException
            Assert.That(n.Value, Is.EqualTo(10));
            // ReSharper restore PossibleInvalidOperationException
        }

        [Test]
        public void ToNullableAppliedToNothing()
        {
            var maybe = Maybe.Nothing<int>();
            var n = maybe.ToNullable();
            Assert.That(n.HasValue, Is.False);
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
        public void LiftM2(Maybe<int>[] maybes, bool expectedIsJust, int expectedFromJust)
        {
            var actual = Maybe.LiftM2((a, b) => a + b, maybes[0], maybes[1]);
            Assert.That(actual.IsJust, Is.EqualTo(expectedIsJust));
            if (expectedIsJust)
            {
                Assert.That(actual.FromJust, Is.EqualTo(expectedFromJust));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForLiftM3Tests")]
        public void LiftM3(Maybe<int>[] maybes, bool expectedIsJust, int expectedFromJust)
        {
            var actual = Maybe.LiftM3((a, b, c) => a + b + c, maybes[0], maybes[1], maybes[2]);
            Assert.That(actual.IsJust, Is.EqualTo(expectedIsJust));
            if (expectedIsJust)
            {
                Assert.That(actual.FromJust, Is.EqualTo(expectedFromJust));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForLiftM4Tests")]
        public void LiftM4(Maybe<int>[] maybes, bool expectedIsJust, int expectedFromJust)
        {
            var actual = Maybe.LiftM4((a, b, c, d) => a + b + c + d, maybes[0], maybes[1], maybes[2], maybes[3]);
            Assert.That(actual.IsJust, Is.EqualTo(expectedIsJust));
            if (expectedIsJust)
            {
                Assert.That(actual.FromJust, Is.EqualTo(expectedFromJust));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForLiftM5Tests")]
        public void LiftM5(Maybe<int>[] maybes, bool expectedIsJust, int expectedFromJust)
        {
            var actual = Maybe.LiftM5((a, b, c, d, e) => a + b + c + d + e, maybes[0], maybes[1], maybes[2], maybes[3], maybes[4]);
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

        [Test]
        public void FoldMWhereFuncAlwaysReturnsJust()
        {
            var actual = Maybe.FoldM((a, b) => Maybe.Just(a + b), 0, Enumerable.Range(1, 5));
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(15));
        }

        [Test]
        public void FoldMWhereFuncReturnsMixtureOfJustAndNothing()
        {
            var actual = Maybe.FoldM((a, b) => a > 3 ? Maybe.Nothing<int>() : Maybe.Just(a + b), 0, Enumerable.Range(1, 5));
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void ZipWithMWhereFuncAlwaysReturnsJust()
        {
            var actual = Maybe.ZipWithM((a, b) => Maybe.Just(a + b), Enumerable.Range(1, 5), Enumerable.Range(10, 5));
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(new[] {1 + 10, 2 + 11, 3 + 12, 4 + 13, 5 + 14}));
        }

        [Test]
        public void ZipWithMWhereFuncReturnsMixtureOfJustAndNothing()
        {
            var actual = Maybe.ZipWithM((a, b) => a > 3 ? Maybe.Nothing<int>() : Maybe.Just(a + b), Enumerable.Range(1, 5), Enumerable.Range(10, 5));
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void FilterMWherePredicateAlwaysReturnsJust()
        {
            var actual = Maybe.FilterM(n => Maybe.Just(n < 10), Enumerable.Range(1, 20));
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(Enumerable.Range(1, 9)));
        }

        [Test]
        public void FilterMWherePredicateReturnsMixtureOfJustAndNothing()
        {
            var actual = Maybe.FilterM(n => n > 10 ? Maybe.Nothing<bool>() : Maybe.Just(true), Enumerable.Range(1, 20));
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void ApAppliedToFuncAndNumber()
        {
            Func<int, int> f = a => a * a;
            var actual = Maybe.Ap(Maybe.Just(f), Maybe.Just(9));
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(81));
        }

        [Test]
        public void ApAppliedToFuncAndNothing()
        {
            Func<int, int> f = a => a * a;
            var actual = Maybe.Ap(Maybe.Just(f), Maybe.Nothing<int>());
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void ApAppliedToNothingAndNumber()
        {
            var actual = Maybe.Ap(Maybe.Nothing<Func<int, int>>(), Maybe.Just(9));
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void ApAppliedToNothingAndNothing()
        {
            var actual = Maybe.Ap(Maybe.Nothing<Func<int, int>>(), Maybe.Nothing<int>());
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
            yield return new TestCaseData(
                new[]
                    {
                        Maybe.Just(1),
                        Maybe.Just(2)
                    },
                true,
                3).SetName("2 Justs");

            yield return new TestCaseData(
                new[]
                    {
                        Maybe.Just(1),
                        Maybe.Nothing<int>()
                    },
                false,
                default(int)).SetName("1 Just and 1 Nothing");

            yield return new TestCaseData(
                new[]
                    {
                        Maybe.Nothing<int>(),
                        Maybe.Nothing<int>()
                    },
                false,
                default(int)).SetName("2 Nothings");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM3Tests()
        {
            yield return new TestCaseData(
                new[]
                    {
                        Maybe.Just(1),
                        Maybe.Just(2),
                        Maybe.Just(3)
                    },
                true,
                6).SetName("3 Justs");

            yield return new TestCaseData(
                new[]
                    {
                        Maybe.Just(1),
                        Maybe.Just(2),
                        Maybe.Nothing<int>()
                    },
                false,
                default(int)).SetName("2 Justs and 1 Nothing");

            yield return new TestCaseData(
                new[]
                    {
                        Maybe.Nothing<int>(),
                        Maybe.Nothing<int>(),
                        Maybe.Nothing<int>()
                    },
                false,
                default(int)).SetName("3 Nothings");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM4Tests()
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
                10).SetName("4 Justs");

            yield return new TestCaseData(
                new[]
                    {
                        Maybe.Just(1),
                        Maybe.Just(2),
                        Maybe.Just(3),
                        Maybe.Nothing<int>()
                    },
                false,
                default(int)).SetName("3 Justs and 1 Nothing");

            yield return new TestCaseData(
                new[]
                    {
                        Maybe.Nothing<int>(),
                        Maybe.Nothing<int>(),
                        Maybe.Nothing<int>(),
                        Maybe.Nothing<int>()
                    },
                false,
                default(int)).SetName("4 Nothings");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM5Tests()
        {
            yield return new TestCaseData(
                new[]
                    {
                        Maybe.Just(1),
                        Maybe.Just(2),
                        Maybe.Just(3),
                        Maybe.Just(4),
                        Maybe.Just(5)
                    },
                true,
                15).SetName("5 Justs");

            yield return new TestCaseData(
                new[]
                    {
                        Maybe.Just(1),
                        Maybe.Just(2),
                        Maybe.Just(3),
                        Maybe.Just(4),
                        Maybe.Nothing<int>()
                    },
                false,
                default(int)).SetName("4 Justs and 1 Nothing");

            yield return new TestCaseData(
                new[]
                    {
                        Maybe.Nothing<int>(),
                        Maybe.Nothing<int>(),
                        Maybe.Nothing<int>(),
                        Maybe.Nothing<int>(),
                        Maybe.Nothing<int>()
                    },
                false,
                default(int)).SetName("5 Nothings");
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
