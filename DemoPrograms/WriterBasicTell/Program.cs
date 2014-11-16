using System;
using MonadLib;

namespace WriterBasicTell
{
    using Fred = Writer<ListMonoid<string>, ListMonoidAdapter<string>, string>;

    internal class Program
    {
        private static void Main()
        {
            var writer = TellHelper("Log message 1").BindIgnoringLeft(
                TellHelper("Log message 2").BindIgnoringLeft(
                    Fred.Return(12)));
            var tuple = writer.RunWriter;
            var a = tuple.Item1;
            var w = tuple.Item2;
            Console.WriteLine("a: {0}", a);
            foreach (var msg in w.List) Console.WriteLine("msg: {0}", msg);
        }

        private static Writer<ListMonoid<string>, ListMonoidAdapter<string>, string, Unit> TellHelper(string s)
        {
            var listMonoid = new ListMonoid<string>(new[] { s });
            return Fred.Tell(listMonoid);
        }
    }
}
