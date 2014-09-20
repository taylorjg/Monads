using System;
using System.Collections.Generic;
using MonadLib;

namespace Monads
{
    using AssociationList = Dictionary<string, Maybe<string>>;

    public class EitherExample
    {
        public static void Demo()
        {
            var alist = new AssociationList
                {
                    {"title", Maybe.Just("TTT")},
                    {"user", Maybe.Just("UUU")},
                    {"review", Maybe.Just("RRR")}
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
            Maybe<string> value;
            if (alist.TryGetValue(key, out value))
            {
                if (value.IsJust)
                {
                    if (!string.IsNullOrEmpty(value.FromJust))
                    {
                        return Either<string>.Right(value.FromJust);
                    }
                    return Either<string>.Left<string>(string.Format("Found key \"{0}\" but its value is empty", key));
                }
                return Either<string>.Left<string>(string.Format("Found key \"{0}\" but it has no value", key));
            }

            return Either<string>.Left<string>(string.Format("Failed to find a value for key \"{0}\"", key));
        }
    }
}
