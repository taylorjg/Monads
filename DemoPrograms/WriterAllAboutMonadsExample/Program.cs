using System;
using System.Collections.Generic;
using System.Linq;
using Flinq;
using MonadLib;

namespace WriterAllAboutMonadsExample
{
    using WriterEntry = Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry>;
    using U1 = Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry, Unit>;
    using U2 = Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry, IEnumerable<Entry>>;
    using U3 = Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry, Maybe<Packet>>;
    using U4 = Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry, IEnumerable<Packet>>;

    internal class Program
    {
        private static Maybe<Rule> MatchPacket(Packet packet, Rule rule)
        {
            return rule.Pattern == packet ? Maybe.Just(rule) : Maybe.Nothing<Rule>();
        }

        private static Maybe<Rule> Match(IEnumerable<Rule> rules, Packet packet)
        {
            var matchedRules = rules.Select(rule => MatchPacket(packet, rule));
            var z = Maybe.Nothing<Rule>() as IMonadPlus<Rule>;
            return (Maybe<Rule>)matchedRules.FoldLeft(z, z.GetMonadPlusAdapter().MPlus);
        }

        private static U1 LogMsg(string s)
        {
            return WriterEntry.Tell(new ListMonoid<Entry>(new[] {new Entry(1, s)}));
        }

        private static U2 MergeEntries(IEnumerable<Entry> entries1, IEnumerable<Entry> entries2)
        {
            return null;
        }

        private static U3 FilterOne(IEnumerable<Rule> rules, Packet packet)
        {
            return Match(rules, packet).Match(
                rule =>
                    {
                        var msg = string.Format("MATCH: {0} <=> {1}", rule, packet);
                        return Writer.When(rule.LogIt, LogMsg(msg)).BindIgnoringLeft(
                            WriterEntry.Return(rule.Disposition == Disposition.Accept ? Maybe.Just(packet) : Maybe.Nothing<Packet>()));
                    },
                () =>
                    {
                        var msg = string.Format("DROPPING UNMATCHED PACKET: {0}", packet);
                        return LogMsg(msg).BindIgnoringLeft(WriterEntry.Return(Maybe.Nothing<Packet>()));
                    });
        }

        private static Writer<ListMonoid<TA>, ListMonoidAdapter<TA>, TA, IEnumerable<TC>> GroupSame<TA, TB, TC>(
            ListMonoid<TA> a,
            Func<TA, TA, Writer<ListMonoid<TA>, ListMonoidAdapter<TA>, TA, TA>> merge,
            IEnumerable<TB> bs,
            Func<TB, Writer<ListMonoid<TA>, ListMonoidAdapter<TA>, TA, TC>> fn)
        {
            return null;
        }

        private static U4 FilterAll(IEnumerable<Rule> rules, IEnumerable<Packet> packets)
        {
            var tell1 = new ListMonoid<Entry>(new[] {new Entry(1, "STARTING PACKET FILTER")});
            var tell2 = new ListMonoid<Entry>(new[] {new Entry(1, "STOPPING PACKET FILTER")});
            return null;
        }

        private static void Main()
        {
        }
    }
}
