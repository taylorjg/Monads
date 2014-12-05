using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
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
