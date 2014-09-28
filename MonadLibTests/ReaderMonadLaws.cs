using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class ReaderMonadLaws
    {
        [Test]
        public void BindLeftIdentity()
        {
            // (return x) >>= f == f x
            const double x = 5;
            Func<double, Reader<int, string>> f = d => Reader<int>.Return(Convert.ToString(d * d));
            var reader1 = Reader<int>.Return(x).Bind(f);
            var reader2 = f(x);
            const int r = 10;
            var a1 = reader1.RunReader(r);
            var a2 = reader2.RunReader(r);
            Assert.That(a1, Is.EqualTo(a2));
        }

        [Test]
        public void BindRightIdentity()
        {
            // m >>= return == m
            var m = Reader<int>.Return('X');
            var reader1 = m.Bind(Reader<int>.Return);
            var reader2 = m;
            const int r = 10;
            var a1 = reader1.RunReader(r);
            var a2 = reader2.RunReader(r);
            Assert.That(a1, Is.EqualTo(a2));
        }

        [Test]
        public void BindAssociativity()
        {
            // (m >>= f) >>= g == m >>= (\x -> f x >>= g)
            Func<int, Reader<int, double>> f = n => Reader<int>.Return(Convert.ToDouble(n * n + 0.1m));
            Func<double, Reader<int, string>> g = d => Reader<int>.Return(Convert.ToString(d));
            var m = Reader<int>.Return(5);
            var reader1 = m.Bind(f).Bind(g);
            var reader2 = m.Bind(x => f(x).Bind(g));
            const int r = 10;
            var a1 = reader1.RunReader(r);
            var a2 = reader2.RunReader(r);
            Assert.That(a1, Is.EqualTo(a2));
        }
    }
}
