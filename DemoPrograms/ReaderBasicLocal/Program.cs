using System;
using MonadLib;

namespace ReaderBasicLocal
{
    internal class Program
    {
        private static Reader<string, string> MyName(string step)
        {
            return Reader<string>.Ask().Bind(
                name => Reader<string>.Return(string.Format("\"{0}, I am {1}\"", step, name)));
        }

        private static Reader<string, Tuple<string, string, string>> LocalExample()
        {
            return MyName("First").Bind(
                a => Reader.Local(r => r + "dy", MyName("Second")).Bind(
                    b => MyName("Third").Bind(
                        c => Reader<string>.Return(Tuple.Create(a, b, c)))));
        }

        private static void Main()
        {
            Console.WriteLine(LocalExample().RunReader("Fred"));
        }
    }
}
