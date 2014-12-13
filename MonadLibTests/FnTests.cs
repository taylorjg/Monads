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
            var partiallyAppliedFn = Fn.PartiallyApply<string, int, string>(StaticFunctionWithArity2, "Hello");
            var actual = partiallyAppliedFn(1);
            Assert.That(actual, Is.EqualTo("Hello-1"));
        }

        [Test]
        public void PartiallyApplyFuncWithArity2()
        {
            var partiallyAppliedFn = Fn.PartiallyApply(FuncWithArity2, "Hello");
            var actual = partiallyAppliedFn(1);
            Assert.That(actual, Is.EqualTo("Hello-1"));
        }

        [Test]
        public void PartiallyApplyLambdaWithArity2()
        {
            var partiallyAppliedFn = Fn.PartiallyApply((string s, int n) => string.Format("{0}-{1}", s, n), "Hello");
            var actual = partiallyAppliedFn(1);
            Assert.That(actual, Is.EqualTo("Hello-1"));
        }

        private static readonly Func<string, int, string> FuncWithArity2 = (s, n) => string.Format("{0}-{1}", s, n);

        private static string StaticFunctionWithArity2(string s, int n)
        {
            return string.Format("{0}-{1}", s, n);
        }
    }
}
