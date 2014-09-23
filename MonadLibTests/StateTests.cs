using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class StateTests
    {
        [Test]
        public void EvalState()
        {
            var actual = State<string>.Return(5).EvalState("state");
            Assert.That(actual, Is.EqualTo(5));
        }

        [Test]
        public void ExecState()
        {
            var actual = State<string>.Return(5).ExecState("state");
            Assert.That(actual, Is.EqualTo("state"));
        }

        [Test]
        public void Get()
        {
            var actual = State<string>.Get().RunState("state");
            Assert.That(actual.Item1, Is.EqualTo("state"));
            Assert.That(actual.Item2, Is.EqualTo("state"));
        }

        [Test]
        public void Put()
        {
            var actual = State<string>.Put("state2").RunState("state1");
            Assert.That(actual.Item1, Is.EqualTo(new Unit()));
            Assert.That(actual.Item2, Is.EqualTo("state2"));
        }

        [Test]
        public void Modify()
        {
            var actual = State<int>.Modify(n => n + 1).RunState(10);
            Assert.That(actual, Is.EqualTo(Tuple.Create(new Unit(), 11)));
        }
    }
}
