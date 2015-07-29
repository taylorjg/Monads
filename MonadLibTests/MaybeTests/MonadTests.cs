using System;
using System.Collections.Generic;
using System.Linq;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests.MaybeTests
{
    // ReSharper disable InconsistentNaming

    [TestFixture]
    internal class MonadTests
    {
        [Test, TestCaseSource("TestCaseSourceForLiftMTests")]
        public void LiftM(Maybe<int>[] maybes, bool expectedIsJust, int expectedFromJust)
        {
            var actual = Maybe.LiftM(a => a, maybes[0]);
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
            var actual = Maybe.FoldM((a, b) => Maybe.Just(a + b), 0, Enumerable.Range(1, 5).ToArray());
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(15));
        }

        [Test]
        public void FoldMWhereFuncReturnsMixtureOfJustAndNothing()
        {
            var actual = Maybe.FoldM((a, b) => a > 3 ? Maybe.Nothing<int>() : Maybe.Just(a + b), 0, Enumerable.Range(1, 5).ToArray());
            Assert.That(actual.IsNothing, Is.True);
        }

        [Test]
        public void ZipWithMWhereFuncAlwaysReturnsJust()
        {
            var actual = Maybe.ZipWithM((a, b) => Maybe.Just(a + b), Enumerable.Range(1, 5), Enumerable.Range(10, 5));
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(new[] { 1 + 10, 2 + 11, 3 + 12, 4 + 13, 5 + 14 }));
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
            var actual = Maybe.FilterM(n => Maybe.Just(n < 10), Enumerable.Range(1, 20).ToArray());
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(Enumerable.Range(1, 9)));
        }

        [Test]
        public void FilterMWherePredicateReturnsMixtureOfJustAndNothing()
        {
            var actual = Maybe.FilterM(n => n > 10 ? Maybe.Nothing<bool>() : Maybe.Just(true), Enumerable.Range(1, 20).ToArray());
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
            yield return MakeLiftTestCaseData(1).SetName("1 Just");
            yield return MakeLiftTestCaseData(new int?[]{null}).SetName("1 Nothing");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM2Tests()
        {
            yield return MakeLiftTestCaseData(1, 2).SetName("2 Justs");
            yield return MakeLiftTestCaseData(1, null).SetName("1 Just and 1 Nothing");
            yield return MakeLiftTestCaseData(null, null).SetName("2 Nothings");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM3Tests()
        {
            yield return MakeLiftTestCaseData(1, 2, 3).SetName("3 Justs");
            yield return MakeLiftTestCaseData(1, 2, null).SetName("2 Justs and 1 Nothing");
            yield return MakeLiftTestCaseData(null, null, null).SetName("3 Nothings");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM4Tests()
        {
            yield return MakeLiftTestCaseData(1, 2, 3, 4).SetName("4 Justs");
            yield return MakeLiftTestCaseData(1, 2, 3, null).SetName("3 Justs and 1 Nothing");
            yield return MakeLiftTestCaseData(null, null, null, null).SetName("4 Nothings");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM5Tests()
        {
            yield return MakeLiftTestCaseData(1, 2, 3, 4, 5).SetName("5 Justs");
            yield return MakeLiftTestCaseData(1, 2, 3, 4, null).SetName("4 Justs and 1 Nothing");
            yield return MakeLiftTestCaseData(null, null, null, null, null).SetName("5 Nothings");
        }

        private static TestCaseData MakeLiftTestCaseData(params int?[] ns)
        {
            var ms = ns.Select(n => n.ToMaybe()).ToArray();
            var expectedIsJust = ns.All(n => n.HasValue);
            var expectedFromJust = (expectedIsJust) ? ns.Sum() : default(int);
            return new TestCaseData(ms, expectedIsJust, expectedFromJust);
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForSequenceTests()
        {
            yield return MakeSequenceTestCaseData(1, 2, 3, 4).SetName("4 Justs");
            yield return MakeSequenceTestCaseData(1, 2, null, 4).SetName("3 Justs and 1 Nothing");
            yield return MakeSequenceTestCaseData().SetName("Empty list of maybes");
        }

        private static TestCaseData MakeSequenceTestCaseData(params int?[] ns)
        {
            var ms = ns.Select(n => n.ToMaybe()).ToArray();
            var expectedIsJust = ns.All(n => n.HasValue);
            var expectedFromJust = expectedIsJust ? ns.SelectMany(n => n.HasValue ? new[] { n.Value } : new int[0]).ToArray() : null;
            return new TestCaseData(ms, expectedIsJust, expectedFromJust);
        }
    }
}
