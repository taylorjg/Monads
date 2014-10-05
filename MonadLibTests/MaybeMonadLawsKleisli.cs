using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class MaybeMonadLawsKleisli
    {
        [Test]
        public void BindLeftIdentity()
        {
            // Compose(f, Return) == f
            Func<int, Maybe<int>> @return = Maybe.Return;
            Func<int, Maybe<int>> f = n => Maybe.Just(n * n);
            var composed = Maybe.Compose(f, @return);
            const int x = 5;
            Assert.That(composed(x), Is.EqualTo(f(x)));
        }

        [Test]
        public void BindRightIdentity()
        {
            // Compose(Return, f) == f
            Func<int, Maybe<int>> @return = Maybe.Return;
            Func<int, Maybe<int>> f = n => Maybe.Just(n * n);
            var composed = Maybe.Compose(@return, f);
            const int x = 5;
            Assert.That(composed(x), Is.EqualTo(f(x)));
        }

        [Test]
        public void BindAssociativity()
        {
            // Compose(Compose(f, g), h) == Compose(f, Compose(g, h))
            Func<int, Maybe<int>> f = n => Maybe.Just(n * n);
            Func<int, Maybe<double>> g = n => Maybe.Just(Convert.ToDouble(n));
            Func<double, Maybe<string>> h = d => Maybe.Just(Convert.ToString(d));
            var composed1 = Maybe.Compose(Maybe.Compose(f, g), h);
            var composed2 = Maybe.Compose(f, Maybe.Compose(g, h));
            const int x = 5;
            Assert.That(composed1(x), Is.EqualTo(composed2(x)));
        }
    }
}
