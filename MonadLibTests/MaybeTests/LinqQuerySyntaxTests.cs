using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests.MaybeTests
{
    using WriterString = Writer<ListMonoid<string>, string>;

    [TestFixture]
    internal class LinqQuerySyntaxTests
    {
        [Test]
        public void MaybeSelectLinqQuerySyntax()
        {
            var m = Maybe.Just(42);
            var mr =
                from x in m
                select Convert.ToString(x);
            Assert.That(mr, Is.EqualTo(Maybe.Just("42")));
        }

        [Test]
        public void MaybeSelectManyLinqQuerySyntax()
        {
            var m1 = Maybe.Just(42);
            var m2 = Maybe.Just(2);
            var mr =
                from x in m1
                from y in m2
                select x * y;
            Assert.That(mr, Is.EqualTo(Maybe.Just(42 * 2)));
        }

        [Test]
        public void MaybeSelectManyTwiceLinqQuerySyntax()
        {
            var m1 = Maybe.Just(42);
            var m2 = Maybe.Just(2);
            var m3 = Maybe.Just(3);
            var mr =
                from x in m1
                from y in m2
                from z in m3
                select x * y * z;
            Assert.That(mr, Is.EqualTo(Maybe.Just(42 * 2 * 3)));
        }
    }
}
