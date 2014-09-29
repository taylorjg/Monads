namespace Monads
{
    public class Config
    {
        public Config(int multiplier)
        {
            Multiplier = multiplier;
        }

        public int Multiplier { get; private set; }
    }
}
