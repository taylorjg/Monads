namespace Monads
{
    public class MovieReview
    {
        public string Title { get; set; }
        public string User { get; set; }
        public string Review { get; set; }

        public string Format()
        {
            return string.Format(
                "{{Title: {0}; User: {1}; Review: {2}}}",
                Title,
                User,
                Review);
        }

        public static string Format(MovieReview movieReview)
        {
            return movieReview.Format();
        }

        public static MovieReview MakeMovieReview(string title, string user, string review)
        {
            return new MovieReview
            {
                Title = title,
                User = user,
                Review = review
            };
        }
    }
}
