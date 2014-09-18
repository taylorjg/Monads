using System;
using System.Collections.Generic;
using MonadLib;

namespace Monads
{
    using AssociationList = Dictionary<string, Maybe<string>>;

    internal class Program
    {
        private static void Main()
        {
            var alist = new AssociationList
                {
                    {"title", Maybe.Just("TTT")},
                    {"user", Maybe.Just("UUU")},
                    {"review", Maybe.Just("RRR")}
                };

            GetMovieReview(alist).Match(
                movieReview => Console.WriteLine("GetMovieReview returned {0}.", FormatMovieReview(movieReview)),
                () => Console.WriteLine("GetMovieReview returned Nothing."));

            Console.WriteLine(
                "GetMovieReview returned {0}.",
                GetMovieReview(alist).Match(
                    FormatMovieReview,
                    () => "Nothing"));
        }

        private class MovieReview
        {
            public string Title { get; set; }
            public string User { get; set; }
            public string Review { get; set; }
        }

        private static Maybe<MovieReview> GetMovieReview(AssociationList alist)
        {
            return Maybe.LiftM3(
                MakeMovieReview,
                Lookup1(alist, "title"),
                Lookup1(alist, "user"),
                Lookup1(alist, "review"));
        }

        private static MovieReview MakeMovieReview(string title, string user, string review)
        {
            return new MovieReview
                {
                    Title = title,
                    User = user,
                    Review = review
                };
        }

        private static Maybe<string> Lookup1(AssociationList alist, string key)
        {
            Maybe<string> value;
            return alist.TryGetValue(key, out value) && value.IsJust && !string.IsNullOrEmpty(value.FromJust)
                       ? value
                       : Maybe.Nothing<string>();
        }

        private static string FormatMovieReview(MovieReview movieReview)
        {
            return string.Format(
                "{{Title: {0}; User: {1}; Review: {2}}}",
                movieReview.Title,
                movieReview.User,
                movieReview.Review);
        }
    }
}
