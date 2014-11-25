namespace WriterAllAboutMonadsExample
{
    public abstract class DataBase
    {
    }

    public class AnyData : DataBase
    {
    }

    public class Data : DataBase
    {
        public Data(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

        public override string ToString()
        {
            return string.Format("Data {0}", Value);
        }

        public override bool Equals(object rhs)
        {
            if (rhs == null)
                return false;

            if (ReferenceEquals(this, rhs))
                return true;

            if (GetType() != rhs.GetType())
                return false;

            return CompareFields(rhs as Data);
        }

        private bool CompareFields(Data rhs)
        {
            return Value == rhs.Value;
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

        private readonly string _value;
    }
}
