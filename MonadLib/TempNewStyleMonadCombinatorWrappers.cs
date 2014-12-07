namespace MonadLib
{
    public static partial class Maybe
    {
        public static Maybe<TA> Join<TA>(Maybe<Maybe<TA>> mma)
        {
            return MonadCombinators.Join<Maybe<Maybe<TA>>, Maybe<TA>, TA>(mma);
        }
    }
}
