#pragma warning disable 168

using System;
using System.Collections.Generic;
using MonadLib;

namespace Monads
{
    public static class Scrapbook
    {
        public static void MaybeScrapbook()
        {
            // Just and Nothing
            var justInt = Maybe.Just(10);
            var nothingString = Maybe.Nothing<string>();

            // FromJust
            var x = justInt.FromJust;

            // Conversion from/to Nullable<T>
            int? n1 = 10;
            var mn = n1.ToMaybe();
            var n2 = mn.ToNullable();

            // Dictionary lookup
            var dict = new Dictionary<int, string>
                {
                    {1, "one"},
                    {3, "three"}
                };
            var mvJust = dict.GetValue(1);
            var mvNothing = dict.GetValue(2);

            // Basic pattern matching
            Console.WriteLine("mvJust: {0}", mvJust.Match(a => a, () => "Nothing"));
            Console.WriteLine("mvNothing: {0}", mvNothing.Match(a => a, () => "Nothing"));

            // Bind
            var ma = Maybe.Just(42);
            var mb = ma.Bind(a => Maybe.Just(Convert.ToString(a * a)));

            // LiftM
            var mc = Maybe.Just(12);
            var md = mc.LiftM(a => Convert.ToString(a * a));
        }
    }
}
