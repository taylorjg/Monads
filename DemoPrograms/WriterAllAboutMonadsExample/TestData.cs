using System.Collections.Generic;

namespace WriterAllAboutMonadsExample
{
    public static class TestData
    {
        public static List<Packet> Packets
        {
            get
            {
                return new List<Packet>
                    {
                        new Packet(new Host(23), new Host(7), new Data("web request")),
                        new Packet(new Host(7), new Host(7), new Data("exploit")),
                        new Packet(new Host(73), new Host(28), new Data("misrouted")),
                        new Packet(new Host(73), new Host(28), new Data("misrouted")),
                        new Packet(new Host(16), new Host(7), new Data("ftp request")),
                        new Packet(new Host(99), new Host(7), new Data("exploit")),
                        new Packet(new Host(19), new Host(7), new Data("ssh login")),
                        new Packet(new Host(67), new Host(7), new Data("network scan")),
                        new Packet(new Host(67), new Host(7), new Data("network scan")),
                        new Packet(new Host(67), new Host(7), new Data("network scan")),
                        new Packet(new Host(67), new Host(7), new Data("network scan")),
                        new Packet(new Host(21), new Host(7), new Data("ftp request")),
                        new Packet(new Host(17), new Host(7), new Data("ssh login")),
                        new Packet(new Host(42), new Host(7), new Data("web request")),
                        new Packet(new Host(42), new Host(7), new Data("web request"))
                    };
            }
        }

        public static List<Rule> Rules
        {
            get
            {
                return new List<Rule>
                    {
                        new Rule(Disposition.Reject, new Packet(new Host(7), new AnyHost(), new AnyData()), true),
                        new Rule(Disposition.Reject, new Packet(new AnyHost(), new AnyHost(), new Data("exploit")), true),
                        new Rule(Disposition.Reject, new Packet(new AnyHost(), new AnyHost(), new Data("network scan")), true),
                        new Rule(Disposition.Reject, new Packet(new Host(21), new AnyHost(), new AnyData()), true),
                        new Rule(Disposition.Accept, new Packet(new AnyHost(), new Host(7), new Data("ssh login")), true),
                        new Rule(Disposition.Accept, new Packet(new AnyHost(), new Host(7), new AnyData()), false)
                    };
            }
        }
    }
}
