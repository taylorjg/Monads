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
            var actual1 = Maybe.Return(x).Bind(f);
            var actual2 = f(x);
            Assert.That(actual1.IsJust, Is.True);
            Assert.That(actual2.IsJust, Is.True);
            Assert.That(actual1.FromJust, Is.EqualTo(actual2.FromJust));
        }

        [Test]
        public void BindRightIdentity()
        {
            // m >>= return == m
            var m = Maybe.Return(5);
            var actual1 = m.Bind(Maybe.Return);
            var actual2 = m;
            Assert.That(actual1.IsJust, Is.True);
            Assert.That(actual2.IsJust, Is.True);
            Assert.That(actual1.FromJust, Is.EqualTo(actual2.FromJust));
        }

        [Test]
        public void BindAssociativity()
        {
            // (m >>= f) >>= g == m >>= (\x -> f x >>= g)
            Func<int, Maybe<int>> f = n => Maybe.Just(n * n);
            Func<int, Maybe<string>> g = n => Maybe.Just(Convert.ToString(n));
            var m = Maybe.Return(5);
            var actual1 = m.Bind(f).Bind(g);
            var actual2 = m.Bind(x => f(x).Bind(g));
            Assert.That(actual1.IsJust, Is.True);
            Assert.That(actual2.IsJust, Is.True);
            Assert.That(actual1.FromJust, Is.EqualTo(actual2.FromJust));
        }
    }
}
