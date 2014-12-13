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

        // ReSharper disable UnusedMethodReturnValue.Local

        private static IEnumerable<ITestCaseData> TestCaseSourceForEquals()
        {
            yield return MakeEqualsTestCaseData(null, null, true).SetName("Nothing and Nothing => true");
            yield return MakeEqualsTestCaseData(42, 42, true).SetName("Just(42) and Just(42) => true");
            yield return MakeEqualsTestCaseData(42, 43, false).SetName("Just(42) and Just(43) => false");
            yield return MakeEqualsTestCaseData(null, 42, false).SetName("Nothing and Just(42) => false");
            yield return MakeEqualsTestCaseData(42, null, false).SetName("Just(42) and Nothing => false");
        }

        private static TestCaseData MakeEqualsTestCaseData(int? a, int? b, bool flag)
        {
            return new TestCaseData(a.ToMaybe(), b.ToMaybe(), flag);
        }
    }
}
