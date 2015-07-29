using MonadLib;
using MonadLib.Registries;
using NUnit.Framework;

namespace MonadLibTests.RegistryTests
{
    [TestFixture]
    internal class MonoidAdapterRegistryTests
    {
        [Test]
        public void Test1()
        {
            var listMonoidAdapter = MonoidAdapterRegistry.Get<string>(typeof (ListMonoid<>));
            var listMonoid = (ListMonoid<string>) listMonoidAdapter.MEmpty;
            Assert.That(listMonoid.List, Is.Empty);
        }
    }
}
