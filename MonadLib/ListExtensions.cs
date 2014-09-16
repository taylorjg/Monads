//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace MonadLib
//{
//    public static class ListExtensions
//    {
//        public static IList<TA> Unit<TA>(TA a)
//        {
//            return new List<TA> {a};
//        }

//        public static IList<TB> Bind<TA, TB>(this IList<TA> ma, Func<TA, IList<TB>> f)
//        {
//            return ma.SelectMany(f).ToList();
//        }
//    }
//}
