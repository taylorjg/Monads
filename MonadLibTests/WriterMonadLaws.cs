using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    using MyWriter = Writer<ListMonoid<string>, string>;
    using MyWriterInt = Writer<ListMonoid<string>, string, int>;
    using MyWriterDouble = Writer<ListMonoid<string>, string, double>;
    using MyWriterString = Writer<ListMonoid<string>, string, string>;

    [TestFixture]
    internal class WriterMonadLaws
    {
        [Test]
        public void BindLeftIdentity()
        {
            // (return x) >>= f == f x
            const int x = 5;
            Func<int, MyWriterString> f = n => MyWriter.Return(Convert.ToString(n * n));
            var writerMonad1 = MyWriter.Return(x).Bind(f);
            var writerMonad2 = f(x);
            var pair1 = writerMonad1.RunWriter;
            var pair2 = writerMonad2.RunWriter;
            Assert.That(pair1.Item1, Is.EqualTo(pair2.Item1));
            Assert.That(pair1.Item2.List, Is.EqualTo(pair2.Item2.List));
        }

        [Test]
        public void BindRightIdentity()
        {
            // m >>= return == m
            var m = MyWriter.Return(10);
            var writerMonad1 = m.Bind(MyWriter.Return);
            var writerMonad2 = m;
            var pair1 = writerMonad1.RunWriter;
            var pair2 = writerMonad2.RunWriter;
            Assert.That(pair1.Item1, Is.EqualTo(pair2.Item1));
            Assert.That(pair1.Item2.List, Is.EqualTo(pair2.Item2.List));
        }

        [Test]
        public void BindAssociativity()
        {
            // (m >>= f) >>= g == m >>= (\x -> f x >>= g)
            Func<int, MyWriterDouble> f = n => MyWriter.Return(Convert.ToDouble(n * n + 0.1m));
            Func<double, MyWriterString> g = d => MyWriter.Return(Convert.ToString(d));
            var m = MyWriter.Return(5);
            var writerMonad1 = m.Bind(f).Bind(g);
            var writerMonad2 = m.Bind(x => f(x).Bind(g));
            var pair1 = writerMonad1.RunWriter;
            var pair2 = writerMonad2.RunWriter;
            Assert.That(pair1.Item1, Is.EqualTo(pair2.Item1));
            Assert.That(pair1.Item2.List, Is.EqualTo(pair2.Item2.List));
        }
    }
}
