using System;
using MonadLib;

namespace Monads
{
    using DemoReaderState = Reader<int>;
    using DemoReader = Reader<int, int>;

    public static class ReaderExample
    {
        public static void Demo()
        {
            var reader = DemoReaderState.Ask().Bind(
                x => DemoReaderState.Return(x * 3));

            var a = reader.RunReader(2);
            Console.WriteLine("reader.RunReader(2): {0}", a);
        }
    }
}
