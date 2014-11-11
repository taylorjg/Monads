using System;
using MonadLib;

namespace StateGame
{
    using GameValue = Int32;
    using GameState = Tuple<bool, int>;

    internal class Program
    {
        private static void Main()
        {
            var startState = Tuple.Create(false, 0);
            Console.WriteLine(PlayGame("abcaaacbbcabbab").EvalState(startState));
        }

        private static State<GameState, GameValue> PlayGame(string s)
        {
            if (s.Length == 0)
            {
                return State<GameState>.Get().Bind(
                    state =>
                        {
                            var score = state.Item2;
                            return State<GameState>.Return(score);
                        });
            }

            var x = s[0];
            var xs = s.Substring(1);

            return State<GameState>.Get().Bind(
                state =>
                    {
                        var on = state.Item1;
                        var score = state.Item2;
                        State<GameState, Unit> m = null;
                        switch (x)
                        {
                            case 'a':
                                if (on) m = State<GameState>.Put(Tuple.Create(true, score + 1));
                                break;

                            case 'b':
                                if (on) m = State<GameState>.Put(Tuple.Create(true, score - 1));
                                break;

                            case 'c':
                                m = State<GameState>.Put(Tuple.Create(!on, score));
                                break;
                        }

                        if (m == null) m = State<GameState>.Put(state);
                        return m.BindIgnoringLeft(PlayGame(xs));
                    });
        }
    }
}
