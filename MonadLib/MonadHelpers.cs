using System;
using System.Collections.Generic;
using System.Linq;

namespace MonadLib
{
    internal static class MonadHelpers
    {
        public static TA Identity<TA>(TA a)
        {
            return a;
        }

        public static IEnumerable<TA> Nil<TA>()
        {
            return Enumerable.Empty<TA>();
        }

        public static IEnumerable<TA> One<TA>(TA a)
        {
            return Enumerable.Repeat(a, 1);
        }

        public static IEnumerable<TA> Cons<TA>(TA x, IEnumerable<TA> xs)
        {
            return One(x).Concat(xs);
        }

        public static Maybe<Tuple<T, IEnumerable<T>>> HeadAndTail<T>(this IEnumerable<T> source)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext()) return Maybe.Nothing<Tuple<T, IEnumerable<T>>>();
                var head = enumerator.Current;
                var tail = new TailIterator<T>(enumerator) as IEnumerable<T>;
                return Maybe.Just(Tuple.Create(head, tail));
            }
        }

        public static Type GetTypeOrGenericTypeDefinition(Type type)
        {
            return type.IsGenericType ? type.GetGenericTypeDefinition() : type;
        }
    }
}
