using System;

namespace MonadLib
{
    public static class Fn
    {
        public static TA Const<TA, TB>(TA a, TB b)
        {
            return a;
        }

        public static TA ConstLazy<TA, TB>(TA a, Func<TB> bFunc)
        {
            return a;
        }

        public static TA ConstLazy<TA, TB>(TA a, Lazy<TB> b)
        {
            return a;
        }

        public static Func<T2, TResult> PartiallyApply<T1, T2, TResult>(Func<T1, T2, TResult> fn, T1 t1)
        {
            return t2 => fn(t1, t2);
        }

        public static Func<T2, T3, TResult> PartiallyApply<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> fn, T1 t1)
        {
            return (t2, t3) => fn(t1, t2, t3);
        }

        public static Func<T2, T3, T4, TResult> PartiallyApply<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> fn, T1 t1)
        {
            return (t2, t3, t4) => fn(t1, t2, t3, t4);
        }

        public static Func<T2, T3, T4, T5, TResult> PartiallyApply<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> fn, T1 t1)
        {
            return (t2, t3, t4, t5) => fn(t1, t2, t3, t4, t5);
        }
    }
}
