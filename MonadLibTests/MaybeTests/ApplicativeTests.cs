using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests.MaybeTests
{
    [TestFixture]
    internal class ApplicativeTests
    {
        [Test]
        public void MaybeApplicativeApplyOfCurriedFuncWithArity2()
        {
            const int a = 1;
            const uint b = 2u;

            var fa = a.Just();
            var fb = b.Just();
            var fn = Fn.Curry(FuncWithArity2);

            var actual = fn.Pure().Apply(fa).Apply(fb);
            
            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(Convert.ToString(a + b)));
        }

        [Test]
        public void MaybeApplicativeApplyOfCurriedFuncWithArity5()
        {
            const int a = 1;
            const uint b = 2u;
            const long c = 3L;
            const float d = 4f;
            const double e = 5d;

            var fa = a.Just();
            var fb = b.Just();
            var fc = c.Just();
            var fd = d.Just();
            var fe = e.Just();
            var fn = Fn.Curry(FuncWithArity5);
        
            var actual = fn.Pure().Apply(fa).Apply(fb).Apply(fc).Apply(fd).Apply(fe);

            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(Convert.ToString(a + b + c + d + e)));
        }

        [Test]
        public void MaybeApplicativeApplyOfCurriedFuncWithArity3OfPartiallyAppliedFuncWithArity5()
        {
            const int a = 1;
            const uint b = 2u;
            const long c = 3L;
            const float d = 4f;
            const double e = 5d;

            var fc = c.Just();
            var fd = d.Just();
            var fe = e.Just();
            var fn = Fn.Curry(Fn.PartiallyApply(FuncWithArity5, a, b));
        
            var actual = fn.Pure().Apply(fc).Apply(fd).Apply(fe);

            Assert.That(actual.IsJust, Is.True);
            Assert.That(actual.FromJust, Is.EqualTo(Convert.ToString(a + b + c + d + e)));
        }

        private static readonly Func<int, uint, string> FuncWithArity2 = (a, b) => string.Format("{0}", a + b);
        private static readonly Func<int, uint, long, float, double, string> FuncWithArity5 = (a, b, c, d, e) => string.Format("{0}", a + b + c + d + e);
    }
}
