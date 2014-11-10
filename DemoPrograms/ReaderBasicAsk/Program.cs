using System;
using MonadLib;

namespace ReaderBasicAsk
{
    internal class Program
    {
        private static void Main()
        {
            var m = Reader<int>.Ask().Bind(
                x => Reader<int>.Return(x * 3));

            const int r = 2;
            var a = m.RunReader(r);

            Console.WriteLine("a: {0}", a);
        }
    }
}
