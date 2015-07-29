using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests.EitherTests
{
    using EitherString = Either<string>;

    [TestFixture]
    internal class MonadLaws
    {
        [Test]
        public void BindLeftIdentity()
        {
            // (return x) >>= f == f x
            const int x = 5;
            Func<int, Either<string, int>> f = n => EitherString.Right(n * n);
            Assert.That(EitherString.Return(x).Bind(f), Is.EqualTo(f(x)));
        }

        [Test]
        public void BindRightIdentity()
        {
            // m >>= return == m
            var m = EitherString.Return(5);
            Assert.That(m.Bind(EitherString.Return), Is.EqualTo(m));
        }

        [Test]
        public void BindAssociativity()
        {
            // (m >>= f) >>= g == m >>= (\x -> f x >>= g)
            Func<int, Either<string, int>> f = n => EitherString.Right(n * n);
            Func<int, Either<string, string>> g = n => EitherString.Right(Convert.ToString(n));
            var m = EitherString.Return(5);
            Assert.That(m.Bind(f).Bind(g), Is.EqualTo(m.Bind(x => f(x).Bind(g))));
        }
    }
}
