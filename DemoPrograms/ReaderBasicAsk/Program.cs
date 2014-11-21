using System;
using MonadLib;

namespace ReaderBasicAsk
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine(Reader<int>.Ask().Bind(
                x => Reader<int>.Return(x * 3)).RunReader(2));
        }
    }
}
