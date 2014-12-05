﻿using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
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