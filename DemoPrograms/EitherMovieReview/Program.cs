using System;
using System.Collections.Generic;
using MonadLib;

namespace EitherMovieReview
{
    using AssociationList = Dictionary<string, Maybe<string>>;
    using EitherError = Either<string>;

    internal class Program
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
            return Either.LiftM3(
                MovieReview.MakeMovieReview,
                Lookup1("title", alist),
                Lookup1("user", alist),
                Lookup1("review", alist));
        }

        private static void Main()
        {
            // All keys present and correct
            Print(LiftedReview(new AssociationList
                {
                    {"title", Maybe.Just("Jaws")},
                    {"user", Maybe.Just("Jon")},
                    {"review", Maybe.Just("A film about a shark")}
                }));

            // Missing "user" key
            Print(LiftedReview(new AssociationList
                {
                    {"title", Maybe.Just("Jaws")},
                    {"review", Maybe.Just("A film about a shark")}
                }));

            // Value of "user" key is empty
            Print(LiftedReview(new AssociationList
                {
                    {"title", Maybe.Just("Jaws")},
                    {"user", Maybe.Just(string.Empty)},
                    {"review", Maybe.Just("A film about a shark")}
                }));

            // Value of "user" key is Nothing
            Print(LiftedReview(new AssociationList
                {
                    {"title", Maybe.Just("Jaws")},
                    {"user", Maybe.Nothing<string>()},
                    {"review", Maybe.Just("A film about a shark")}
                }));
        }

        private static void Print(Either<string, MovieReview> movieReview)
        {
            Console.WriteLine(movieReview.Match(
                error => string.Format("An error occurred: {0}", error),
                MovieReview.Format));
        }
    }
}
