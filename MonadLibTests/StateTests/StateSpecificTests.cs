using MonadLib;
using NUnit.Framework;

namespace MonadLibTests.StateTests
{
    [TestFixture]
    internal class StateSpecificTests
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
            var a = actual.Item1;
            var s = actual.Item2;
            Assert.That(a, Is.EqualTo("state"));
            Assert.That(s, Is.EqualTo("state"));
        }

        [Test]
        public void Put()
        {
            var actual = State<string>.Put("state2").RunState("state1");
            var a = actual.Item1;
            var s = actual.Item2;
            Assert.That(a, Is.EqualTo(new Unit()));
            Assert.That(s, Is.EqualTo("state2"));
        }

        [Test]
        public void Modify()
        {
            var actual = State<int>.Modify(n => n + 1).RunState(10);
            var a = actual.Item1;
            var s = actual.Item2;
            Assert.That(a, Is.EqualTo(new Unit()));
            Assert.That(s, Is.EqualTo(10 + 1));
        }

        private class MyState
        {
            public int Property1 { get; set; }
            public string Property2 { get; set; }
        }

        [Test]
        public void Gets()
        {
            var initialState = new MyState
                {
                    Property1 = 42,
                    Property2 = "Hello"
                };

            var stateMonad1 = State<MyState>.Return(10.1);
            var stateMonad2 = stateMonad1.Gets(state => state.Property1);
            var stateMonad3 = stateMonad1.Gets(state => state.Property2);

            var newState1 = stateMonad2.RunState(initialState);
            var a1 = newState1.Item1;
            var s1 = newState1.Item2;
            Assert.That(a1, Is.EqualTo(initialState.Property1));
            Assert.That(s1, Is.SameAs(initialState));

            var newState2 = stateMonad3.RunState(initialState);
            var a2 = newState2.Item1;
            var s2 = newState2.Item2;
            Assert.That(a2, Is.EqualTo(initialState.Property2));
            Assert.That(s2, Is.SameAs(initialState));
        }
    }
}
