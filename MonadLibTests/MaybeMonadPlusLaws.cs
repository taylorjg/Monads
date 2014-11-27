using MonadLib;
using NUnit.Framework;

namespace MonadLibTests
{
    // ReSharper disable InconsistentNaming

    [TestFixture]
    internal class MaybeMonadPlusLaws
    {
        private MonadPlusAdapter<int> _monadPlusAdapter;

        [SetUp]
        public void SetUp()
        {
            _monadPlusAdapter = Maybe.Nothing<int>().GetMonadPlusAdapter();    
        }

        [Test]
        public void MPlusOfMZeroAndMIsM()
        {
            // mzero `mplus` m  =  m
            var m = Maybe.Just(42);
            var actual = (Maybe<int>)_monadPlusAdapter.MPlus(_monadPlusAdapter.MZero, m);
            Assert.That(actual, Is.EqualTo(m));
        }

        [Test]
        public void MPlusOfMAndMZeroIsM()
        {
            // m `mplus` mzero  =  m
            var m = Maybe.Just(42);
            var actual = (Maybe<int>)_monadPlusAdapter.MPlus(m, _monadPlusAdapter.MZero);
            Assert.That(actual, Is.EqualTo(m));
        }

        [Test]
        public void MPlusAssociativity()
        {
            // m `mplus` (n `mplus` o)  =  (m `mplus` n) `mplus` o
            var m = Maybe.Just(42);
            var n = Maybe.Just(43);
            var o = Maybe.Just(44);
            var actual1 = (Maybe<int>) _monadPlusAdapter.MPlus(m, _monadPlusAdapter.MPlus(n, o));
            var actual2 = (Maybe<int>) _monadPlusAdapter.MPlus(_monadPlusAdapter.MPlus(m, n), o);
            Assert.That(actual1, Is.EqualTo(actual2));
        }
    }
}
