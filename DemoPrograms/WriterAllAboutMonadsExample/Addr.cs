namespace WriterAllAboutMonadsExample
{
    public abstract class AddrBase
    {
    }

    public class AnyHost : AddrBase
    {
        public override string ToString()
        {
            return "AnyHost";
        }

        public override bool Equals(object rhs)
        {
            return rhs != null;
        }

        public override int GetHashCode()
        {
            return 0;
        }
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

        public override string ToString()
        {
            return string.Format("Host {0}", Value);
        }

        public override bool Equals(object rhs)
        {
            if (rhs == null)
                return false;

            if (ReferenceEquals(this, rhs))
                return true;

            if (rhs is AnyHost)
                return true;

            if (GetType() != rhs.GetType())
                return false;

            return CompareFields(rhs as Host);
        }

        private bool CompareFields(Host rhs)
        {
            return Value.Equals(rhs.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + Value.GetHashCode();
                return hash;
            }
        }

        private readonly int _value;
    }
}
