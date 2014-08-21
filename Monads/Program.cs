#pragma warning disable 168

using System;

namespace Monads
{
    internal class Program
    {
        private static void Main()
        {
            MaybeNotUsingBind();
            MaybeUsingBind();
        }

        private static void MaybeNotUsingBind()
        {
            var r1Step1 = F1(10);
            var r1Step2 = (r1Step1.IsJust) ? F2(r1Step1.FromJust()) : Maybe.Nothing<string>();
            var r1Step3 = (r1Step2.IsJust) ? F3(r1Step2.FromJust()) : Maybe.Nothing<bool>();

            var r2Step1 = F1(100);
            var r2Step2 = (r2Step1.IsJust) ? F2(r2Step1.FromJust()) : Maybe.Nothing<string>();
            var r2Step3 = (r2Step2.IsJust) ? F3(r2Step2.FromJust()) : Maybe.Nothing<bool>();

            var r3Step1 = F1(100);
            var r3Step2 = (r3Step1.IsJust) ? F2(r3Step1.FromJust()) : Maybe.Nothing<string>();
            var r3Step3 = (r3Step2.IsJust) ? F3(r3Step2.FromJust()) : Maybe.Nothing<bool>();
        }

        private static void MaybeUsingBind()
        {
            var r1 = F1(10).Bind(F2).Bind(F3);
            var r2 = F1(100).Bind(F2).Bind(F3);
            var r3 = F1(1000).Bind(F2).Bind(F3);
        }

        private static Maybe<int> F1(int x)
        {
            return Maybe.Just(x * x);
        }

        private static Maybe<string> F2(int x)
        {
            return x <= 10000 ? Maybe.Just(Convert.ToString(x)) : Maybe.Nothing<string>();
        }

        private static Maybe<bool> F3(string s)
        {
            return s.Length <= 3 ? Maybe.Just(s.Contains("0")) : Maybe.Nothing<bool>();
        }
    }
}
