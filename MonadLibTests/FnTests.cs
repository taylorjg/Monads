using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    public class FnTests
    {
        [Test]
        public void Const()
        {
            var actual = Fn.Const("Hello", 1);
            Assert.That(actual, Is.EqualTo("Hello"));
        }

        [Test]
        public void ConstLazyUsingFunc()
        {
            var lazyFuncCalled = false;
            Func<int> lazy = () =>
                {
                    lazyFuncCalled = true;
                    return 1;
                };

            var actual = Fn.ConstLazy("Hello", lazy);
            Assert.That(actual, Is.EqualTo("Hello"));
            Assert.That(lazyFuncCalled, Is.False);
        }

        [Test]
        public void ConstLazyUsingLazyT()
        {
            var lazyFuncCalled = false;
            var lazy = new Lazy<int>(() =>
                {
                    lazyFuncCalled = true;
                    return 1;
                });

            var actual = Fn.ConstLazy("Hello", lazy);
            Assert.That(actual, Is.EqualTo("Hello"));
            Assert.That(lazyFuncCalled, Is.False);
        }

        [Test]
        public void PartiallyApplyStaticFunctionWithArity2()
        {
            var partiallyAppliedFn = Fn.PartiallyApply<int, uint, string>(StaticFunctionWithArity2, 1);
            var actual = partiallyAppliedFn(2);
            Assert.That(actual, Is.EqualTo("3"));
        }

        [Test]
        public void PartiallyApplyFuncWithArity2()
        {
            var partiallyAppliedFn = Fn.PartiallyApply(FuncWithArity2, 1);
            var actual = partiallyAppliedFn(2);
            Assert.That(actual, Is.EqualTo("3"));
        }

        [Test]
        public void PartiallyApplyLambdaWithArity2()
        {
            var partiallyAppliedFn = Fn.PartiallyApply((int a, uint b) => string.Format("{0}", a + b), 1);
            var actual = partiallyAppliedFn(2);
            Assert.That(actual, Is.EqualTo("3"));
        }

        [Test]
        public void CurryFunctionWithArity2()
        {
            const int a = 1;
            const uint b = 2u;
            var cf = Fn.Curry(FuncWithArity2);
            var actual1 = FuncWithArity2(a, b);
            var actual2 = cf(a)(b);
            Assert.That(actual1, Is.EqualTo(actual2));
        }

        [Test]
        public void CurryFunctionWithArity3()
        {
            const int a = 1;
            const uint b = 2u;
            const long c = 3L;
            var cf = Fn.Curry(FuncWithArity3);
            var actual1 = FuncWithArity3(a, b, c);
            var actual2 = cf(a)(b)(c);
            Assert.That(actual1, Is.EqualTo(actual2));
        }

        [Test]
        public void CurryFunctionWithArity4()
        {
            const int a = 1;
            const uint b = 2u;
            const long c = 3L;
            const float d = 4f;
            var cf = Fn.Curry(FuncWithArity4);
            var actual1 = FuncWithArity4(a, b, c, d);
            var actual2 = cf(a)(b)(c)(d);
            Assert.That(actual1, Is.EqualTo(actual2));
        }

        [Test]
        public void CurryFunctionWithArity5()
        {
            const int a = 1;
            const uint b = 2u;
            const long c = 3L;
            const float d = 4f;
            const double e = 5d;
            var cf = Fn.Curry(FuncWithArity5);
            var actual1 = FuncWithArity5(a, b, c, d, e);
            var actual2 = cf(a)(b)(c)(d)(e);
            Assert.That(actual1, Is.EqualTo(actual2));
        }

        private static readonly Func<int, uint, string> FuncWithArity2 = (a, b) => string.Format("{0}", a + b);
        private static readonly Func<int, uint, long, string> FuncWithArity3 = (a, b, c) => string.Format("{0}", a + b + c);
        private static readonly Func<int, uint, long, float, string> FuncWithArity4 = (a, b, c, d) => string.Format("{0}", a + b + c + d);
        private static readonly Func<int, uint, long, float, double, string> FuncWithArity5 = (a, b, c, d, e) => string.Format("{0}", a + b + c + d + e);

        private static string StaticFunctionWithArity2(int a, uint b)
        {
            return string.Format("{0}", a + b);
        }
    }
}
