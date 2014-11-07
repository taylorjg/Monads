using System;
using System.Collections.Generic;
using System.Linq;
using MonadLib;

namespace ZipWithIndex
{
    internal class Program
    {
        private static void Main()
        {
            var source = new[] { "a", "b", "c", "d", "e" };
            var xs = ZipWithIndex(source);
            foreach (var x in xs) Console.WriteLine(x);
        }

        public static IEnumerable<Tuple<int, T>> ZipWithIndex<T>(IEnumerable<T> source)
        {
            var seed = State<int>.Return(Enumerable.Empty<Tuple<int, T>>());
            var stateMonad = source.Aggregate(seed, (acc, a) => acc.Bind(
                xs => State<int>.Get().Bind(
                    n => State<int>.Put(n + 1).LiftM(
                        _ => Cons(Tuple.Create(n, a), xs)))));
            return stateMonad.EvalState(0).Reverse();
        }

        private static IEnumerable<T> Cons<T>(T t, IEnumerable<T> ts)
        {
            return Enumerable.Repeat(t, 1).Concat(ts);
        }
    }
}
