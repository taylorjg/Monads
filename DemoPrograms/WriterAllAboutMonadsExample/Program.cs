using System;
using System.Collections.Generic;
using System.Linq;
using Flinq;
using MonadLib;
using Enumerable = System.Linq.Enumerable;

namespace WriterAllAboutMonadsExample
{
    using WriterEntries = Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry>;
    using WriterEntriesUnit = Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry, Unit>;
    using WriterEntriesEntries = Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry, ListMonoid<Entry>>;
    using WriterEntriesMaybePacket = Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry, Maybe<Packet>>;
    using WriterEntriesPackets = Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry, IEnumerable<Packet>>;

    internal class Program
    {
        private static Maybe<Rule> MatchPacket(Packet packet, Rule rule)
        {
            return Equals(rule.Pattern, packet) ? Maybe.Just(rule) : Maybe.Nothing<Rule>();
        }

        private static Maybe<Rule> Match(IEnumerable<Rule> rules, Packet packet)
        {
            var matchedRules = rules.Select(rule => MatchPacket(packet, rule));
            var z = Maybe.Nothing<Rule>() as IMonadPlus<Rule>;
            return (Maybe<Rule>)matchedRules.FoldLeft(z, z.GetMonadPlusAdapter().MPlus);
        }

        private static WriterEntriesUnit LogMsg(string s)
        {
            return WriterEntries.Tell(new ListMonoid<Entry>(new[] {new Entry(1, s)}));
        }

        private static WriterEntriesEntries MergeEntries(ListMonoid<Entry> e1, ListMonoid<Entry> e2)
        {
            if (e1.List.Count == 0) WriterEntries.Return(e2);
            if (e2.List.Count == 0) WriterEntries.Return(e1);

            if (e1.List.Count == 1 && e2.List.Count == 1)
            {
                var n1 = e1.List[0].Count;
                var msg1 = e1.List[0].Msg;
                var n2 = e2.List[0].Count;
                var msg2 = e2.List[0].Msg;

                if (msg1 == msg2)
                {
                    WriterEntries.Return(new ListMonoid<Entry>(new[] {new Entry(n1 + n2, msg1)}));
                }
                else
                {
                    return WriterEntries.Tell(e1).BindIgnoringLeft(WriterEntries.Return(e2));
                }
            }

            return null;
        }

        private static WriterEntriesMaybePacket FilterOne(IEnumerable<Rule> rules, Packet packet)
        {
            return Match(rules, packet).Match(
                rule =>
                    {
                        var msg = string.Format("MATCH: {0} <=> {1}", rule, packet);
                        return Writer.When(rule.LogIt, LogMsg(msg)).BindIgnoringLeft(
                            WriterEntries.Return(rule.Disposition == Disposition.Accept ? Maybe.Just(packet) : Maybe.Nothing<Packet>()));
                    },
                () =>
                    {
                        var msg = string.Format("DROPPING UNMATCHED PACKET: {0}", packet);
                        return LogMsg(msg).BindIgnoringLeft(WriterEntries.Return(Maybe.Nothing<Packet>()));
                    });
        }

        private static Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry, IList<Maybe<Packet>>> GroupSame(
            ListMonoid<Entry> initial,
            Func<ListMonoid<Entry>, ListMonoid<Entry>, Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry, ListMonoid<Entry>>> merge,
            IList<Packet> packets,
            Func<Packet, Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry, Maybe<Packet>>> fn)
        {
            if (!packets.Any())
            {
                WriterEntries.Tell(initial).BindIgnoringLeft(WriterEntries.Return(Enumerable.Empty<Maybe<Packet>>()));
            }

            var x = packets.First();
            var xs = packets.Skip(1).ToList();

            var tuple = fn(x).RunWriter;
            var result = tuple.Item1;
            var output = tuple.Item2;

            return merge(initial, output).Bind(
                @new => GroupSame(@new, merge, xs, fn)).Bind(
                    rest => WriterEntries.Return(new[] {result}.Concat(rest) as IList<Maybe<Packet>>));
        }

        private static WriterEntriesPackets FilterAll(IEnumerable<Rule> rules, IList<Packet> packets)
        {
            Func<IEnumerable<Rule>, Func<Packet, WriterEntriesMaybePacket>> partiallyAppliedFilterOne = rs => packet => FilterOne(rs, packet);

            return LogMsg("STARTING PACKET FILTER").BindIgnoringLeft(
                GroupSame(new ListMonoid<Entry>(), MergeEntries, packets, partiallyAppliedFilterOne(rules)).Bind(
                    @out => LogMsg("STOPPING PACKET FILTER").BindIgnoringLeft(
                        WriterEntries.Return(Maybe.CatMaybes(@out)))));
        }

        private static void Main()
        {
        }
    }
}
