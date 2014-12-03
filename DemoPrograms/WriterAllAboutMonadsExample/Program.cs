using System;
using System.Collections.Generic;
using System.Linq;
using Flinq;
using MonadLib;
using Enumerable = System.Linq.Enumerable;

namespace WriterAllAboutMonadsExample
{
    using WriterEntries = Writer<ListMonoid<Entry>, Entry>;
    using WriterEntriesUnit = Writer<ListMonoid<Entry>, Entry, Unit>;
    using WriterEntriesEntries = Writer<ListMonoid<Entry>, Entry, ListMonoid<Entry>>;
    using WriterEntriesMaybePacket = Writer<ListMonoid<Entry>, Entry, Maybe<Packet>>;
    using WriterEntriesPackets = Writer<ListMonoid<Entry>, Entry, IEnumerable<Packet>>;

    internal class Program
    {
        private static Maybe<Rule> MatchPacket(Packet packet, Rule rule)
        {
            return packet.Equals(rule.Pattern) ? Maybe.Just(rule) : Maybe.Nothing<Rule>();
        }

        private static Maybe<Rule> Match(IEnumerable<Rule> rules, Packet packet)
        {
            Func<Packet, Func<Rule, Maybe<Rule>>> partiallyAppliedMatchPacket = p => r => MatchPacket(p, r);
            var matchedRules = rules.Map(partiallyAppliedMatchPacket(packet));
            return matchedRules.FoldLeft(Maybe.MZero<Rule>(), Maybe.MPlus);
        }

        private static WriterEntriesUnit LogMsg(string s)
        {
            return WriterEntries.Tell(new ListMonoid<Entry>(new Entry(1, s)));
        }

        private static WriterEntriesEntries MergeEntries(ListMonoid<Entry> e1, ListMonoid<Entry> e2)
        {
            if (e1.List.Count == 0) return WriterEntries.Return(e2);
            if (e2.List.Count == 0) return WriterEntries.Return(e1);

            if (e1.List.Count == 1 && e2.List.Count == 1)
            {
                var n1 = e1.List[0].Count;
                var msg1 = e1.List[0].Msg;
                var n2 = e2.List[0].Count;
                var msg2 = e2.List[0].Msg;

                return (msg1 == msg2)
                           ? WriterEntries.Return(new ListMonoid<Entry>(new Entry(n1 + n2, msg1)))
                           : from _ in WriterEntries.Tell(e1)
                             select e2;
            }

            throw new InvalidOperationException("MergeEntries: non-exhaustive patterns");
        }

        private static WriterEntriesMaybePacket FilterOne(IEnumerable<Rule> rules, Packet packet)
        {
            return Match(rules, packet).Match(
                rule =>
                    {
                        var msg = string.Format("MATCH: {0} <=> {1}", rule, packet);
                        return from _ in Writer.When(rule.LogIt, LogMsg(msg))
                               select (rule.Disposition == Disposition.Accept) ? Maybe.Just(packet) : Maybe.Nothing<Packet>();
                    },
                () =>
                    {
                        var msg = string.Format("DROPPING UNMATCHED PACKET: {0}", packet);
                        return from _ in LogMsg(msg)
                               select Maybe.Nothing<Packet>();
                    });
        }

        private static Writer<ListMonoid<Entry>, Entry, List<Maybe<Packet>>> GroupSame(
            ListMonoid<Entry> initial,
            Func<ListMonoid<Entry>, ListMonoid<Entry>, Writer<ListMonoid<Entry>, Entry, ListMonoid<Entry>>> merge,
            List<Packet> packets,
            Func<Packet, Writer<ListMonoid<Entry>, Entry, Maybe<Packet>>> fn)
        {
            if (!packets.Any())
            {
                return from _ in WriterEntries.Tell(initial)
                       select new List<Maybe<Packet>>();
            }

            var x = packets.First();
            var xs = packets.Skip(1).ToList();

            return from tuple in WriterEntries.Return(fn(x).RunWriter)
                   let result = tuple.Item1
                   let output = tuple.Item2
                   let localxs = xs
                   let localfn = fn
                   let localinitial = initial
                   from @new in merge(localinitial, output)
                   from rest in GroupSame(@new, merge, localxs, localfn)
                   select Enumerable.Repeat(result, 1).Concat(rest).ToList();
        }

        private static WriterEntriesPackets FilterAll(IEnumerable<Rule> rules, List<Packet> packets)
        {
            Func<IEnumerable<Rule>, Func<Packet, WriterEntriesMaybePacket>> partiallyAppliedFilterOne = rs => packet => FilterOne(rs, packet);

            return from _ in LogMsg("STARTING PACKET FILTER")
                   from @out in GroupSame(new ListMonoid<Entry>(), MergeEntries, packets, partiallyAppliedFilterOne(rules))
                   from __ in LogMsg("STOPPING PACKET FILTER")
                   select Maybe.CatMaybes(@out);
        }

        private static void Main()
        {
            var rules = TestData.Rules;
            var packets = TestData.Packets;
            var tuple = FilterAll(rules, packets).RunWriter;
            var @out = tuple.Item1;
            var log = tuple.Item2;
            Console.WriteLine("ACCEPTED PACKETS");
            foreach (var packet in @out) Console.WriteLine(packet);
            Console.WriteLine("{0}{0}FIREWALL LOG", Environment.NewLine);
            foreach (var entry in log.List) Console.WriteLine(entry);
        }
    }
}
