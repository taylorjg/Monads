#pragma warning disable 168

using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class EitherTests
    {
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
            Assert.Throws<InvalidOperationException>(() => { var dummy = either.Left; });
        }

        [Test]
        public void RightAppliedToLeftThrowsException()
        {
            var either = Either<string>.Left<int>("error");
            Assert.Throws<InvalidOperationException>(() => { var dummy = either.Right; });
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

        //[Test]
        //public void MatchOfTAppliedToLeft()
        //{
        //}

        //[Test]
        //public void MatchOfTAppliedToRight()
        //{
        //}

        // TODO: add tests re monadic behaviour:
        // Either.Unit
        // Either.Bind x 1 with left/right
        // Either.Bind x 2 with left/right combinations
        // Either.Unit => Either.Bind x 2 with left/right combinations
        // Either.LiftM2 with left/right
        // Either.LiftM3 with left/right

        [Test]
        public void LiftMAppliedToLeft()
        {
            var either = Either<string>.Left<int>("error").LiftM(a => a * a > 50);
            Assert.That(either.IsLeft, Is.True);
            Assert.That(either.IsRight, Is.False);
            Assert.That(either.Left, Is.EqualTo("error"));
        }

        [Test]
        public void LiftMAppliedToRight()
        {
            var either = Either<string>.Right(10).LiftM(a => a * a > 50);
            Assert.That(either.IsLeft, Is.False);
            Assert.That(either.IsRight, Is.True);
            Assert.That(either.Right, Is.True);
        }

        [Test]
        [Ignore("I need to fix a design flaw in order for this to work!")]
        public void SequenceAppliedToEmptyCollection()
        {
            var eithers = new Either<string, int>[] {};
            var actual = Either.Sequence(eithers);
            Assert.That(actual.IsRight, Is.True);
            Assert.That(actual.Right, Is.EqualTo(new int[] {}));
        }

        [Test]
        public void SequenceAppliedToRights()
        {
            var eithers = new[]
                {
                    Either<string>.Right(1),
                    Either<string>.Right(2),
                    Either<string>.Right(3),
                    Either<string>.Right(4)
                };
            var actual = Either.Sequence(eithers);
            Assert.That(actual.IsRight, Is.True);
            Assert.That(actual.Right, Is.EqualTo(new[] {1, 2, 3, 4}));
        }

        [Test]
        public void SequenceAppliedToMixtureOfLeftsAndRights()
        {
            var eithers = new[]
                {
                    Either<string>.Right(1),
                    Either<string>.Right(2),
                    Either<string>.Left<int>("Error 1"),
                    Either<string>.Right(4),
                    Either<string>.Left<int>("Error 2"),
                    Either<string>.Right(6)
                };
            var actual = Either.Sequence(eithers);
            Assert.That(actual.IsLeft, Is.True);
            Assert.That(actual.Left, Is.EqualTo("Error 1"));
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
    }
}
