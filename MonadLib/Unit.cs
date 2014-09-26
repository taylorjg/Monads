namespace MonadLib
{
    public sealed class Unit
    {
        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
