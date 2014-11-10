using System;
using System.Collections.Generic;
using MonadLib;

namespace EitherMovieReview
{
    using AssociationList = Dictionary<string, Maybe<string>>;
    using EitherError = Either<string>;

    internal class Program
    {
        private static void Main()
        {
            var alist = new AssociationList
                {
                    {"title", Maybe.Just("Jaws")},
                    {"user", Maybe.Just("Jon")},
                    {"review", Maybe.Just("A film about a shark")}
                };

            // Using void Either.Match()
            GetMovieReview(alist).Match(
                error => Console.WriteLine("GetMovieReview returned an error." + Environment.NewLine + error),
                movieReview => Console.WriteLine("GetMovieReview returned {0}.", MovieReview.Format(movieReview)));

            // Using T Either.Match<T>()
            Console.WriteLine(
                "GetMovieReview returned {0}.",
                GetMovieReview(alist).Match(
                    error => string.Format("an error: {0}", error),
                    MovieReview.Format));
        }

        private static Either<string, MovieReview> GetMovieReview(AssociationList alist)
        {
            return Either.LiftM3(
                MovieReview.MakeMovieReview,
                Lookup(alist, "title"),
                Lookup(alist, "user"),
                Lookup(alist, "review"));
        }

        private static Either<string, string> Lookup(AssociationList alist, string key)
        {
            var keyWithEmptyValue = EitherError.Left<string>(string.Format("Found key \"{0}\" but its value is empty", key));
            var keyWithNoValue = EitherError.Left<string>(string.Format("Found key \"{0}\" but it has no value", key));
            var keyNotFound = EitherError.Left<string>(string.Format("Failed to find a value for key \"{0}\"", key));

            return alist.GetValue(key).Match(
                v => v.Match(
                    s => string.IsNullOrEmpty(s) ? keyWithEmptyValue : EitherError.Right(s),
                    () => keyWithNoValue),
                () => keyNotFound);
        }
    }
}
