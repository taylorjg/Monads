using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class StateMonadLaws
    {
        [Test]
        public void BindLeftIdentity()
        {
            // (return x) >>= f == f x
            const int x = 5;
            const int y = 10;
            Func<int, State<int, string>> f = n => State<int>.Return(Convert.ToString(n * n));
            var actual1 = State<int>.Return(x).Bind(f);
            var actual2 = f(x);
            Assert.That(actual1.RunState(y), Is.EqualTo(actual2.RunState(y)));
        }

        [Test]
        [Ignore("Not implemented yet!")]
        public void BindRightIdentity()
        {
        }

        [Test]
        [Ignore("Not implemented yet!")]
        public void BindAssociativity()
        {
        }
    }
}
