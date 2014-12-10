using System;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    [TestFixture]
    internal class MonadAgnosticFunctionsTests
    {
        private enum Context
        {
            Home,
            Mobile,
            Business
        }

        private Tuple<Context, string>[] _phoneBook;

        [SetUp]
        public void SetUp()
        {
            _phoneBook = new[]
                {
                    Tuple.Create(Context.Mobile, "456"),
                    Tuple.Create(Context.Home, "123-1"),
                    Tuple.Create(Context.Home, "123-2"),
                    Tuple.Create(Context.Business, "789"),
                };
        }

        [Test]
        public void LookupMTest()
        {
            var homeNumber = MonadPlusAgnosticFunctions.LookupM<Maybe<string>, Context, string>(Context.Home, _phoneBook);
            Assert.That(homeNumber.FromJust, Is.EqualTo("123-1"));
        }
    }
}
