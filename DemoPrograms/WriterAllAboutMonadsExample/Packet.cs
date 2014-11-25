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

        private readonly AddrBase _from;
        private readonly AddrBase _to;
        private readonly DataBase _payload;
    }
}
