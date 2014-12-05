using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class MonadAdapterRegistryTests
    {
        [Test]
        public void Test1()
        {
            var eitherMonadAdapter = MonadAdapterRegistry.Get<string>(typeof (Either<>));
            var either = (Either<string, int>)eitherMonadAdapter.Return(42);
            Assert.That(either.Right, Is.EqualTo(42));
        }

        [Test]
        [Ignore("Need to figure out how to register the WriterMonadAdapter")]
        public void Test2()
        {
            var writerMonadAdapter = MonadAdapterRegistry.Get<ListMonoid<string>, string>(typeof(Writer<ListMonoid<string>, string>));
            var writer = (Writer<ListMonoid<string>, string, int>)writerMonadAdapter.Return(42);
            Assert.That(writer.RunWriter.Item1, Is.EqualTo(42));
        }
    }
}
