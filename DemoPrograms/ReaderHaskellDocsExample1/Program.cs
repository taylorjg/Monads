using System;
using System.Collections.Generic;
using System.Linq;
using MonadLib;

namespace ReaderHaskellDocsExample1
{
    using Bindings = IDictionary<string, int>;

    internal class Program
    {
        private static void Main()
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
    }
}
