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
    }
}
