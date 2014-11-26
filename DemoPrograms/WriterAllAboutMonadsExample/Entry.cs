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

        public override string ToString()
        {
            return Count == 1 ? Msg : string.Format("{0} X {1}", Count, Msg);
        }

        private readonly int _count;
        private readonly string _msg;
    }
}
