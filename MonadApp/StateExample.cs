using System;
using MonadLib;

namespace Monads
{
    using TickState = State<int>;
    using Tick = State<int, int>;

    public class StateExample
    {
        public static void Demo()
        {
            var tick = Tick();
            var tuple = tick.RunState(5);
            Console.WriteLine("tick.RunState(5): {0}", tuple);
        }

        public static Tick Tick()
        {
            return TickState.Get().Bind(
                n => TickState.Put(n + 1).BindIgnoringLeft(
                    TickState.Return(n)));
        }
    }
}
