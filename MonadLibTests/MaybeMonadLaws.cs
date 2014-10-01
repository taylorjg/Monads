using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class MaybeMonadLaws
    {
        [Test]
        public void BindLeftIdentity()
        {
            // (return x) >>= f == f x
            const int x = 5;
            Func<int, Maybe<int>> f = n => Maybe.Just(n * n);
            Assert.That(Maybe.Return(x).Bind(f), Is.EqualTo(f(x)));
        }

        [Test]
        public void BindRightIdentity()
        {
            // m >>= return == m
            var m = Maybe.Return(5);
            Assert.That(m.Bind(Maybe.Return), Is.EqualTo(m));
        }

        [Test]
        public void BindAssociativity()
        {
            // (m >>= f) >>= g == m >>= (\x -> f x >>= g)
            Func<int, Maybe<int>> f = n => Maybe.Just(n * n);
            Func<int, Maybe<string>> g = n => Maybe.Just(Convert.ToString(n));
            var m = Maybe.Return(5);
            Assert.That(m.Bind(f).Bind(g), Is.EqualTo(m.Bind(x => f(x).Bind(g))));
        }
    }
}
