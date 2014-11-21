using System;
using System.Collections.Generic;
using MonadLib;

namespace MaybeMovieReview
{
    using AssociationList = Dictionary<string, Maybe<string>>;

    internal class Program
    {
        private static Maybe<string> Lookup1(string key, AssociationList alist)
        {
            return alist
                .GetValue(key)
                .Bind(v => v.MFilter(s => !string.IsNullOrEmpty(s)));
        }

        private static Maybe<MovieReview> LiftedReview(AssociationList alist)
        {
            return Maybe.LiftM3(
                MovieReview.MakeMovieReview,
                Lookup1("title", alist),
                Lookup1("user", alist),
                Lookup1("review", alist));
        }

        private static void Main()
        {
	        // All keys present and correct
            Print(LiftedReview(new AssociationList {
                    {"title", Maybe.Just("Jaws")},
                    {"user", Maybe.Just("Jon")},
                    {"review", Maybe.Just("A film about a shark")}
                }));

	        // Missing "user" key
            Print(LiftedReview(new AssociationList {
                    {"title", Maybe.Just("Jaws")},
                    {"review", Maybe.Just("A film about a shark")}
                }));

	        // Value of "user" key is empty
            Print(LiftedReview(new AssociationList {
                    {"title", Maybe.Just("Jaws")},
                    {"user", Maybe.Just(string.Empty)},
                    {"review", Maybe.Just("A film about a shark")}
                }));

	        // Value of "user" key is Nothing
            Print(LiftedReview(new AssociationList {
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
