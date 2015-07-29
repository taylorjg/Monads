using System;
using System.Collections.Generic;
using System.Linq;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests.EitherTests
{
    // ReSharper disable InconsistentNaming

    using EitherString = Either<string>;

    [TestFixture]
    internal class EitherMonadTests
    {
        [Test, TestCaseSource("TestCaseSourceForLiftMTests")]
        public void LiftM(Either<string, int>[] eithers, bool expectedIsRight, string expectedLeft, int expectedRight)
        {
            var ma = eithers[0];
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
        public void LiftM2(Either<string, int>[] eithers, bool expectedIsRight, string expectedLeft, int expectedRight)
        {
            var ma = eithers[0];
            var mb = eithers[1];
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
        public void LiftM3(Either<string, int>[] eithers, bool expectedIsRight, string expectedLeft, int expectedRight)
        {
            var ma = eithers[0];
            var mb = eithers[1];
            var mc = eithers[2];
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
        public void LiftM4(Either<string, int>[] eithers, bool expectedIsRight, string expectedLeft, int expectedRight)
        {
            var ma = eithers[0];
            var mb = eithers[1];
            var mc = eithers[2];
            var md = eithers[3];
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
        public void LiftM5(Either<string, int>[] eithers, bool expectedIsRight, string expectedLeft, int expectedRight)
        {
            var ma = eithers[0];
            var mb = eithers[1];
            var mc = eithers[2];
            var md = eithers[3];
            var me = eithers[4];
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
            var actual = Either.MapM(n => EitherString.Right(Convert.ToString(n)), ints);
            Assert.That(actual.IsRight, Is.True);
            Assert.That(actual.Right, Is.EqualTo(new[] { "1", "2", "3", "4", "5" }));
        }

        [Test]
        public void MapMWithFuncReturningMixtureOfLeftsAndRights()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            var actual = Either.MapM(n => n < 4 ? EitherString.Right(Convert.ToString(n)) : EitherString.Left<string>("error"), ints);
            Assert.That(actual.IsLeft, Is.True);
            Assert.That(actual.Left, Is.EqualTo("error"));
        }

        [Test]
        public void MapM_WithFuncReturningRights()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            var actual = Either.MapM_(n => EitherString.Right(Convert.ToString(n)), ints);
            Assert.That(actual.IsRight, Is.True);
            Assert.That(actual.Right, Is.EqualTo(new Unit()));
        }

        [Test]
        public void MapM_WithFuncReturningMixtureOfLeftsAndRights()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            var actual = Either.MapM_(n => n < 4 ? EitherString.Right(Convert.ToString(n)) : EitherString.Left<string>("error"), ints);
            Assert.That(actual.IsLeft, Is.True);
            Assert.That(actual.Left, Is.EqualTo("error"));
        }

        [Test]
        public void ReplicateMAppliedToLeft()
        {
            var actual = Either.ReplicateM(5, EitherString.Left<int>("error"));
            Assert.That(actual.IsLeft, Is.True);
            Assert.That(actual.Left, Is.EqualTo("error"));
        }

        [Test]
        public void ReplicateMAppliedToRight()
        {
            var actual = Either.ReplicateM(5, EitherString.Right(42));
            Assert.That(actual.IsRight, Is.True);
            Assert.That(actual.Right, Is.EqualTo(new[] { 42, 42, 42, 42, 42 }));
        }

        [Test]
        public void ReplicateM_AppliedToLeft()
        {
            var actual = Either.ReplicateM_(5, EitherString.Left<int>("error"));
            Assert.That(actual.IsLeft, Is.True);
            Assert.That(actual.Left, Is.EqualTo("error"));
        }

        [Test]
        public void ReplicateM_AppliedToRight()
        {
            var actual = Either.ReplicateM_(5, EitherString.Right(42));
            Assert.That(actual.IsRight, Is.True);
            Assert.That(actual.Right, Is.EqualTo(new Unit()));
        }

        [Test]
        public void JoinAppliedToLeft()
        {
            var actual = Either.Join(EitherString.Left<Either<string, int>>("error"));
            Assert.That(actual.IsLeft, Is.True);
            Assert.That(actual.Left, Is.EqualTo("error"));
        }

        [Test]
        public void JoinAppliedToRightOfRight()
        {
            var actual = Either.Join(EitherString.Right(EitherString.Right(42)));
            Assert.That(actual.IsRight, Is.True);
            Assert.That(actual.Right, Is.EqualTo(42));
        }

        [Test]
        public void JoinAppliedToRightOfLeft()
        {
            var actual = Either.Join(EitherString.Right(EitherString.Left<int>("error")));
            Assert.That(actual.IsLeft, Is.True);
            Assert.That(actual.Left, Is.EqualTo("error"));
        }

        // ReSharper disable UnusedMethodReturnValue.Local

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftMTests()
        {
            yield return MakeLiftTestCaseData(EorN.N(1)).SetName("1 Right");
            yield return MakeLiftTestCaseData(EorN.E("error 1")).SetName("1 Left");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM2Tests()
        {
            yield return MakeLiftTestCaseData(EorN.N(1), EorN.N(2)).SetName("2 Rights");
            yield return MakeLiftTestCaseData(EorN.N(1), EorN.E("error")).SetName("1 Right and 1 Left");
            yield return MakeLiftTestCaseData(EorN.E("error 1"), EorN.E("error 2")).SetName("2 Lefts");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM3Tests()
        {
            yield return MakeLiftTestCaseData(EorN.N(1), EorN.N(2), EorN.N(3)).SetName("3 Rights");
            yield return MakeLiftTestCaseData(EorN.N(1), EorN.N(2), EorN.E("error")).SetName("2 Rights and 1 Left");
            yield return MakeLiftTestCaseData(EorN.E("error 1"), EorN.E("error 2"), EorN.E("error 3")).SetName("3 Lefts");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM4Tests()
        {
            yield return MakeLiftTestCaseData(EorN.N(1), EorN.N(2), EorN.N(3), EorN.N(4)).SetName("4 Rights");
            yield return MakeLiftTestCaseData(EorN.N(1), EorN.N(2), EorN.N(3), EorN.E("error")).SetName("3 Rights and 1 Left");
            yield return MakeLiftTestCaseData(EorN.E("error 1"), EorN.E("error 2"), EorN.E("error 3"), EorN.E("error 4")).SetName("4 Lefts");
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForLiftM5Tests()
        {
            yield return MakeLiftTestCaseData(EorN.N(1), EorN.N(2), EorN.N(3), EorN.N(4), EorN.N(5)).SetName("5 Rights");
            yield return MakeLiftTestCaseData(EorN.N(1), EorN.N(2), EorN.N(3), EorN.N(4), EorN.E("error")).SetName("4 Rights and 1 Left");
            yield return MakeLiftTestCaseData(EorN.E("error 1"), EorN.E("error 2"), EorN.E("error 3"), EorN.E("error 4"), EorN.E("error 5")).SetName("5 Lefts");
        }

        private static TestCaseData MakeLiftTestCaseData(params EorN[] ens)
        {
            var ms = ens.Select(en => en.ToEither()).ToArray();
            var expectedIsRight = ms.All(m => m.IsRight);
            var expectedLeft = !expectedIsRight ? Either.Lefts(ms).First() : null;
            var expectedRight = (expectedIsRight) ? Either.Rights(ms).Sum() : default(int);
            return new TestCaseData(ms, expectedIsRight, expectedLeft, expectedRight);
        }

        private static IEnumerable<ITestCaseData> TestCaseSourceForSequenceTests()
        {
            yield return MakeSequenceTestCaseData(EorN.N(1), EorN.N(2), EorN.N(3), EorN.N(4)).SetName("4 Rights");
            yield return MakeSequenceTestCaseData(EorN.N(1), EorN.N(2), EorN.E("error"), EorN.N(4)).SetName("3 Rights and 1 Left");
            yield return MakeSequenceTestCaseData().SetName("Empty list of eithers");
        }

        private static TestCaseData MakeSequenceTestCaseData(params EorN[] ens)
        {
            var ms = ens.Select(en => en.ToEither()).ToArray();
            var expectedIsRight = ms.All(m => m.IsRight);
            var expectedLeft = !expectedIsRight ? Either.Lefts(ms).First() : null;
            var expectedRight = expectedIsRight ? Either.Rights(ms).ToArray() : null;
            return new TestCaseData(ms, expectedIsRight, expectedLeft, expectedRight);
        }

        private class EorN
        {
            private EorN(string error)
            {
                _error = error;
            }

            private EorN(int number)
            {
                _number = number;
            }

            public static EorN N(int n)
            {
                return new EorN(n);
            }

            public static EorN E(string error)
            {
                return new EorN(error);
            }

            public Either<string, int> ToEither()
            {
                return _error != null ? EitherString.Left<int>(_error) : EitherString.Right(_number);
            }

            private readonly int _number;
            private readonly string _error;
        }
    }
}
