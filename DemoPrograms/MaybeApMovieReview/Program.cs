using System;
using System.Collections.Generic;
using MonadLib;

namespace MaybeApMovieReview
{
    using AssociationList = IDictionary<string, Maybe<string>>;

    internal static class Program
    {
        private static Maybe<string> Lookup1(string key, AssociationList alist)
        {
            return alist
                .GetValue(key)
                .Bind(v => v.MFilter(s => !string.IsNullOrEmpty(s)));
        }

        private static Maybe<MovieReview> ApReview(AssociationList alist)
        {
            return Maybe.LiftM(Fn.Curry(MovieReview.MakeMovieReviewFunc), Lookup1("title", alist))
                .Ap(Lookup1("user", alist))
                .Ap(Lookup1("review", alist));
        }

        private static void Main()
        {
            // All keys present and correct
            Print(ApReview(new Dictionary<string, Maybe<string>>
                {
                    {"title", "Jaws".Just()},
                    {"user", "Jon".Just()},
                    {"review", "A film about a shark".Just()}
                }));

            // Missing "user" key
            Print(ApReview(new Dictionary<string, Maybe<string>>
                {
                    {"title", "Jaws".Just()},
                    {"review", "A film about a shark".Just()}
                }));

            // Value of "user" key is empty
            Print(ApReview(new Dictionary<string, Maybe<string>>
                {
                    {"title", "Jaws".Just()},
                    {"user", string.Empty.Just()},
                    {"review", "A film about a shark".Just()}
                }));

            // Value of "user" key is Nothing
            Print(ApReview(new Dictionary<string, Maybe<string>>
                {
                    {"title", "Jaws".Just()},
                    {"user", Maybe.Nothing<string>()},
                    {"review", "A film about a shark".Just()}
                }));
        }

        private static void Print(Maybe<MovieReview> movieReview)
        {
            Console.WriteLine(movieReview.Match(MovieReview.Format, () => "Nothing"));
        }
    }
}
