namespace Monads
{
    internal class Program
    {
        private static void Main()
        {
            StateExample.Demo();
            ReaderExample.Demo();
            Scrapbook.MaybeScrapbook();
            Scrapbook.EitherScrapbook();
            Scrapbook.StateScrapbook();
            Scrapbook.ReaderScrapbook();
        }
    }
}
