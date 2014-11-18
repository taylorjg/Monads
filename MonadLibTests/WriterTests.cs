using System;
using System.Linq;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    using MyWriter = Writer<ListMonoid<string>, ListMonoidAdapter<string>, string>;
    using MyWriterInt = Writer<ListMonoid<string>, ListMonoidAdapter<string>, string, int>;
    using MyWriterUnit = Writer<ListMonoid<string>, ListMonoidAdapter<string>, string, Unit>;

    [TestFixture]
    internal class WriterTests
    {
        [Test]
        public void Tell()
        {
            var writer = TellHelper("Message 1").BindIgnoringLeft(
                TellHelper("Message 2")).BindIgnoringLeft(
                    MyWriter.Return(42));

            var tuple = writer.RunWriter;
            var a = tuple.Item1;
            var w = tuple.Item2;

            Assert.That(a, Is.EqualTo(42));
            Assert.That(w.List, Is.EqualTo(new[]{"Message 1", "Message 2"}));
        }

        [Test]
        public void Listen()
        {
            var writer = TellHelper("Message 1").BindIgnoringLeft(
                TellHelper("Message 2")).BindIgnoringLeft(
                    MyWriter.Return(42)).Listen();

            var tuple = writer.RunWriter;
            var a = tuple.Item1.Item1;
            var w1 = tuple.Item1.Item2;
            var w2 = tuple.Item2;

            Assert.That(a, Is.EqualTo(42));
            Assert.That(w1.List, Is.EqualTo(w2.List));
            Assert.That(w1.List, Is.SameAs(w2.List));
            Assert.That(w1.List, Is.EqualTo(new[] { "Message 1", "Message 2" }));
            Assert.That(w2.List, Is.EqualTo(new[] { "Message 1", "Message 2" }));
        }

        [Test]
        public void Listens()
        {
            var writer = TellHelper("Message 1").BindIgnoringLeft(
                TellHelper("Message 2")).BindIgnoringLeft(
                    MyWriter.Return(42)).Listens(
                        msgs => String.Join(", ", msgs.List));

            var tuple = writer.RunWriter;
            var a = tuple.Item1.Item1;
            var b = tuple.Item1.Item2;
            var w = tuple.Item2;

            Assert.That(a, Is.EqualTo(42));
            Assert.That(b, Is.EqualTo("Message 1, Message 2"));
            Assert.That(w.List, Is.EqualTo(new[] { "Message 1", "Message 2" }));
        }

        [Test]
        public void Pass()
        {
            var writer1 = TellHelper("Message 1").BindIgnoringLeft(
                TellHelper("Message 2")).BindIgnoringLeft(
                    MyWriter.Return(42));

            Func<ListMonoid<string>, ListMonoid<string>> f = msgs => new ListMonoid<string>(msgs.List.Where(msg => msg.EndsWith("2")));
            var writer2 = MyWriter.Pass(writer1.Listen().Bind(t => MyWriter.Return(Tuple.Create(t.Item1, f))));

            var tuple = writer2.RunWriter;
            var a = tuple.Item1;
            var w = tuple.Item2;

            Assert.That(a, Is.EqualTo(42));
            Assert.That(w.List, Is.EqualTo(new[] { "Message 2" }));
        }

        [Test]
        public void Censor()
        {
            var writer1 = TellHelper("Message 1").BindIgnoringLeft(
                TellHelper("Message 2")).BindIgnoringLeft(
                    MyWriter.Return(42));

            var writer2 = MyWriter.Censor(msgs => new ListMonoid<string>(msgs.List.Where(msg => msg.EndsWith("2"))), writer1);

            var tuple = writer2.RunWriter;
            var a = tuple.Item1;
            var w = tuple.Item2;

            Assert.That(a, Is.EqualTo(42));
            Assert.That(w.List, Is.EqualTo(new[] { "Message 2" }));
        }

        private static MyWriterUnit TellHelper(string s)
        {
            var listMonoid = new ListMonoid<string>(new[] { s });
            return MyWriter.Tell(listMonoid);
        }
    }
}
