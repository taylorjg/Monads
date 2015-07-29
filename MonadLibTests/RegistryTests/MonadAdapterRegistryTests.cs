using System;
using MonadLib;
using MonadLib.Registries;
using NUnit.Framework;

namespace MonadLibTests.RegistryTests
{
    using TwTuple = Tuple<string, int>;

    [TestFixture]
    internal class MonadAdapterRegistryTests
    {
        [Test]
        public void Test1()
        {
            var eitherMonadAdapter = MonadAdapterRegistry.Get<string>(typeof (Either<,>));
            var either = (Either<string, int>)eitherMonadAdapter.Return(42);
            Assert.That(either.Right, Is.EqualTo(42));
        }

        [Test]
        public void Test2()
        {
            var writerMonadAdapter = MonadAdapterRegistry.Get<ListMonoid<string>, string>(typeof(Writer<ListMonoid<string>, string, int>));
            var writer = (Writer<ListMonoid<string>, string, int>)writerMonadAdapter.Return(42);
            Assert.That(writer.RunWriter.Item1, Is.EqualTo(42));
        }

        [Test]
        public void Test3()
        {
            var writerMonadAdapter = MonadAdapterRegistry.Get<ListMonoid<TwTuple>, TwTuple>(typeof(Writer<ListMonoid<TwTuple>, TwTuple, int>));
            var writer = (Writer<ListMonoid<TwTuple>, TwTuple, int>)writerMonadAdapter.Return(42);
            Assert.That(writer.RunWriter.Item1, Is.EqualTo(42));
        }
    }
}
