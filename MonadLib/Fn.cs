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

        // PartiallyApply passing one parameter

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

        // PartiallyApply passing two parameters

        public static Func<T3, TResult> PartiallyApply<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> fn, T1 t1, T2 t2)
        {
            return t3 => fn(t1, t2, t3);
        }

        public static Func<T3, T4, TResult> PartiallyApply<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> fn, T1 t1, T2 t2)
        {
            return (t3, t4) => fn(t1, t2, t3, t4);
        }

        public static Func<T3, T4, T5, TResult> PartiallyApply<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> fn, T1 t1, T2 t2)
        {
            return (t3, t4, t5) => fn(t1, t2, t3, t4, t5);
        }

        // PartiallyApply passing three parameters

        public static Func<T4, TResult> PartiallyApply<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> fn, T1 t1, T2 t2, T3 t3)
        {
            return t4 => fn(t1, t2, t3, t4);
        }

        public static Func<T4, T5, TResult> PartiallyApply<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> fn, T1 t1, T2 t2, T3 t3)
        {
            return (t4, t5) => fn(t1, t2, t3, t4, t5);
        }

        // PartiallyApply passing four parameters

        public static Func<T5, TResult> PartiallyApply<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> fn, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return t5 => fn(t1, t2, t3, t4, t5);
        }

        // Curry

        public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(Func<T1, T2, TResult> f)
        {
            return t1 => t2 => f(t1, t2);
        }

        public static Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> f)
        {
            return t1 => t2 => t3 => f(t1, t2, t3);
        }

        public static Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> Curry<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> f)
        {
            return t1 => t2 => t3 => t4 => f(t1, t2, t3, t4);
        }

        public static Func<T1, Func<T2, Func<T3, Func<T4, Func<T5, TResult>>>>> Curry<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> f)
        {
            return t1 => t2 => t3 => t4 => t5 => f(t1, t2, t3, t4, t5);
        }
    }
}
