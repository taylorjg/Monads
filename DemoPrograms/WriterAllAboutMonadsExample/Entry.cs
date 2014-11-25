namespace WriterAllAboutMonadsExample
{
    public class Entry
    {
        public Entry(int count, string msg)
        {
            _count = count;
            _msg = msg;
        }

        public int Count
        {
            get { return _count; }
        }

        public string Msg
        {
            get { return _msg; }
        }

        public override bool Equals(object rhs)
        {
            if (rhs == null)
                return false;

            if (ReferenceEquals(this, rhs))
                return true;

            if (GetType() != rhs.GetType())
                return false;

            return CompareFields(rhs as Entry);
        }

        private bool CompareFields(Entry rhs)
        {
            return Count == rhs.Count && Msg == rhs.Msg;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + Count.GetHashCode();
                hash = hash * 23 + Msg.GetHashCode();
                return hash;
            }
        }

        private readonly int _count;
        private readonly string _msg;
    }
}
