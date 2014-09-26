﻿using System;
using MonadLib;

namespace Monads
{
    using TickState = State<int>;
    using Tick = State<int, int>;

    public class StateExample
    {
        // https://hackage.haskell.org/package/mtl-1.1.0.2/docs/src/Control-Monad-State-Lazy.html#State

        public static void Demo()
        {
            var tick = Tick();
            var tuple = tick.RunState(5);
            Console.WriteLine("tick.RunState(5): {0}", tuple);
        }

        public static Tick Tick()
        {
            return new Tick(
                s => TickState.Get().Bind(
                    n => TickState.Put(n + 1).Bind(
                        _ => TickState.Return(n))).RunState(s));
        }
    }
}