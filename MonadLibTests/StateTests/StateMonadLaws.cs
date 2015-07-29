using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests.StateTests
{
    [TestFixture]
    internal class StateMonadLaws
    {
        [Test]
        public void BindLeftIdentity()
        {
            // (return x) >>= f == f x
            const double x = 5;
            Func<double, State<int, string>> f = d => State<int>.Return(Convert.ToString(d * d));
            var stateMonad1 = State<int>.Return(x).Bind(f);
            var stateMonad2 = f(x);
            const int s = 10;
            var pair1 = stateMonad1.RunState(s);
            var pair2 = stateMonad2.RunState(s);
            Assert.That(pair1, Is.EqualTo(pair2));
        }

        [Test]
        public void BindRightIdentity()
        {
            // m >>= return == m
            var m = State<int>.Return('X');
            var stateMonad1 = m.Bind(State<int>.Return);
            var stateMonad2 = m;
            const int s = 10;
            var pair1 = stateMonad1.RunState(s);
            var pair2 = stateMonad2.RunState(s);
            Assert.That(pair1, Is.EqualTo(pair2));
        }

        [Test]
        public void BindAssociativity()
        {
            // (m >>= f) >>= g == m >>= (\x -> f x >>= g)
            Func<int, State<int, double>> f = n => State<int>.Return(Convert.ToDouble(n * n + 0.1m));
            Func<double, State<int, string>> g = d => State<int>.Return(Convert.ToString(d));
            var m = State<int>.Return(5);
            var stateMonad1 = m.Bind(f).Bind(g);
            var stateMonad2 = m.Bind(x => f(x).Bind(g));
            const int s = 10;
            var pair1 = stateMonad1.RunState(s);
            var pair2 = stateMonad2.RunState(s);
            Assert.That(pair1, Is.EqualTo(pair2));
        }
    }
}
