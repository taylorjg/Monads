using System;
using System.Collections.Generic;
using MonadLib;

namespace EitherMovieReview
{
    using AssociationList = IDictionary<string, Maybe<string>>;
    using EitherError = Either<string>;

    internal static class Program
    {
        private static Either<string, string> Lookup1(string key, AssociationList alist)
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

        private static Either<string, MovieReview> LiftedReview(AssociationList alist)
        {
            return Either.LiftM(Fn.Curry(MovieReview.MakeMovieReviewFunc), Lookup1("title", alist))
                .Ap(Lookup1("user", alist))
                .Ap(Lookup1("review", alist));
        }

        private static void Main()
        {
            // All keys present and correct
            Print(LiftedReview(new Dictionary<string, Maybe<string>>
                {
                    {"title", "Jaws".Just()},
                    {"user", "Jon".Just()},
                    {"review", "A film about a shark".Just()}
                }));

            // Missing "user" key
            Print(LiftedReview(new Dictionary<string, Maybe<string>>
                {
                    {"title", "Jaws".Just()},
                    {"review", "A film about a shark".Just()}
                }));

            // Value of "user" key is empty
            Print(LiftedReview(new Dictionary<string, Maybe<string>>
                {
                    {"title", "Jaws".Just()},
                    {"user", string.Empty.Just()},
                    {"review", "A film about a shark".Just()}
                }));

            // Value of "user" key is Nothing
            Print(LiftedReview(new Dictionary<string, Maybe<string>>
                {
                    {"title", "Jaws".Just()},
                    {"user", Maybe.Nothing<string>()},
                    {"review", "A film about a shark".Just()}
                }));
        }

        private static void Print(Either<string, MovieReview> movieReview)
        {
            Console.WriteLine(movieReview.Match(error => error, MovieReview.Format));
        }
    }
}
