#pragma warning disable 168

using System;
using System.Collections.Generic;
using MonadLib;

namespace Monads
{
    // Either has two type parameters - TLeft and TA.
    // This is a little trick to make EitherString an alias for Either<String>.
    // EitherString is then a monad with a single type parameter - TA.
    using EitherString = Either<String>;

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
            var mb1 = ma.Bind(a => Maybe.Just(a * a)); // returns Maybe<int>
            var mb2 = ma.Bind(a => Maybe.Return(a * a)); // returns Maybe<int>
            var mb3 = ma.Bind(a => Maybe.Just(Convert.ToString(a * a))); // returns Maybe<string>
            var mb4 = ma.Bind(a => Maybe.Return(Convert.ToString(a * a))); // returns Maybe<string>

            // LiftM
            var mc = Maybe.Just(12);
            var md1 = mc.LiftM(a => a * a); // returns Maybe<int>
            var md2 = mc.LiftM(a => Convert.ToString(a * a)); // returns Maybe<string>
        }

        public static void EitherScrapbook()
        {
            // Creating a Left and Right
            var eitherLeft = EitherString.Left<int>("an error message");
            var eitherRight = EitherString.Right(10);

            // Extracting values via Left and Right
            var left = eitherLeft.Left;
            var right = eitherRight.Right;

            // Basic pattern matching
            Console.WriteLine("eitherLeft: {0}", eitherLeft.Match(l => Convert.ToString(l), r => Convert.ToString(r)));
            Console.WriteLine("eitherRight: {0}", eitherRight.Match(l => Convert.ToString(l), r => Convert.ToString(r)));

            // Bind
            var eitherRightSquared1 = eitherRight.Bind(r => EitherString.Right(r * r));
            var eitherRightSquared2 = eitherRight.Bind(r => EitherString.Return(r * r));

            // LiftM
            var eitherRightSquared3 = eitherRight.LiftM(r => r * r);
        }
    }
}
