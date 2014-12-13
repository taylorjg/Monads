using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class MaybeApplicativeTests
    {
        [Test]
        public void MaybeApplicativeApplyWithArity2()
        {
            var ma = Maybe.Just("Hello");
            var mb = Maybe.Just(1);
            var mf = Maybe.Just(FuncWithArity2);

            var actual = mb.Apply(ma.Apply(mf));
            Assert.That(actual, Is.EqualTo(Maybe.Just("Hello-1")));
        }

        [Test]
        public void MaybeApplicativeApplyWithArity5()
        {
            const int a = 1;
            const uint b = 2u;
            const long c = 3L;
            const float d = 4f;
            const double e = 5d;
            var ma = Maybe.Just(a);
            var mb = Maybe.Just(b);
            var mc = Maybe.Just(c);
            var md = Maybe.Just(d);
            var me = Maybe.Just(e);
            var mf = Maybe.Just(FuncWithArity5);

            var actual = me.Apply(md.Apply(mc.Apply(mb.Apply(ma.Apply(mf)))));
            Assert.That(actual, Is.EqualTo(Maybe.Just(Convert.ToString(a * b * c * d * e))));
        }

        [Test]
        public void MaybeApplicativeApplyOfPartialFunction()
        {
            const int a = 1;
            const uint b = 2u;
            const long c = 3L;
            const float d = 4f;
            const double e = 5d;
            var mc = Maybe.Just(c);
            var md = Maybe.Just(d);
            var me = Maybe.Just(e);
            var mpf = Maybe.Just(Fn.PartiallyApply(FuncWithArity5, a, b));
        
            var actual = me.Apply(md.Apply(mc.Apply(mpf)));
            Assert.That(actual, Is.EqualTo(Maybe.Just(Convert.ToString(a * b * c * d * e))));
        }

        private static readonly Func<string, int, string> FuncWithArity2 = (s, n) => string.Format("{0}-{1}", s, n);
        private static readonly Func<int, uint, long, float, double, string> FuncWithArity5 = (a, b, c, d, e) => string.Format("{0}", a * b * c * d * e);
    }
}
