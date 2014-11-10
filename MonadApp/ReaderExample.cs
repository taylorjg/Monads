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

            Console.WriteLine(LocalExample().RunReader("Fred"));

            // TODO: add a better example
            // e.g. see https://hackage.haskell.org/package/mtl-1.1.0.2/docs/Control-Monad-Reader.html
            // Example 1: Simple Reader Usage
            // Example 2: Modifying Reader Content With local
        }

        private static Reader<string, string> MyName(string step)
        {
            return Reader<string>.Ask().Bind(
                name => Reader<string>.Return(string.Format("{0}, I am {1}", step, name)));
        }

        private static Reader<string, Tuple<string, string, string>> LocalExample()
        {
            return MyName("First").Bind(
                a => Reader.Local(r => r + "dy", MyName("Second")).Bind(
                    b => MyName("Third").Bind(
                        c => Reader<string>.Return(Tuple.Create(a, b, c)))));
        }
    }
}
