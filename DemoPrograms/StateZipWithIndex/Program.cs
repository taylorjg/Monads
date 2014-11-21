using System;
using System.Collections.Generic;
using System.Linq;
using MonadLib;

namespace StateZipWithIndex
{
    using F = State<int>;

    internal class Program
    {
        private static void Main()
        {
            Print(ZipWithIndex(new[] {"A", "B", "C"}));
        }

        public static IEnumerable<Tuple<int, T>> ZipWithIndex<T>(IEnumerable<T> @as)
        {
            var z = F.Return(Enumerable.Empty<Tuple<int, T>>());
            var m = @as.Aggregate(z, (acc, a) => acc.Bind(
                xs => F.Get().Bind(
                    n => F.Put(n + 1).LiftM(
                        _ => Cons(Tuple.Create(n, a), xs)))));
            return m.EvalState(0).Reverse();
        }

        private static void Print(IEnumerable<Tuple<int, string>> xs)
        {
            foreach (var x in xs) Console.WriteLine(x);
        }

        private static IEnumerable<T> Cons<T>(T t, IEnumerable<T> ts)
        {
            return Enumerable.Repeat(t, 1).Concat(ts);
        }
    }
}
