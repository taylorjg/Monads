namespace WriterAllAboutMonadsExample
{
    public class Rule
    {
        public Rule(Disposition disposition, Packet pattern, bool logIt)
        {
            _disposition = disposition;
            _pattern = pattern;
            _logIt = logIt;
        }

        public Disposition Disposition
        {
            get { return _disposition; }
        }

        public Packet Pattern
        {
            get { return _pattern; }
        }

        public bool LogIt
        {
            get { return _logIt; }
        }

        public override string ToString()
        {
            // Rule {disposition = Reject, pattern = Packet {from = Host 7, to = AnyHost, payload = AnyData}, logIt = True}
            return string.Format("Rule {{disposition = {0}, pattern = {1}, logIt = {2}}}", Disposition, Pattern, LogIt);
        }

        private readonly Disposition _disposition;
        private readonly Packet _pattern;
        private readonly bool _logIt;
    }
}
