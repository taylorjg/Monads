using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests.ReaderTests
{
    using MyReader = Reader<string>;
    using MyReaderString = Reader<string, string>;

    [TestFixture]
    internal class ReaderTests
    {
        [Test]
        public void Ask()
        {
            string actual = null;

            var reader = MyReader.Ask().Bind(r =>
                {
                    actual = r;
                    return MyReader.Return(42);
                });

            var n = reader.RunReader("MyReadOnlyState");

            Assert.That(n, Is.EqualTo(42));
            Assert.That(actual, Is.EqualTo("MyReadOnlyState"));
        }

        [Test]
        public void Asks()
        {
            string actual = null;

            var reader = Reader.Asks<string, string>(r => r.ToUpper()).Bind(r =>
                {
                    actual = r;
                    return MyReader.Return(42);
                });

            var n = reader.RunReader("MyReadOnlyState");

            Assert.That(n, Is.EqualTo(42));
            Assert.That(actual, Is.EqualTo("MYREADONLYSTATE"));
        }

        [Test]
        public void Local()
        {
            var reader = MyName("Step 1").Bind(
                a => Reader.Local(r => r.ToUpper(), MyName("Step 2")).Bind(
                    b => MyName("Step 3").Bind(
                        c => Reader<string>.Return(Tuple.Create(a, b, c)))));

            var actual = reader.RunReader("Fred");

            Assert.That(actual.Item1, Is.EqualTo("Step 1, Fred"));
            Assert.That(actual.Item2, Is.EqualTo("Step 2, FRED"));
            Assert.That(actual.Item3, Is.EqualTo("Step 3, Fred"));
        }

        private static MyReaderString MyName(string step)
        {
            return MyReader.Ask().Bind(
                name => MyReader.Return(string.Format("{0}, {1}", step, name)));
        }
    }
}
