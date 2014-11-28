using System;
using MonadLib;

namespace WriterBasicTell
{
    using MyWriter = Writer<ListMonoid<string>, ListMonoidAdapter<string>, string>;
    using MyWriterInt = Writer<ListMonoid<string>, ListMonoidAdapter<string>, string, int>;
    using MyWriterUnit = Writer<ListMonoid<string>, ListMonoidAdapter<string>, string, Unit>;

    internal class Program
    {
        private static MyWriterInt LogNumber(int x)
        {
            return TellHelper(string.Format("Got number: {0}", x))
                .BindIgnoringLeft(MyWriter.Return(x));
        }

        private static MyWriterInt MultWithLog()
        {
            return LogNumber(3).Bind(
                a => LogNumber(5).Bind(
                    b => TellHelper(string.Format("multiplying {0} and {1}", a, b)).BindIgnoringLeft(
                        MyWriter.Return(a * b))));
        }

        private static void Main()
        {
            Print(MultWithLog().RunWriter);
        }

        private static MyWriterUnit TellHelper(string s)
        {
            var listMonoid = new ListMonoid<string>(s);
            return MyWriter.Tell(listMonoid);
        }

        private static void Print(Tuple<int, ListMonoid<string>> tuple)
        {
            var a = tuple.Item1;
            var w = tuple.Item2;
            Console.WriteLine("a: {0}", a);
            foreach (var msg in w.List) Console.WriteLine("msg: {0}", msg);
        }
    }
}
