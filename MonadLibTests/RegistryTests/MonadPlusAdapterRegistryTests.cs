using MonadLib;
using MonadLib.Registries;
using NUnit.Framework;

namespace MonadLibTests.RegistryTests
{
    [TestFixture]
    internal class MonadPlusAdapterRegistryTests
    {
        [Test]
        public void Test1()
        {
            var maybeMonadPlusAdapter = MonadPlusAdapterRegistry.Get<int>(typeof (Maybe<>));
            var maybe = (Maybe<int>)maybeMonadPlusAdapter.Return(42);
            Assert.That(maybe.FromJust, Is.EqualTo(42));
        }
    }
}
