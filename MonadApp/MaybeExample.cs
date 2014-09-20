using System;
using System.Collections.Generic;
using MonadLib;

namespace Monads
{
    using AssociationList = Dictionary<string, Maybe<string>>;

    public static class MaybeExample
    {
        public static void Demo()
        {
            var alist = new AssociationList
                {
                    {"title", Maybe.Just("TTT")},
                    {"user", Maybe.Just("UUU")},
                    {"review", Maybe.Just("RRR")}
                };

            // Using void Maybe.Match()
            GetMovieReview(alist).Match(
                movieReview => Console.WriteLine("GetMovieReview returned {0}.", MovieReview.Format(movieReview)),
                () => Console.WriteLine("GetMovieReview returned Nothing."));

            // Using T Maybe.Match<T>()
            Console.WriteLine(
                "GetMovieReview returned {0}.",
                GetMovieReview(alist).Match(
                    MovieReview.Format,
                    () => "Nothing"));
        }

        private static Maybe<MovieReview> GetMovieReview(AssociationList alist)
        {
            return Maybe.LiftM3(
                MovieReview.MakeMovieReview,
                Lookup(alist, "title"),
                Lookup(alist, "user"),
                Lookup(alist, "review"));
        }

        private static Maybe<string> Lookup(AssociationList alist, string key)
        {
            Maybe<string> value;
            return alist.TryGetValue(key, out value) && value.IsJust && !string.IsNullOrEmpty(value.FromJust)
                       ? value
                       : Maybe.Nothing<string>();
        }
    }
}
