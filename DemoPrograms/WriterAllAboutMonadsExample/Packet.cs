namespace WriterAllAboutMonadsExample
{
    public class Packet
    {
        public Packet(AddrBase from, AddrBase to, DataBase payload)
        {
            _from = from;
            _to = to;
            _payload = payload;
        }

        public AddrBase From
        {
            get { return _from; }
        }

        public AddrBase To
        {
            get { return _to; }
        }

        public DataBase Payload
        {
            get { return _payload; }
        }

        public override string ToString()
        {
            return string.Format("Packet {{from = {0}, to = {1}, payload = {2}}}", From, To, Payload);
        }

        public override bool Equals(object rhs)
        {
            if (rhs == null)
                return false;

            if (ReferenceEquals(this, rhs))
                return true;

            if (GetType() != rhs.GetType())
                return false;

            return CompareFields(rhs as Packet);
        }

        private bool CompareFields(Packet rhs)
        {
            return From == rhs.From && To == rhs.To && Payload == rhs.Payload;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + From.GetHashCode();
                hash = hash * 23 + To.GetHashCode();
                hash = hash * 23 + Payload.GetHashCode();
                return hash;
            }
        }

        private readonly AddrBase _from;
        private readonly AddrBase _to;
        private readonly DataBase _payload;
    }
}
