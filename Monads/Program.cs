#pragma warning disable 168

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using MonadLib;

namespace Monads
{
    internal class Program
    {
        private static void Main()
        {
            MaybeNotUsingBind();
            MaybeUsingBind();

            EitherUsingBind();

            TaskUsingBind();
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
            var r4 = Maybe.Unit(10).Bind(F2).Bind(F3);
            var r5 = Maybe.Unit(10).LiftM(RawFunction);
        }

        private static void EitherUsingBind()
        {
            var er1 = Either<string>.Right(10);
            var er2 = er1
                .Bind(FunctionReturningEither1)
                .Bind(FunctionReturningEither2);

            var el1 = Either<string>.Left<int>("my error message");
            var el2 = el1
                .Bind(FunctionReturningEither1)
                .Bind(FunctionReturningEither2);

            var er3 = Either<string>.Right(10);
            var er4 = er3.LiftM(RawFunction);

            var el5 = Either<string>.Left<string>("error");
            var er5 = Either<string>.Right("success");

            var er6 = Either<string>.Unit(12).LiftM(RawFunction);
        }

        private static Either<string, string> FunctionReturningEither1(int n)
        {
            return Either<string>.Right(Convert.ToString(n));
        }

        private static Either<string, bool> FunctionReturningEither2(string s)
        {
            return Either<string>.Right(s.Length > 1);
        }

        private static string RawFunction(int n)
        {
            return Convert.ToString(n * n);
        }

        private static void TaskUsingBind()
        {
            var memoryStream = new MemoryStream();
            var t = TaskMonadExtensions
                .Unit("http://google.com")
                .Bind(url => WebRequest.Create(url).GetResponseAsync())
                // ReSharper disable PossibleNullReferenceException
                .Bind(
                    webResponse =>
                    webResponse.GetResponseStream()
                               .CopyToAsync(memoryStream)
                               .ContinueWith(_ => TaskMonadExtensions.Unit(memoryStream)).Unwrap());
                // ReSharper restore PossibleNullReferenceException
            t.Wait();
            var r = t.Result;
            var bytes = r.ToArray();
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
