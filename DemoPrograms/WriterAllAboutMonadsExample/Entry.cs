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

        private readonly int _count;
        private readonly string _msg;
    }
}
