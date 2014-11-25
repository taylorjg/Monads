using System;
using System.Collections.Generic;
using System.Linq;
using Flinq;
using MonadLib;

namespace WriterAllAboutMonadsExample
{
    using WriterEntry = Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry>;
    using U1 = Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry, Unit>;
    using U2 = Writer<ListMonoid<Entry>, ListMonoidAdapter<Entry>, Entry, ListMonoid<Entry>>;
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

        private static U2 MergeEntries(ListMonoid<Entry> entries1, ListMonoid<Entry> entries2)
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
            Func<ListMonoid<TA>, ListMonoid<TA>, Writer<ListMonoid<TA>, ListMonoidAdapter<TA>, TA, ListMonoid<TA>>> merge,
            IEnumerable<TB> bs,
            Func<TB, Writer<ListMonoid<TA>, ListMonoidAdapter<TA>, TA, TC>> fn)
        {
            return null;
        }

        private static U4 FilterAll(IEnumerable<Rule> rules, IEnumerable<Packet> packets)
        {
            Func<IEnumerable<Rule>, Func<Packet, U3>> partiallyAppliedFilterOne = rs => packet => FilterOne(rs, packet);

            var tell1 = new ListMonoid<Entry>(new[] {new Entry(1, "STARTING PACKET FILTER")});
            var tell2 = new ListMonoid<Entry>(new[] {new Entry(1, "STOPPING PACKET FILTER")});

            return WriterEntry.Tell(tell1).BindIgnoringLeft(
                GroupSame(new ListMonoid<Entry>(), MergeEntries, packets, partiallyAppliedFilterOne(rules)).Bind(
                    @out => WriterEntry.Tell(tell2).BindIgnoringLeft(
                        WriterEntry.Return(Maybe.CatMaybes(@out)))));
        }

        // TODO:
        // override ToString for Packet
        // override Equals/GetHashCode for Packet
        //
        // override ToString for Rule
        // 
        // override ToString for AddrBase / AnyHost / Addr
        // override Equals/GetHashCode for AddrBase / AnyHost / Addr
        //
        // override ToString for DataBase / AnyData / Data
        // override Equals/GetHashCode for DataBase / AnyData / Data

        private static void Main()
        {
        }
    }
}
