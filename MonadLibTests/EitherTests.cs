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
        public void LeftOfRightThrowsException()
        {
            var either = Either<string>.Right(42);
            Assert.Throws<InvalidOperationException>(() => { var dummy = either.Left; });
        }

        [Test]
        public void RightOfLeftThrowsException()
        {
            var either = Either<string>.Left<int>("error");
            Assert.Throws<InvalidOperationException>(() => { var dummy = either.Right; });
        }
    }
}
