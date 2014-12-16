using System;
using System.Collections.Generic;
using MonadLib;

namespace MaybeApMovieReview
{
    internal class Program
    {
        private static Maybe<string> Lookup1(string key, Dictionary<string, Maybe<string>> alist)
        {
            return alist
                .GetValue(key)
                .Bind(v => v.MFilter(s => !string.IsNullOrEmpty(s)));
        }

        private static Maybe<MovieReview> ApReview(Dictionary<string, Maybe<string>> alist)
        {
            return
                Maybe.Ap(
                    Maybe.Ap(
                        Maybe.LiftM(Fn.Curry(MovieReview.MakeMovieReviewFunc), Lookup1("title", alist)),
                        Lookup1("user", alist)),
                    Lookup1("review", alist));
        }

        private static void Main()
        {
            // All keys present and correct
            Print(ApReview(new Dictionary<string, Maybe<string>>
                {
                    {"title", Maybe.Just("Jaws")},
                    {"user", Maybe.Just("Jon")},
                    {"review", Maybe.Just("A film about a shark")}
                }));

            // Missing "user" key
            Print(ApReview(new Dictionary<string, Maybe<string>>
                {
                    {"title", Maybe.Just("Jaws")},
                    {"review", Maybe.Just("A film about a shark")}
                }));

            // Value of "user" key is empty
            Print(ApReview(new Dictionary<string, Maybe<string>>
                {
                    {"title", Maybe.Just("Jaws")},
                    {"user", Maybe.Just(string.Empty)},
                    {"review", Maybe.Just("A film about a shark")}
                }));

            // Value of "user" key is Nothing
            Print(ApReview(new Dictionary<string, Maybe<string>>
                {
                    {"title", Maybe.Just("Jaws")},
                    {"user", Maybe.Nothing<string>()},
                    {"review", Maybe.Just("A film about a shark")}
                }));
        }

        private static void Print(Maybe<MovieReview> movieReview)
        {
            Console.WriteLine(movieReview.Match(MovieReview.Format, () => "Nothing"));
        }
    }
}
