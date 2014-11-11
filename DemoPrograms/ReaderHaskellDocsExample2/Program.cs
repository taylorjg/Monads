using System;
using MonadLib;

namespace ReaderHaskellDocsExample2
{
    internal class Program
    {
        private static void Main()
        {
            const string s = "12345";
            var modifiedLen = CalculateModifiedContentLen.RunReader(s);
            var len = CalculateContentLen.RunReader(s);
            Console.WriteLine("Modified 's' length: {0}", modifiedLen);
            Console.WriteLine("Original 's' length: {0}", len);
        }

        private static Reader<string, int> CalculateContentLen
        {
            get
            {
                return Reader<string>.Ask().Bind(
                    content => Reader<string>.Return(content.Length));
            }
        }

        private static Reader<string, int> CalculateModifiedContentLen
        {
            get
            {
                Func<string, Func<string, string>> partiallyAppliedStringConcatenation = s1 => s2 => s1 + s2;
                return Reader.Local(partiallyAppliedStringConcatenation("Prefix "), CalculateContentLen);
            }
        }
    }
}
