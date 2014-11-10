using System;
using System.Collections.Generic;
using System.Linq;
using MonadLib;

namespace Monads
{
    using DemoReaderState = Reader<int>;
    using DemoReader = Reader<int, int>;
    using Bindings = IDictionary<string, int>;

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

            Example1SimpleReaderUsage();
            Example2ModifyingReaderContentWithLocal();
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

        // ***************************************
        // Start of Example 1: Simple Reader Usage
        // ***************************************

        private static bool IsCountCorrect(Bindings bindings)
        {
            return CalcIsCountCorrect().RunReader(bindings);
        }

        private static Reader<Bindings, bool> CalcIsCountCorrect()
        {
            Func<Bindings, int> lookupVarPartiallyApplied = bindings => LookupVar("count", bindings);

            return Reader.Asks(lookupVarPartiallyApplied).Bind(
                count => Reader<Bindings>.Ask().Bind(
                    bindings => Reader<Bindings>.Return(
                        count == bindings.Count)));
        }

        private static int LookupVar(string name, Bindings bindings)
        {
            return bindings.GetValue(name).FromJust;
        }

        private static string FormatBindings(Bindings bindings)
        {
            Func<KeyValuePair<string, int>, string> formatBinding =
                kvp => string.Format("(\"{0}\",{1})", kvp.Key, kvp.Value);
            return string.Format("[{0}]", string.Join(",", bindings.Select(formatBinding)));
        }

        private static void Example1SimpleReaderUsage()
        {
            var sampleBindings = new Dictionary<string, int>
                {
                    {"count", 3},
                    {"1", 1},
                    {"b", 2}
                };

            Console.Write("Count is correct for bindings " + FormatBindings(sampleBindings) + ": ");
            Console.WriteLine(IsCountCorrect(sampleBindings));
        }

        // *************************************
        // End of Example 1: Simple Reader Usage
        // *************************************

        // ********************************************************************************

        // *******************************************************
        // Start of Example 2: Modifying Reader Content With local
        // *******************************************************

        private static Reader<string, int> CalculateContentLen()
        {
            return Reader<string>.Ask().Bind(
                content => Reader<string>.Return(content.Length));
        }

        private static Reader<string, int> CalculateModifiedContentLen()
        {
            return Reader.Local(content => "Prefix " + content, CalculateContentLen());
        }

        private static void Example2ModifyingReaderContentWithLocal()
        {
            const string s = "12345";
            var modifiedLen = CalculateModifiedContentLen().RunReader(s);
            var len = CalculateContentLen().RunReader(s);
            Console.WriteLine("Modified 's' length: {0}", modifiedLen);
            Console.WriteLine("Original 's' length: {0}", len);
        }

        // *****************************************************
        // End of Example 2: Modifying Reader Content With local
        // *****************************************************
    }
}
