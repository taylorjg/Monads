using System;
using System.Collections.Generic;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    // ReSharper disable InconsistentNaming

    using EitherString = Either<string>;

    [TestFixture]
    internal class EitherTests
    {
        private static IEnumerable<Either<string, int>> MixtureOfLeftsAndRights()
        {
            return new[]
                {
                    EitherString.Right(1),
                    EitherString.Right(2),
                    EitherString.Left<int>("Error 1"),
                    EitherString.Right(4),
                    EitherString.Left<int>("Error 2"),
                    EitherString.Right(6)
                };
        }

        [Test]
        public void Left()
        {
            var either = EitherString.Left<int>("error");
            Assert.That(either.IsLeft, Is.True);
            Assert.That(either.IsRight, Is.False);
            Assert.That(either.Left, Is.EqualTo("error"));
        }

        [Test]
        public void Right()
        {
            var either = EitherString.Right(42);
            Assert.That(either.IsLeft, Is.False);
            Assert.That(either.IsRight, Is.True);
            Assert.That(either.Right, Is.EqualTo(42));
        }

        [Test]
        public void LeftAppliedToRightThrowsException()
        {
            var either = EitherString.Right(42);
            #pragma warning disable 168
            Assert.Throws<InvalidOperationException>(() => { var dummy = either.Left; });
            #pragma warning restore 168
        }

        [Test]
        public void RightAppliedToLeftThrowsException()
        {
            var either = EitherString.Left<int>("error");
            #pragma warning disable 168
            Assert.Throws<InvalidOperationException>(() => { var dummy = either.Right; });
            #pragma warning restore 168
        }

        [Test]
        public void MatchAppliedToLeft()
        {
            var either = EitherString.Left<int>("error");
            var leftActionCalled = false;
            var leftActionParam = default(string);
            var rightActionCalled = false;
            either.Match(
                e =>
                    {
                        leftActionCalled = true;
                        leftActionParam = e;
                    },
                _ =>
                    {
                        rightActionCalled = true;
                    });
            Assert.That(leftActionCalled, Is.True);
            Assert.That(leftActionParam, Is.EqualTo("error"));
            Assert.That(rightActionCalled, Is.False);
        }

        [Test]
        public void MatchAppliedToRight()
        {
            var either = EitherString.Right(42);
            var leftActionCalled = false;
            var rightActionCalled = false;
            var rightActionParam = default(int);
            either.Match(
                _ =>
                {
                    leftActionCalled = true;
                },
                a =>
                {
                    rightActionCalled = true;
                    rightActionParam = a;
                });
            Assert.That(leftActionCalled, Is.False);
            Assert.That(rightActionCalled, Is.True);
            Assert.That(rightActionParam, Is.EqualTo(42));
        }

        [Test]
        public void MatchOfTAppliedToLeft()
        {
            var either = EitherString.Left<int>("error");
            var actual = either.Match(_ => 1.0, _ => 2.0);
            Assert.That(actual, Is.EqualTo(1.0));
        }

        [Test]
        public void MatchOfTAppliedToRight()
        {
            var either = EitherString.Right(42);
            var actual = either.Match(_ => 1.0, _ => 2.0);
            Assert.That(actual, Is.EqualTo(2.0));
        }

        [Test]
        public void Lefts()
        {
            var eithers = MixtureOfLeftsAndRights();
            var actual = Either.Lefts(eithers);
            Assert.That(actual, Is.EqualTo(new[] { "Error 1", "Error 2" }));
        }

        [Test]
        public void Rights()
        {
            var eithers = MixtureOfLeftsAndRights();
            var actual = Either.Rights(eithers);
            Assert.That(actual, Is.EqualTo(new[] { 1, 2, 4, 6 }));
        }

        [Test]
        public void PartitionEithers()
        {
            var eithers = MixtureOfLeftsAndRights();
            var actual = Either.PartitionEithers(eithers);
            Assert.That(actual.Item1, Is.EqualTo(new[] { "Error 1", "Error 2" }));
            Assert.That(actual.Item2, Is.EqualTo(new[] {1, 2, 4, 6}));
        }

        [Test]
        public void MapEitherAppliedToLeft()
        {
            var either = EitherString.Left<int>("error");
            var actual = either.MapEither(l => l + l, null);
            Assert.That(actual, Is.EqualTo("errorerror"));
        }

        [Test]
        public void MapEitherAppliedToRight()
        {
            var either = EitherString.Right(42);
            var actual = either.MapEither(null, r => r + r);
            Assert.That(actual, Is.EqualTo(84));
        }

        [Test, TestCaseSource("TestCaseSourceForEquals")]
        public void Equals(Either<string, int> e1, Either<string, int> e2, bool expected)
        {
            var actual1 = e1.Equals(e2);
            Assert.That(actual1, Is.EqualTo(expected));

            var actual2 = e2.Equals(e1);
            Assert.That(actual2, Is.EqualTo(expected));
        }

        // ReSharper disable SuspiciousTypeConversion.Global
        [Test]
        public void EqualsOfEithersWithDifferentLeftTypes()
        {
            var e1 = Either<string>.Right(42);
            var e2 = Either<bool>.Right(42);

            var actual1 = e1.Equals(e2);
            Assert.That(actual1, Is.EqualTo(false));

            var actual2 = e2.Equals(e1);
            Assert.That(actual2, Is.EqualTo(false));
        }
        // ReSharper restore SuspiciousTypeConversion.Global

        // ReSharper disable UnusedMethodReturnValue.Local

        private static IEnumerable<ITestCaseData> TestCaseSourceForEquals()
        {
            yield return MakeEqualsTestCaseData("error", "error", true).SetName("Left(\"error\") and Left(\"error\") => true");
            yield return MakeEqualsTestCaseData("error1", "error2", false).SetName("Left(\"error1\") and Left(\"error2\") => false");
            yield return MakeEqualsTestCaseData(42, 42, true).SetName("Right(42) and Right(42) => true");
            yield return MakeEqualsTestCaseData(42, 43, false).SetName("Right(42) and Right(43) => false");
            yield return MakeEqualsTestCaseData("error", 42, false).SetName("Left(\"error\") and Right(42) => false");
            yield return MakeEqualsTestCaseData(42, "error", false).SetName("Right(42) and Left(\"error\") => false");
        }

        private static TestCaseData MakeEqualsTestCaseData(int n1, int n2, bool flag)
        {
            return new TestCaseData(EitherString.Right(n1), EitherString.Right(n2), flag);
        }

        private static TestCaseData MakeEqualsTestCaseData(int n, string e, bool flag)
        {
            return new TestCaseData(EitherString.Right(n), EitherString.Left<int>(e), flag);
        }

        private static TestCaseData MakeEqualsTestCaseData(string e, int n, bool flag)
        {
            return new TestCaseData(EitherString.Left<int>(e), EitherString.Right(n), flag);
        }

        private static TestCaseData MakeEqualsTestCaseData(string e1, string e2, bool flag)
        {
            return new TestCaseData(EitherString.Left<int>(e1), EitherString.Left<int>(e2), flag);
        }
    }
}
