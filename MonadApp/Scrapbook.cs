#pragma warning disable 168

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using MonadLib;

namespace Monads
{
    // Either has two type parameters - TLeft and TA.
    // This is a little trick to make EitherString an alias for Either<String>.
    // EitherString is then a monad with a single type parameter - TA.
    using EitherString = Either<String>;

    // Reader has two type parameters - TR and TA.
    // This is a little trick to make ReaderConfig an alias for Reader<Config>.
    // ReaderConfig is then a monad with a single type parameter - TA.
    using ReaderConfig = Reader<Config>;

    // State has two type parameters - TS and TA.
    // This is a little trick to make StateString an alias for State<string>.
    // StateString is then a monad with a single type parameter - TA.
    using TickState = State<int>;

    using IntState = State<int>;

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

            var nameValueCollection = new NameValueCollection
                {
                    {"REF", "12345678"},
                    {"SR", "AOL1234567"},
                    {"Status", "S"}
                };

            Func<string, string, string, bool> f = (r, sr, status) => true;

            var mresult = Maybe.LiftM3(
                f,
                nameValueCollection.GetValue("REF"),
                nameValueCollection.GetValue("SR"),
                nameValueCollection.GetValue("Status"));
            Console.WriteLine("mresult: {0}", mresult.Match(b => b.ToString(), () => "Nothing"));
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

        public static void StateScrapbook()
        {
            var tick = TickState
                .Get()
                .Bind(n => TickState
                               .Put(n + 1)
                               .BindIgnoringLeft(TickState.Return(n)));

            Console.WriteLine("tick.EvalState(5): {0}", tick.EvalState(5));
            Console.WriteLine("tick.ExecState(5): {0}", tick.ExecState(5));

            F1();
            F2();
            F3();
            F4();
        }

        public static void F1()
        {
            var stateMonad = IntState.Return("abc");
            var result = stateMonad.RunState(4);
            Console.WriteLine("result: {0}", result);
            // Output: result: (abc, 4)
        }

        public static void F2()
        {
            var stateMonad = IntState.Return("abc").Bind(
                s => IntState.Return(s + s));
            var result = stateMonad.RunState(4);
            Console.WriteLine("result: {0}", result);
            // Output: result: (abcabc, 4)
        }

        public static void F3()
        {
            var stateMonad = IntState.Return("abc").Bind(
                s => IntState.Get().Bind(
                    _ => IntState.Return(s + s)));
            var result = stateMonad.RunState(4);
            Console.WriteLine("result: {0}", result);
            // Output: result: (abcabc, 4)
        }

        public static void F4()
        {
            var stateMonad = IntState.Return("abc").Bind(
                s => IntState.Get().Bind(
                    n => IntState.Put(n + 1).BindIgnoringLeft(
                        IntState.Return(s + s))));
            var result = stateMonad.RunState(4);
            Console.WriteLine("result: {0}", result);
            // Output: result: (abcabc, 5)
        }

        public static void ReaderScrapbook()
        {
            var config = new Config(2);

            var reader1 = ReaderConfig
                .Ask()
                .Bind(c1 => ReaderConfig.Return(c1.Multiplier * 3));
            Console.WriteLine("reader1.RunReader(config): {0}", reader1.RunReader(config));

            var reader2 = ReaderConfig
                .Ask()
                .Local(c1 => new Config(c1.Multiplier * 2))
                .Bind(c2 => ReaderConfig.Return(c2.Multiplier * 3));
            Console.WriteLine("reader2.RunReader(config): {0}", reader2.RunReader(config));
        }
    }
}
