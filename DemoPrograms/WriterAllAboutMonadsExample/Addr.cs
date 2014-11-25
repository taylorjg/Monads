namespace WriterAllAboutMonadsExample
{
    public abstract class AddrBase
    {
    }

    public class AnyHost : AddrBase
    {
    }

    public class Host : AddrBase
    {
        public Host(int value)
        {
            _value = value;
        }

        public int Value
        {
            get { return _value; }
        }

        private readonly int _value;
    }
}
