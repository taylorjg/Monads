using System.Linq;
using MonadLib;
using NUnit.Framework;

namespace MonadLibTests.WriterTests
{
    using WriterString = Writer<ListMonoid<string>, string>;

    [TestFixture]
    internal class LinqQuerySyntaxTests
    {
        [Test]
        public void WriterSelectManyLinqQuerySyntax()
        {
            var m1 = Writer<ListMonoid<string>, string>.Return(2);
            var m2 = Writer<ListMonoid<string>, string>.Return(4);
            var r =
                from x in m1
                from y in m2
                select x * y;
            Assert.That(r.RunWriter.Item1, Is.EqualTo(2 * 4));
        }

        [Test]
        public void WriterSelectManyAndTellLinqQuerySyntax()
        {
            var m1 = WriterString.Return("Hello");
            var m2 = WriterString.Return(2);
            var mr =
                from s in m1
                from _ in TellHelper(string.Format("s = {0}", s))
                from n in m2
                from __ in TellHelper(string.Format("n = {0}", n))
                select string.Concat(Enumerable.Repeat(s, n));
            var actual = mr.RunWriter;
            Assert.That(actual.Item1, Is.EqualTo("HelloHello"));
            Assert.That(actual.Item2.List, Is.EqualTo(new[] {"s = Hello", "n = 2"}));
        }

        [Test]
        public void WriterSelectManyAndLogNumberLinqQuerySyntax()
        {
            var m1 = WriterString.Return(3);
            var m2 = WriterString.Return(5);
            var mr =
                from n1 in m1
                from _ in LogNumber(n1)
                from n2 in m2
                from __ in LogNumber(n2)
                select n1 * n2;
            var actual = mr.RunWriter;
            Assert.That(actual.Item1, Is.EqualTo(3 * 5));
            Assert.That(actual.Item2.List, Is.EqualTo(new[] { "Got number 3", "Got number 5" }));
        }

        private static Writer<ListMonoid<string>, string, Unit> TellHelper(string s)
        {
            return WriterString.Tell(new ListMonoid<string>(s));
        }

        private static Writer<ListMonoid<string>, string, Unit> LogNumber(int n)
        {
            return TellHelper(string.Format("Got number {0}", n));
        }
    }
}
