//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Monads
//{
//    public class EnumerableMonad<TA> : IMonad<TA>
//    {
//        public EnumerableMonad<TB> Bind<TB>(Func<TA, EnumerableMonad<TB>> f)
//        {
//            var monadAdapter = MonadAdapter;
//            var mb = monadAdapter.Bind(this, f);
//            return (EnumerableMonad<TB>)mb;
//        }

//        private IMonadAdapter _monadAdapter;
//        public IMonadAdapter MonadAdapter
//        {
//            get { return _monadAdapter ?? (_monadAdapter = new EnumerableMonadAdapter()); }
//        }
//    }

//    public static class Enumerable
//    {
//        public static IEnumerable<TA> Unit<TA>(TA a)
//        {
//            yield return a;
//        }

//        public static IEnumerable<TB> Bind<TA, TB>(this IEnumerable<TA> ma, Func<TA, IEnumerable<TB>> f)
//        {
//            return ma.SelectMany(f);
//        }
//    }

//    //public static class EnumerableExtensions
//    //{
//    //    public static IEnumerable<TA> Unit<TA>(TA a)
//    //    {
//    //        yield return a;
//    //    }

//    //    public static IEnumerable<TB> Bind<TA, TB>(this IEnumerable<TA> ma, Func<TA, IEnumerable<TB>> f)
//    //    {
//    //        return ma.SelectMany(f);
//    //    }
//    //}

//    internal class EnumerableMonadAdapter : IMonadAdapter
//    {
//        public IMonad<TA> Unit<TA>(TA a)
//        {
//            throw new NotImplementedException();
//        }

//        public IMonad<TB> Bind<TA, TB>(IMonad<TA> ma, Func<TA, IMonad<TB>> f)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
