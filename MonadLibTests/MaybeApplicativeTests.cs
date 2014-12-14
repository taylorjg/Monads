using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class MaybeApplicativeTests
    {
        [Test]
        public void MaybeApplicativeApplyOfCurriedFuncWithArity2()
        {
            const int a = 1;
            const uint b = 2u;
            var ma = Maybe.Just(a);
            var mb = Maybe.Just(b);
            var mf = Maybe.Just(Fn.Curry(FuncWithArity2));

            var actual = mb.Apply(ma.Apply(mf));
            Assert.That(actual, Is.EqualTo(Maybe.Just(Convert.ToString(a + b))));
        }

        [Test]
        public void MaybeApplicativeApplyOfCurriedFuncWithArity5()
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
            var mf = Maybe.Just(Fn.Curry(FuncWithArity5));
        
            var actual = me.Apply(md.Apply(mc.Apply(mb.Apply(ma.Apply(mf)))));
            Assert.That(actual, Is.EqualTo(Maybe.Just(Convert.ToString(a + b + c + d + e))));
        }

        [Test]
        public void MaybeApplicativeApplyOfCurriedFuncWithArity3OfPartiallyAppliedFuncWithArity5()
        {
            const int a = 1;
            const uint b = 2u;
            const long c = 3L;
            const float d = 4f;
            const double e = 5d;
            var mc = Maybe.Just(c);
            var md = Maybe.Just(d);
            var me = Maybe.Just(e);
            var mf = Maybe.Just(Fn.Curry(Fn.PartiallyApply(FuncWithArity5, a, b)));
        
            var actual = me.Apply(md.Apply(mc.Apply(mf)));
            Assert.That(actual, Is.EqualTo(Maybe.Just(Convert.ToString(a + b + c + d + e))));
        }

        private static readonly Func<int, uint, string> FuncWithArity2 = (a, b) => string.Format("{0}", a + b);
        private static readonly Func<int, uint, long, float, double, string> FuncWithArity5 = (a, b, c, d, e) => string.Format("{0}", a + b + c + d + e);
    }
}
