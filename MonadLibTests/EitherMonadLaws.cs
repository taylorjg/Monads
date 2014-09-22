using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class EitherMonadLaws
    {
        [Test]
        public void BindLeftIdentity()
        {
            // (return x) >>= f == f x
            const int x = 5;
            Func<int, Either<string, int>> f = n => Either<string>.Right(n * n);
            var actual1 = Either<string>.Return(x).Bind(f);
            var actual2 = f(x);
            Assert.That(actual1.IsRight, Is.True);
            Assert.That(actual2.IsRight, Is.True);
            Assert.That(actual1.Right, Is.EqualTo(actual2.Right));
        }

        [Test]
        public void BindRightIdentity()
        {
            // m >>= return == m
            var m = Either<string>.Return(5);
            var actual1 = m.Bind(Either<string>.Return);
            var actual2 = m;
            Assert.That(actual1.IsRight, Is.True);
            Assert.That(actual2.IsRight, Is.True);
            Assert.That(actual1.Right, Is.EqualTo(actual2.Right));
        }

        [Test]
        public void BindAssociativity()
        {
            // (m >>= f) >>= g == m >>= (\x -> f x >>= g)
            Func<int, Either<string, int>> f = n => Either<string>.Right(n * n);
            Func<int, Either<string, string>> g = n => Either<string>.Right(Convert.ToString(n));
            var m = Either<string>.Return(5);
            var actual1 = m.Bind(f).Bind(g);
            var actual2 = m.Bind(x => f(x).Bind(g));
            Assert.That(actual1.IsRight, Is.True);
            Assert.That(actual2.IsRight, Is.True);
            Assert.That(actual1.Right, Is.EqualTo(actual2.Right));
        }
    }
}
