using System;
using System.Collections.Generic;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    // ReSharper disable InconsistentNaming

    [TestFixture]
    internal class EitherTests
    {
        private static IEnumerable<Either<string, int>> MixtureOfLeftsAndRights()
        {
            return new[]
                {
                    Either<string>.Right(1),
                    Either<string>.Right(2),
                    Either<string>.Left<int>("Error 1"),
                    Either<string>.Right(4),
                    Either<string>.Left<int>("Error 2"),
                    Either<string>.Right(6)
                };
        }

        [Test]
        public void Left()
        {
            var either = Either<string>.Left<int>("error");
            Assert.That(either.IsLeft, Is.True);
            Assert.That(either.IsRight, Is.False);
            Assert.That(either.Left, Is.EqualTo("error"));
        }

        [Test]
        public void Right()
        {
            var either = Either<string>.Right(42);
            Assert.That(either.IsLeft, Is.False);
            Assert.That(either.IsRight, Is.True);
            Assert.That(either.Right, Is.EqualTo(42));
        }

        [Test]
        public void LeftAppliedToRightThrowsException()
        {
            var either = Either<string>.Right(42);
#pragma warning disable 168
            Assert.Throws<InvalidOperationException>(() => { var dummy = either.Left; });
#pragma warning restore 168
        }

        [Test]
        public void RightAppliedToLeftThrowsException()
        {
            var either = Either<string>.Left<int>("error");
#pragma warning disable 168
            Assert.Throws<InvalidOperationException>(() => { var dummy = either.Right; });
#pragma warning restore 168
        }

        [Test]
        public void MatchAppliedToLeft()
        {
            var either = Either<string>.Left<int>("error");
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
            var either = Either<string>.Right(42);
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
            var either = Either<string>.Left<int>("error");
            var actual = either.Match(_ => 1.0, _ => 2.0);
            Assert.That(actual, Is.EqualTo(1.0));
        }

        [Test]
        public void MatchOfTAppliedToRight()
        {
            var either = Either<string>.Right(42);
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
            var either = Either<string>.Left<int>("error");
            var actual = either.MapEither(l => l + l, null);
            Assert.That(actual, Is.EqualTo("errorerror"));
        }

        [Test]
        public void MapEitherAppliedToRight()
        {
            var either = Either<string>.Right(42);
            var actual = either.MapEither(null, r => r + r);
            Assert.That(actual, Is.EqualTo(84));
        }

        // TODO: add tests to cover the following:
        // Either.Return
        // Either.Bind
        // Either.BindIgnoringLeft

        [Test, TestCaseSource("TestCaseSourceForLiftMTests")]
        public void LiftM(Either<string, int> ma, bool expectedIsRight, string expectedLeft, int expectedRight)
        {
            var actual = Either.LiftM(a => a, ma);
            Assert.That(actual.IsRight, Is.EqualTo(expectedIsRight));
            if (expectedIsRight)
            {
                Assert.That(actual.Right, Is.EqualTo(expectedRight));
            }
            else
            {
                Assert.That(actual.Left, Is.EqualTo(expectedLeft));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForLiftM2Tests")]
        public void LiftM2(Either<string, int> ma, Either<string, int> mb, bool expectedIsRight, string expectedLeft, int expectedRight)
        {
            var actual = Either.LiftM2((a, b) => a + b, ma, mb);
            Assert.That(actual.IsRight, Is.EqualTo(expectedIsRight));
            if (expectedIsRight)
            {
                Assert.That(actual.Right, Is.EqualTo(expectedRight));
            }
            else
            {
                Assert.That(actual.Left, Is.EqualTo(expectedLeft));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForLiftM3Tests")]
        public void LiftM3(Either<string, int> ma, Either<string, int> mb, Either<string, int> mc, bool expectedIsRight, string expectedLeft, int expectedRight)
        {
            var actual = Either.LiftM3((a, b, c) => a + b + c, ma, mb, mc);
            Assert.That(actual.IsRight, Is.EqualTo(expectedIsRight));
            if (expectedIsRight)
            {
                Assert.That(actual.Right, Is.EqualTo(expectedRight));
            }
            else
            {
                Assert.That(actual.Left, Is.EqualTo(expectedLeft));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForLiftM4Tests")]
        public void LiftM4(Either<string, int> ma, Either<string, int> mb, Either<string, int> mc, Either<string, int> md, bool expectedIsRight, string expectedLeft, int expectedRight)
        {
            var actual = Either.LiftM4((a, b, c, d) => a + b + c + d, ma, mb, mc, md);
            Assert.That(actual.IsRight, Is.EqualTo(expectedIsRight));
            if (expectedIsRight)
            {
                Assert.That(actual.Right, Is.EqualTo(expectedRight));
            }
            else
            {
                Assert.That(actual.Left, Is.EqualTo(expectedLeft));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForLiftM5Tests")]
        public void LiftM5(Either<string, int> ma, Either<string, int> mb, Either<string, int> mc, Either<string, int> md, Either<string, int> me, bool expectedIsRight, string expectedLeft, int expectedRight)
        {
            var actual = Either.LiftM5((a, b, c, d, e) => a + b + c + d + e, ma, mb, mc, md, me);
            Assert.That(actual.IsRight, Is.EqualTo(expectedIsRight));
            if (expectedIsRight)
            {
                Assert.That(actual.Right, Is.EqualTo(expectedRight));
            }
            else
            {
                Assert.That(actual.Left, Is.EqualTo(expectedLeft));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForSequenceTests")]
        public void Sequence(Either<string, int>[] eithers, bool expectedIsRight, string expectedLeft, int[] expectedRight)
        {
            var actual = Either.Sequence(eithers);
            Assert.That(actual.IsRight, Is.EqualTo(expectedIsRight));
            if (expectedIsRight)
            {
                Assert.That(actual.Right, Is.EqualTo(expectedRight));
            }
            else
            {
                Assert.That(actual.Left, Is.EqualTo(expectedLeft));
            }
        }

        [Test, TestCaseSource("TestCaseSourceForSequenceTests")]
        public void Sequence_(Either<string, int>[] eithers, bool expectedIsRight, string expectedLeft, int[] expectedRight)
        {
            var actual = Either.Sequence_(eithers);
            Assert.That(actual.IsRight, Is.EqualTo(expectedIsRight));
            if (expectedIsRight)
            {
                Assert.That(actual.Right, Is.EqualTo(new Unit()));
            }
            else
            {
                Assert.That(actual.Left, Is.EqualTo(expectedLeft));
            }
        }

        [Test]
        public void MapMWithFuncReturningRights()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            var actual = Either.MapM(n => Either<string>.Right(Convert.ToString(n)), ints);
            Assert.That(actual.IsRight, Is.True);
            Assert.That(actual.Right, Is.EqualTo(new[] { "1", "2", "3", "4", "5" }));
        }

        [Test]
        public void MapMWithFuncReturningMixtureOfLeftsAndRights()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            var actual = Either.MapM(n => n < 4 ? Either<string>.Right(Convert.ToString(n)) : Either<string>.Left<string>("error"), ints);
            Assert.That(actual.IsLeft, Is.True);
            Assert.That(actual.Left, Is.EqualTo("error"));
        }

        [Test]
        public void MapM_WithFuncReturningRights()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            var actual = Either.MapM_(n => Either<string>.Right(Convert.ToString(n)), ints);
            Assert.That(actual.IsRight, Is.True);
            Assert.That(actual.Right, Is.EqualTo(new Unit()));
        }

        [Test]
        public void MapM_WithFuncReturningMixtureOfLeftsAndRights()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            var actual = Either.MapM_(n => n < 4 ? Either<string>.Right(Convert.ToString(n)) : Either<string>.Left<string>("error"), ints);
            Assert.That(actual.IsLeft, Is.True);
            Assert.That(actual.Left, Is.EqualTo("error"));
        }

        [Test]
        public void ReplicateMAppliedToLeft()
        {
            var actual = Either.ReplicateM(5, Either<string>.Left<int>("error"));
            Assert.That(actual.IsLeft, Is.True);
            Assert.That(actual.Left, Is.EqualTo("error"));
        }

        [Test]
        public void ReplicateMAppliedToRight()
        {
            var actual = Either.ReplicateM(5, Either<string>.Right(42));
            Assert.That(actual.IsRight, Is.True);
            Assert.That(actual.Right, Is.EqualTo(new[] { 42, 42, 42, 42, 42 }));
        }

        [Test]
        public void ReplicateM_AppliedToLeft()
        {
            var actual = Either.ReplicateM_(5, Either<string>.Left<int>("error"));
            Assert.That(actual.IsLeft, Is.True);
            Assert.That(actual.Left, Is.EqualTo("error"));
        }

        [Test]
        public void ReplicateM_AppliedToRight()
        {
            var actual = Either.ReplicateM_(5, Either<string>.Right(42));
            Assert.That(actual.IsRight, Is.True);
            Assert.That(actual.Right, Is.EqualTo(new Unit()));
        }

        [Test]
        public void JoinAppliedToLeft()
        {
            var actual = Either.Join(Either<string>.Left<Either<string, int>>("error"));
            Assert.That(actual.IsLeft, Is.True);
            Assert.That(actual.Left, Is.EqualTo("error"));
        }

        [Test]
        public void JoinAppliedToRightOfRight()
        {
            var actual = Either.Join(Either<string>.Right(Either<string>.Right(42)));
            Assert.That(actual.IsRight, Is.True);
            Assert.That(actual.Right, Is.EqualTo(42));
        }

        [Test]
        public void JoinAppliedToRightOfLeft()
        {
            var actual = Either.Join(Either<string>.Right(Either<string>.Left<int>("error")));
            Assert.That(actual.IsLeft, Is.True);
            Assert.That(actual.Left, Is.EqualTo("error"));
        }

        // ReSharper disable UnusedMethodReturnValue.Local

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftMTests()
        {
            yield return new TestCaseData(Either<string>.Right(1), true, null, 1).SetName("1 Right");
            yield return new TestCaseData(Either<string>.Left<int>("error"), false, "error", default(int)).SetName("1 Left");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM2Tests()
        {
            yield return new TestCaseData(Either<string>.Right(1), Either<string>.Right(2), true, null, 3).SetName("2 Rights");
            yield return new TestCaseData(Either<string>.Right(1), Either<string>.Left<int>("error"), false, "error", default(int)).SetName("1 Right and 1 Left");
            yield return new TestCaseData(Either<string>.Left<int>("error 1"), Either<string>.Left<int>("error 2"), false, "error 1", default(int)).SetName("2 Lefts");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM3Tests()
        {
            yield return new TestCaseData(Either<string>.Right(1), Either<string>.Right(2), Either<string>.Right(3), true, null, 6).SetName("3 Rights");
            yield return new TestCaseData(Either<string>.Right(1), Either<string>.Right(2), Either<string>.Left<int>("error"), false, "error", default(int)).SetName("2 Rights and 1 Left");
            yield return new TestCaseData(Either<string>.Left<int>("error 1"), Either<string>.Left<int>("error 2"), Either<string>.Left<int>("error 3"), false, "error 1", default(int)).SetName("3 Lefts");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM4Tests()
        {
            yield return new TestCaseData(Either<string>.Right(1), Either<string>.Right(2), Either<string>.Right(3), Either<string>.Right(4), true, null, 10).SetName("4 Rights");
            yield return new TestCaseData(Either<string>.Right(1), Either<string>.Right(2), Either<string>.Right(3), Either<string>.Left<int>("error"), false, "error", default(int)).SetName("3 Rights and 1 Left");
            yield return new TestCaseData(Either<string>.Left<int>("error 1"), Either<string>.Left<int>("error 2"), Either<string>.Left<int>("error 3"), Either<string>.Left<int>("error 4"), false, "error 1", default(int)).SetName("4 Lefts");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM5Tests()
        {
            yield return new TestCaseData(Either<string>.Right(1), Either<string>.Right(2), Either<string>.Right(3), Either<string>.Right(4), Either<string>.Right(5), true, null, 15).SetName("5 Rights");
            yield return new TestCaseData(Either<string>.Right(1), Either<string>.Right(2), Either<string>.Right(3), Either<string>.Right(4), Either<string>.Left<int>("error"), false, "error", default(int)).SetName("4 Rights and 1 Left");
            yield return new TestCaseData(Either<string>.Left<int>("error 1"), Either<string>.Left<int>("error 2"), Either<string>.Left<int>("error 3"), Either<string>.Left<int>("error 4"), Either<string>.Left<int>("error 5"), false, "error 1", default(int)).SetName("5 Lefts");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForSequenceTests()
        {
            yield return new TestCaseData(
                new[]
                    {
                        Either<string>.Right(1),
                        Either<string>.Right(2),
                        Either<string>.Right(3),
                        Either<string>.Right(4)
                    },
                true,
                null as string,
                new[] {1, 2, 3, 4}).SetName("4 Rights");

            yield return new TestCaseData(
                new[]
                    {
                        Either<string>.Right(1),
                        Either<string>.Right(2),
                        Either<string>.Left<int>("error"),
                        Either<string>.Right(4)
                    },
                false,
                "error",
                null as int[]).SetName("3 Rights and 1 Left");

            yield return new TestCaseData(
                new Either<string, int>[] {},
                true,
                null as string,
                new int[] {}).SetName("Empty list of eithers");
        }
    }
}
