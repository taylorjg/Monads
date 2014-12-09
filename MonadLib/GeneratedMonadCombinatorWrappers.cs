using System;
using System.Collections.Generic;
using System.Linq;
using MonadLib.Registries;

namespace MonadLib
{
    public static partial class Maybe
    {
        public static Maybe<TB> Select<TA, TB>(this Maybe<TA> ma, Func<TA, TB> f) 
        {
            return ma.Map(f);
        }

        public static Maybe<TB> SelectMany<TA, TB>(this Maybe<TA> ma, Func<TA, Maybe<TB>> f) 
        {
            return ma.Bind(f);
        }

        public static Maybe<TC> SelectMany<TA, TB, TC>(this Maybe<TA> ma, Func<TA, Maybe<TB>> f1, Func<TA, TB, TC> f2) 
        {
            return ma.Bind(
                a => f1(a).Bind(
                    b => Return(f2(a, b))));
        }

        public static Maybe<TB> Bind<TA, TB>(this Maybe<TA> ma, Func<TA, Maybe<TB>> f) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (Maybe<TB>)monadAdapter.Bind(ma, f);
        }

        public static Maybe<TB> BindIgnoringLeft<TA, TB>(this Maybe<TA> ma, Maybe<TB> mb) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (Maybe<TB>)monadAdapter.BindIgnoringLeft(ma, mb);
        }

        public static Maybe<TB> Map<TA, TB>(Func<TA, TB> f, Maybe<TA> ma) 
        {
            return ma.Map(f);
        }
        
        public static Maybe<TB> Map<TA, TB>(this Maybe<TA> ma, Func<TA, TB> f) 
        {
            return ma.LiftM(f);
        }
        
        public static Maybe<TB> FlatMap<TA, TB>(Func<TA, Maybe<TB>> f, Maybe<TA> ma) 
        {
            return ma.Bind(f);
        }

        public static Maybe<TB> FlatMap<TA, TB>(this Maybe<TA> ma, Func<TA, Maybe<TB>> f) 
        {
            return ma.Bind(f);
        }
        
        public static Maybe<TB> LiftM<TA, TB>(Func<TA, TB> f, Maybe<TA> ma) 
        {
            return ma.LiftM(f);
        }

        public static Maybe<TB> LiftM<TA, TB>(this Maybe<TA> ma, Func<TA, TB> f) 
        {
            return MonadCombinators.LiftM<Maybe<TB>, TA, TB>(f, ma);
        }

        public static Maybe<TC> LiftM2<TA, TB, TC>(Func<TA, TB, TC> f, Maybe<TA> ma, Maybe<TB> mb) 
        {
            return ma.LiftM2(mb, f);
        }

        public static Maybe<TC> LiftM2<TA, TB, TC>(this Maybe<TA> ma, Maybe<TB> mb, Func<TA, TB, TC> f) 
        {
            return MonadCombinators.LiftM2<Maybe<TC>, TA, TB, TC>(f, ma, mb);
        }

        public static Maybe<TD> LiftM3<TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc) 
        {
            return ma.LiftM3(mb, mc, f);
        }

        public static Maybe<TD> LiftM3<TA, TB, TC, TD>(this Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Func<TA, TB, TC, TD> f) 
        {
            return MonadCombinators.LiftM3<Maybe<TD>, TA, TB, TC, TD>(f, ma, mb, mc);
        }

        public static Maybe<TE> LiftM4<TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Maybe<TD> md) 
        {
            return ma.LiftM4(mb, mc, md, f);
        }

        public static Maybe<TE> LiftM4<TA, TB, TC, TD, TE>(this Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Maybe<TD> md, Func<TA, TB, TC, TD, TE> f) 
        {
            return MonadCombinators.LiftM4<Maybe<TE>, TA, TB, TC, TD, TE>(f, ma, mb, mc, md);
        }

        public static Maybe<TF> LiftM5<TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Maybe<TD> md, Maybe<TE> me) 
        {
            return ma.LiftM5(mb, mc, md, me, f);
        }

        public static Maybe<TF> LiftM5<TA, TB, TC, TD, TE, TF>(this Maybe<TA> ma, Maybe<TB> mb, Maybe<TC> mc, Maybe<TD> md, Maybe<TE> me, Func<TA, TB, TC, TD, TE, TF> f) 
        {
            return MonadCombinators.LiftM5<Maybe<TF>, TA, TB, TC, TD, TE, TF>(f, ma, mb, mc, md, me);
        }

        public static Maybe<TA> Join<TA>(Maybe<Maybe<TA>> mma) 
        {
            return MonadCombinators.Join<Maybe<Maybe<TA>>, Maybe<TA>, TA>(mma);
        }

        public static Maybe<IEnumerable<TA>> Sequence<TA>(IEnumerable<Maybe<TA>> ms) 
        {
            return MonadCombinators.Sequence<Maybe<IEnumerable<TA>>, TA>(ms);
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> Sequence_<TA>(IEnumerable<Maybe<TA>> ms) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators.Sequence_<Maybe<Unit>, TA>(ms);
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> Sequence_<TA>(params Maybe<TA>[] ms) 
        // ReSharper restore InconsistentNaming
        {
            return Sequence_(ms.AsEnumerable());
        }

        public static Maybe<IEnumerable<TB>> MapM<TA, TB>(Func<TA, Maybe<TB>> f, IEnumerable<TA> @as)  
        {
            return MonadCombinators.MapM<Maybe<IEnumerable<TB>>, TA, TB>(f, @as);
        }

        public static Maybe<IEnumerable<TB>> MapM<TA, TB>(Func<TA, Maybe<TB>> f, params TA[] @as) 
        {
            return MapM(f, @as.AsEnumerable());
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> MapM_<TA, TB>(Func<TA, Maybe<TB>> f, IEnumerable<TA> @as) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators.MapM_<Maybe<Unit>, TA, TB>(f, @as);
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> MapM_<TA, TB>(Func<TA, Maybe<TB>> f, params TA[] @as) 
        // ReSharper restore InconsistentNaming
        {
            return MapM_(f, @as.AsEnumerable());
        }

        public static Maybe<IEnumerable<TB>> ForM<TA, TB>(IEnumerable<TA> @as, Func<TA, Maybe<TB>> f) 
        {
            return MonadCombinators.MapM<Maybe<IEnumerable<TB>>, TA, TB>(f, @as);
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> ForM_<TA, TB>(IEnumerable<TA> @as, Func<TA, Maybe<TB>> f) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators.MapM_<Maybe<Unit>, TA, TB>(f, @as);
        }

        public static Maybe<IEnumerable<TA>> ReplicateM<TA>(int n, Maybe<TA> ma) 
        {
            return ma.ReplicateM(n);
        }

        public static Maybe<IEnumerable<TA>> ReplicateM<TA>(this Maybe<TA> ma, int n) 
        {
            return MonadCombinators.ReplicateM<Maybe<IEnumerable<TA>>, TA>(n, ma);
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> ReplicateM_<TA>(int n, Maybe<TA> ma) 
        // ReSharper restore InconsistentNaming
        {
            return ma.ReplicateM_(n);
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> ReplicateM_<TA>(this Maybe<TA> ma, int n) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators.ReplicateM_<Maybe<Unit>, TA>(n, ma);
        }

        public static Maybe<TA> FoldM<TA, TB>(Func<TA, TB, Maybe<TA>> f, TA a, IEnumerable<TB> bs) 
        {
            return (Maybe<TA>)MonadCombinators.FoldMInternal(f, a, bs, new MaybeMonadAdapter());
        }

        public static Maybe<TA> FoldM<TA, TB>(Func<TA, TB, Maybe<TA>> f, TA a, params TB[] bs) 
        {
            return FoldM(f, a, bs.AsEnumerable());
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> FoldM_<TA, TB>(Func<TA, TB, Maybe<TA>> f, TA a, IEnumerable<TB> bs) 
        // ReSharper restore InconsistentNaming
        {
            return (Maybe<Unit>)MonadCombinators.FoldMInternal_(f, a, bs, new MaybeMonadAdapter());
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> FoldM_<TA, TB>(Func<TA, TB, Maybe<TA>> f, TA a, params TB[] bs) 
        // ReSharper restore InconsistentNaming
        {
            return FoldM_(f, a, bs.AsEnumerable());
        }

        public static Maybe<IEnumerable<TC>> ZipWithM<TA, TB, TC>(Func<TA, TB, Maybe<TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs) 
        {
            return MonadCombinators.ZipWithM<Maybe<IEnumerable<TC>>, TA, TB, TC>(f, @as, bs);
        }

        // ReSharper disable InconsistentNaming
        public static Maybe<Unit> ZipWithM_<TA, TB, TC>(Func<TA, TB, Maybe<TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators.ZipWithM_<Maybe<Unit>, TA, TB, TC>(f, @as, bs);
        }

        public static Maybe<IEnumerable<TA>> FilterM<TA>(Func<TA, Maybe<bool>> p, IEnumerable<TA> @as) 
        {
            return (Maybe<IEnumerable<TA>>)MonadCombinators.FilterMInternal(p, @as, new MaybeMonadAdapter());
        }

        public static Maybe<IEnumerable<TA>> FilterM<TA>(Func<TA, Maybe<bool>> p, params TA[] @as) 
        {
            return FilterM(p, @as.AsEnumerable());
        }

        public static Maybe<Unit> When(bool b, Maybe<Unit> m) 
        {
            return m.When(b);
        }

        public static Maybe<Unit> When(this Maybe<Unit> m, bool b) 
        {
            return MonadCombinators.When(b, m);
        }

        public static Maybe<Unit> Unless(bool b, Maybe<Unit> m) 
        {
            return m.Unless(b);
        }

        public static Maybe<Unit> Unless(this Maybe<Unit> m, bool b) 
        {
            return MonadCombinators.Unless(b, m);
        }

        public static Maybe<TB> Forever<TA, TB>(this Maybe<TA> m) 
        {
            return (Maybe<TB>)MonadCombinators.Forever<TA, TB>(m);
        }

        public static Maybe<Unit> Void<TA>(this Maybe<TA> m) 
        {
            return (Maybe<Unit>)MonadCombinators.Void(m);
        }

        public static Maybe<TB> Ap<TA, TB>(Maybe<Func<TA, TB>> mf, Maybe<TA> ma) 
        {
            return ma.Ap(mf);
        }

        public static Maybe<TB> Ap<TA, TB>(this Maybe<TA> ma, Maybe<Func<TA, TB>> mf) 
        {
            return MonadCombinators.Ap<Maybe<TB>, TA, TB>(mf, ma);
        }

        public static Func<TA, Maybe<TC>> Compose<TA, TB, TC>(Func<TA, Maybe<TB>> f, Func<TB, Maybe<TC>> g) 
        {
            return a => (Maybe<TC>)MonadCombinators.Compose(f, g)(a);
        }

        public static Maybe<TA> MZero<TA>()
        {
            var monadPlusAdapter = MonadPlusAdapterRegistry.Get<TA>(typeof(Maybe<TA>));
            return (Maybe<TA>)monadPlusAdapter.MZero;
        }

        public static Maybe<TA> MPlus<TA>(this Maybe<TA> xs, Maybe<TA> ys)
        {
            var monadPlusAdapter = MonadPlusAdapterRegistry.Get<TA>(typeof(Maybe<TA>));
            return (Maybe<TA>)monadPlusAdapter.MPlus(xs, ys);
        }

        public static Maybe<TA> MFilter<TA>(Func<TA, bool> p, Maybe<TA> ma)
        {
            return ma.MFilter(p);
        }

        public static Maybe<TA> MFilter<TA>(this Maybe<TA> ma, Func<TA, bool> p)
        {
            return MonadPlusCombinators.MFilter<Maybe<TA>, TA>(p, ma);
        }

        public static Maybe<TA> MSum<TA>(IEnumerable<Maybe<TA>> ms)
        {
            return MonadPlusCombinators.MSum<Maybe<TA>, TA>(ms);
        }

        public static Maybe<TA> MSum<TA>(params Maybe<TA>[] ms)
        {
            return MSum(ms.AsEnumerable());
        }

        public static Maybe<Unit> Guard(bool b)
        {
            return MonadPlusCombinators.Guard<Maybe<Unit>>(b);
        }
    }

    public static partial class Either
    {
        public static Either<TLeft, TB> Select<TLeft, TA, TB>(this Either<TLeft, TA> ma, Func<TA, TB> f) 
        {
            return ma.Map(f);
        }

        public static Either<TLeft, TB> SelectMany<TLeft, TA, TB>(this Either<TLeft, TA> ma, Func<TA, Either<TLeft, TB>> f) 
        {
            return ma.Bind(f);
        }

        public static Either<TLeft, TC> SelectMany<TLeft, TA, TB, TC>(this Either<TLeft, TA> ma, Func<TA, Either<TLeft, TB>> f1, Func<TA, TB, TC> f2) 
        {
            return ma.Bind(
                a => f1(a).Bind(
                    b => Either<TLeft>.Return(f2(a, b))));
        }

        public static Either<TLeft, TB> Bind<TLeft, TA, TB>(this Either<TLeft, TA> ma, Func<TA, Either<TLeft, TB>> f) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (Either<TLeft, TB>)monadAdapter.Bind(ma, f);
        }

        public static Either<TLeft, TB> BindIgnoringLeft<TLeft, TA, TB>(this Either<TLeft, TA> ma, Either<TLeft, TB> mb) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (Either<TLeft, TB>)monadAdapter.BindIgnoringLeft(ma, mb);
        }

        public static Either<TLeft, TB> Map<TLeft, TA, TB>(Func<TA, TB> f, Either<TLeft, TA> ma) 
        {
            return ma.Map(f);
        }
        
        public static Either<TLeft, TB> Map<TLeft, TA, TB>(this Either<TLeft, TA> ma, Func<TA, TB> f) 
        {
            return ma.LiftM(f);
        }
        
        public static Either<TLeft, TB> FlatMap<TLeft, TA, TB>(Func<TA, Either<TLeft, TB>> f, Either<TLeft, TA> ma) 
        {
            return ma.Bind(f);
        }

        public static Either<TLeft, TB> FlatMap<TLeft, TA, TB>(this Either<TLeft, TA> ma, Func<TA, Either<TLeft, TB>> f) 
        {
            return ma.Bind(f);
        }
        
        public static Either<TLeft, TB> LiftM<TLeft, TA, TB>(Func<TA, TB> f, Either<TLeft, TA> ma) 
        {
            return ma.LiftM(f);
        }

        public static Either<TLeft, TB> LiftM<TLeft, TA, TB>(this Either<TLeft, TA> ma, Func<TA, TB> f) 
        {
            return MonadCombinators<TLeft>.LiftM<Either<TLeft, TB>, TA, TB>(f, ma);
        }

        public static Either<TLeft, TC> LiftM2<TLeft, TA, TB, TC>(Func<TA, TB, TC> f, Either<TLeft, TA> ma, Either<TLeft, TB> mb) 
        {
            return ma.LiftM2(mb, f);
        }

        public static Either<TLeft, TC> LiftM2<TLeft, TA, TB, TC>(this Either<TLeft, TA> ma, Either<TLeft, TB> mb, Func<TA, TB, TC> f) 
        {
            return MonadCombinators<TLeft>.LiftM2<Either<TLeft, TC>, TA, TB, TC>(f, ma, mb);
        }

        public static Either<TLeft, TD> LiftM3<TLeft, TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, Either<TLeft, TA> ma, Either<TLeft, TB> mb, Either<TLeft, TC> mc) 
        {
            return ma.LiftM3(mb, mc, f);
        }

        public static Either<TLeft, TD> LiftM3<TLeft, TA, TB, TC, TD>(this Either<TLeft, TA> ma, Either<TLeft, TB> mb, Either<TLeft, TC> mc, Func<TA, TB, TC, TD> f) 
        {
            return MonadCombinators<TLeft>.LiftM3<Either<TLeft, TD>, TA, TB, TC, TD>(f, ma, mb, mc);
        }

        public static Either<TLeft, TE> LiftM4<TLeft, TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, Either<TLeft, TA> ma, Either<TLeft, TB> mb, Either<TLeft, TC> mc, Either<TLeft, TD> md) 
        {
            return ma.LiftM4(mb, mc, md, f);
        }

        public static Either<TLeft, TE> LiftM4<TLeft, TA, TB, TC, TD, TE>(this Either<TLeft, TA> ma, Either<TLeft, TB> mb, Either<TLeft, TC> mc, Either<TLeft, TD> md, Func<TA, TB, TC, TD, TE> f) 
        {
            return MonadCombinators<TLeft>.LiftM4<Either<TLeft, TE>, TA, TB, TC, TD, TE>(f, ma, mb, mc, md);
        }

        public static Either<TLeft, TF> LiftM5<TLeft, TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, Either<TLeft, TA> ma, Either<TLeft, TB> mb, Either<TLeft, TC> mc, Either<TLeft, TD> md, Either<TLeft, TE> me) 
        {
            return ma.LiftM5(mb, mc, md, me, f);
        }

        public static Either<TLeft, TF> LiftM5<TLeft, TA, TB, TC, TD, TE, TF>(this Either<TLeft, TA> ma, Either<TLeft, TB> mb, Either<TLeft, TC> mc, Either<TLeft, TD> md, Either<TLeft, TE> me, Func<TA, TB, TC, TD, TE, TF> f) 
        {
            return MonadCombinators<TLeft>.LiftM5<Either<TLeft, TF>, TA, TB, TC, TD, TE, TF>(f, ma, mb, mc, md, me);
        }

        public static Either<TLeft, TA> Join<TLeft, TA>(Either<TLeft, Either<TLeft, TA>> mma) 
        {
            return MonadCombinators<TLeft>.Join<Either<TLeft, Either<TLeft, TA>>, Either<TLeft, TA>, TA>(mma);
        }

        public static Either<TLeft, IEnumerable<TA>> Sequence<TLeft, TA>(IEnumerable<Either<TLeft, TA>> ms) 
        {
            return MonadCombinators<TLeft>.Sequence<Either<TLeft, IEnumerable<TA>>, TA>(ms);
        }

        // ReSharper disable InconsistentNaming
        public static Either<TLeft, Unit> Sequence_<TLeft, TA>(IEnumerable<Either<TLeft, TA>> ms) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TLeft>.Sequence_<Either<TLeft, Unit>, TA>(ms);
        }

        // ReSharper disable InconsistentNaming
        public static Either<TLeft, Unit> Sequence_<TLeft, TA>(params Either<TLeft, TA>[] ms) 
        // ReSharper restore InconsistentNaming
        {
            return Sequence_(ms.AsEnumerable());
        }

        public static Either<TLeft, IEnumerable<TB>> MapM<TLeft, TA, TB>(Func<TA, Either<TLeft, TB>> f, IEnumerable<TA> @as)  
        {
            return MonadCombinators<TLeft>.MapM<Either<TLeft, IEnumerable<TB>>, TA, TB>(f, @as);
        }

        public static Either<TLeft, IEnumerable<TB>> MapM<TLeft, TA, TB>(Func<TA, Either<TLeft, TB>> f, params TA[] @as) 
        {
            return MapM(f, @as.AsEnumerable());
        }

        // ReSharper disable InconsistentNaming
        public static Either<TLeft, Unit> MapM_<TLeft, TA, TB>(Func<TA, Either<TLeft, TB>> f, IEnumerable<TA> @as) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TLeft>.MapM_<Either<TLeft, Unit>, TA, TB>(f, @as);
        }

        // ReSharper disable InconsistentNaming
        public static Either<TLeft, Unit> MapM_<TLeft, TA, TB>(Func<TA, Either<TLeft, TB>> f, params TA[] @as) 
        // ReSharper restore InconsistentNaming
        {
            return MapM_(f, @as.AsEnumerable());
        }

        public static Either<TLeft, IEnumerable<TB>> ForM<TLeft, TA, TB>(IEnumerable<TA> @as, Func<TA, Either<TLeft, TB>> f) 
        {
            return MonadCombinators<TLeft>.MapM<Either<TLeft, IEnumerable<TB>>, TA, TB>(f, @as);
        }

        // ReSharper disable InconsistentNaming
        public static Either<TLeft, Unit> ForM_<TLeft, TA, TB>(IEnumerable<TA> @as, Func<TA, Either<TLeft, TB>> f) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TLeft>.MapM_<Either<TLeft, Unit>, TA, TB>(f, @as);
        }

        public static Either<TLeft, IEnumerable<TA>> ReplicateM<TLeft, TA>(int n, Either<TLeft, TA> ma) 
        {
            return ma.ReplicateM(n);
        }

        public static Either<TLeft, IEnumerable<TA>> ReplicateM<TLeft, TA>(this Either<TLeft, TA> ma, int n) 
        {
            return MonadCombinators<TLeft>.ReplicateM<Either<TLeft, IEnumerable<TA>>, TA>(n, ma);
        }

        // ReSharper disable InconsistentNaming
        public static Either<TLeft, Unit> ReplicateM_<TLeft, TA>(int n, Either<TLeft, TA> ma) 
        // ReSharper restore InconsistentNaming
        {
            return ma.ReplicateM_(n);
        }

        // ReSharper disable InconsistentNaming
        public static Either<TLeft, Unit> ReplicateM_<TLeft, TA>(this Either<TLeft, TA> ma, int n) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TLeft>.ReplicateM_<Either<TLeft, Unit>, TA>(n, ma);
        }

        public static Either<TLeft, TA> FoldM<TLeft, TA, TB>(Func<TA, TB, Either<TLeft, TA>> f, TA a, IEnumerable<TB> bs) 
        {
            return (Either<TLeft, TA>)MonadCombinators<TLeft>.FoldMInternal(f, a, bs, new EitherMonadAdapter<TLeft>());
        }

        public static Either<TLeft, TA> FoldM<TLeft, TA, TB>(Func<TA, TB, Either<TLeft, TA>> f, TA a, params TB[] bs) 
        {
            return FoldM(f, a, bs.AsEnumerable());
        }

        // ReSharper disable InconsistentNaming
        public static Either<TLeft, Unit> FoldM_<TLeft, TA, TB>(Func<TA, TB, Either<TLeft, TA>> f, TA a, IEnumerable<TB> bs) 
        // ReSharper restore InconsistentNaming
        {
            return (Either<TLeft, Unit>)MonadCombinators<TLeft>.FoldMInternal_(f, a, bs, new EitherMonadAdapter<TLeft>());
        }

        // ReSharper disable InconsistentNaming
        public static Either<TLeft, Unit> FoldM_<TLeft, TA, TB>(Func<TA, TB, Either<TLeft, TA>> f, TA a, params TB[] bs) 
        // ReSharper restore InconsistentNaming
        {
            return FoldM_(f, a, bs.AsEnumerable());
        }

        public static Either<TLeft, IEnumerable<TC>> ZipWithM<TLeft, TA, TB, TC>(Func<TA, TB, Either<TLeft, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs) 
        {
            return MonadCombinators<TLeft>.ZipWithM<Either<TLeft, IEnumerable<TC>>, TA, TB, TC>(f, @as, bs);
        }

        // ReSharper disable InconsistentNaming
        public static Either<TLeft, Unit> ZipWithM_<TLeft, TA, TB, TC>(Func<TA, TB, Either<TLeft, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TLeft>.ZipWithM_<Either<TLeft, Unit>, TA, TB, TC>(f, @as, bs);
        }

        public static Either<TLeft, IEnumerable<TA>> FilterM<TLeft, TA>(Func<TA, Either<TLeft, bool>> p, IEnumerable<TA> @as) 
        {
            return (Either<TLeft, IEnumerable<TA>>)MonadCombinators<TLeft>.FilterMInternal(p, @as, new EitherMonadAdapter<TLeft>());
        }

        public static Either<TLeft, IEnumerable<TA>> FilterM<TLeft, TA>(Func<TA, Either<TLeft, bool>> p, params TA[] @as) 
        {
            return FilterM(p, @as.AsEnumerable());
        }

        public static Either<TLeft, Unit> When<TLeft>(bool b, Either<TLeft, Unit> m) 
        {
            return m.When(b);
        }

        public static Either<TLeft, Unit> When<TLeft>(this Either<TLeft, Unit> m, bool b) 
        {
            return MonadCombinators<TLeft>.When(b, m);
        }

        public static Either<TLeft, Unit> Unless<TLeft>(bool b, Either<TLeft, Unit> m) 
        {
            return m.Unless(b);
        }

        public static Either<TLeft, Unit> Unless<TLeft>(this Either<TLeft, Unit> m, bool b) 
        {
            return MonadCombinators<TLeft>.Unless(b, m);
        }

        public static Either<TLeft, TB> Forever<TLeft, TA, TB>(this Either<TLeft, TA> m) 
        {
            return (Either<TLeft, TB>)MonadCombinators<TLeft>.Forever<TA, TB>(m);
        }

        public static Either<TLeft, Unit> Void<TLeft, TA>(this Either<TLeft, TA> m) 
        {
            return (Either<TLeft, Unit>)MonadCombinators<TLeft>.Void(m);
        }

        public static Either<TLeft, TB> Ap<TLeft, TA, TB>(Either<TLeft, Func<TA, TB>> mf, Either<TLeft, TA> ma) 
        {
            return ma.Ap(mf);
        }

        public static Either<TLeft, TB> Ap<TLeft, TA, TB>(this Either<TLeft, TA> ma, Either<TLeft, Func<TA, TB>> mf) 
        {
            return MonadCombinators<TLeft>.Ap<Either<TLeft, TB>, TA, TB>(mf, ma);
        }

        public static Func<TA, Either<TLeft, TC>> Compose<TLeft, TA, TB, TC>(Func<TA, Either<TLeft, TB>> f, Func<TB, Either<TLeft, TC>> g) 
        {
            return a => (Either<TLeft, TC>)MonadCombinators<TLeft>.Compose(f, g)(a);
        }

    }

    public static partial class State
    {
        public static State<TS, TB> Select<TS, TA, TB>(this State<TS, TA> ma, Func<TA, TB> f) 
        {
            return ma.Map(f);
        }

        public static State<TS, TB> SelectMany<TS, TA, TB>(this State<TS, TA> ma, Func<TA, State<TS, TB>> f) 
        {
            return ma.Bind(f);
        }

        public static State<TS, TC> SelectMany<TS, TA, TB, TC>(this State<TS, TA> ma, Func<TA, State<TS, TB>> f1, Func<TA, TB, TC> f2) 
        {
            return ma.Bind(
                a => f1(a).Bind(
                    b => State<TS>.Return(f2(a, b))));
        }

        public static State<TS, TB> Bind<TS, TA, TB>(this State<TS, TA> ma, Func<TA, State<TS, TB>> f) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (State<TS, TB>)monadAdapter.Bind(ma, f);
        }

        public static State<TS, TB> BindIgnoringLeft<TS, TA, TB>(this State<TS, TA> ma, State<TS, TB> mb) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (State<TS, TB>)monadAdapter.BindIgnoringLeft(ma, mb);
        }

        public static State<TS, TB> Map<TS, TA, TB>(Func<TA, TB> f, State<TS, TA> ma) 
        {
            return ma.Map(f);
        }
        
        public static State<TS, TB> Map<TS, TA, TB>(this State<TS, TA> ma, Func<TA, TB> f) 
        {
            return ma.LiftM(f);
        }
        
        public static State<TS, TB> FlatMap<TS, TA, TB>(Func<TA, State<TS, TB>> f, State<TS, TA> ma) 
        {
            return ma.Bind(f);
        }

        public static State<TS, TB> FlatMap<TS, TA, TB>(this State<TS, TA> ma, Func<TA, State<TS, TB>> f) 
        {
            return ma.Bind(f);
        }
        
        public static State<TS, TB> LiftM<TS, TA, TB>(Func<TA, TB> f, State<TS, TA> ma) 
        {
            return ma.LiftM(f);
        }

        public static State<TS, TB> LiftM<TS, TA, TB>(this State<TS, TA> ma, Func<TA, TB> f) 
        {
            return MonadCombinators<TS>.LiftM<State<TS, TB>, TA, TB>(f, ma);
        }

        public static State<TS, TC> LiftM2<TS, TA, TB, TC>(Func<TA, TB, TC> f, State<TS, TA> ma, State<TS, TB> mb) 
        {
            return ma.LiftM2(mb, f);
        }

        public static State<TS, TC> LiftM2<TS, TA, TB, TC>(this State<TS, TA> ma, State<TS, TB> mb, Func<TA, TB, TC> f) 
        {
            return MonadCombinators<TS>.LiftM2<State<TS, TC>, TA, TB, TC>(f, ma, mb);
        }

        public static State<TS, TD> LiftM3<TS, TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, State<TS, TA> ma, State<TS, TB> mb, State<TS, TC> mc) 
        {
            return ma.LiftM3(mb, mc, f);
        }

        public static State<TS, TD> LiftM3<TS, TA, TB, TC, TD>(this State<TS, TA> ma, State<TS, TB> mb, State<TS, TC> mc, Func<TA, TB, TC, TD> f) 
        {
            return MonadCombinators<TS>.LiftM3<State<TS, TD>, TA, TB, TC, TD>(f, ma, mb, mc);
        }

        public static State<TS, TE> LiftM4<TS, TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, State<TS, TA> ma, State<TS, TB> mb, State<TS, TC> mc, State<TS, TD> md) 
        {
            return ma.LiftM4(mb, mc, md, f);
        }

        public static State<TS, TE> LiftM4<TS, TA, TB, TC, TD, TE>(this State<TS, TA> ma, State<TS, TB> mb, State<TS, TC> mc, State<TS, TD> md, Func<TA, TB, TC, TD, TE> f) 
        {
            return MonadCombinators<TS>.LiftM4<State<TS, TE>, TA, TB, TC, TD, TE>(f, ma, mb, mc, md);
        }

        public static State<TS, TF> LiftM5<TS, TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, State<TS, TA> ma, State<TS, TB> mb, State<TS, TC> mc, State<TS, TD> md, State<TS, TE> me) 
        {
            return ma.LiftM5(mb, mc, md, me, f);
        }

        public static State<TS, TF> LiftM5<TS, TA, TB, TC, TD, TE, TF>(this State<TS, TA> ma, State<TS, TB> mb, State<TS, TC> mc, State<TS, TD> md, State<TS, TE> me, Func<TA, TB, TC, TD, TE, TF> f) 
        {
            return MonadCombinators<TS>.LiftM5<State<TS, TF>, TA, TB, TC, TD, TE, TF>(f, ma, mb, mc, md, me);
        }

        public static State<TS, TA> Join<TS, TA>(State<TS, State<TS, TA>> mma) 
        {
            return MonadCombinators<TS>.Join<State<TS, State<TS, TA>>, State<TS, TA>, TA>(mma);
        }

        public static State<TS, IEnumerable<TA>> Sequence<TS, TA>(IEnumerable<State<TS, TA>> ms) 
        {
            return MonadCombinators<TS>.Sequence<State<TS, IEnumerable<TA>>, TA>(ms);
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> Sequence_<TS, TA>(IEnumerable<State<TS, TA>> ms) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TS>.Sequence_<State<TS, Unit>, TA>(ms);
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> Sequence_<TS, TA>(params State<TS, TA>[] ms) 
        // ReSharper restore InconsistentNaming
        {
            return Sequence_(ms.AsEnumerable());
        }

        public static State<TS, IEnumerable<TB>> MapM<TS, TA, TB>(Func<TA, State<TS, TB>> f, IEnumerable<TA> @as)  
        {
            return MonadCombinators<TS>.MapM<State<TS, IEnumerable<TB>>, TA, TB>(f, @as);
        }

        public static State<TS, IEnumerable<TB>> MapM<TS, TA, TB>(Func<TA, State<TS, TB>> f, params TA[] @as) 
        {
            return MapM(f, @as.AsEnumerable());
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> MapM_<TS, TA, TB>(Func<TA, State<TS, TB>> f, IEnumerable<TA> @as) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TS>.MapM_<State<TS, Unit>, TA, TB>(f, @as);
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> MapM_<TS, TA, TB>(Func<TA, State<TS, TB>> f, params TA[] @as) 
        // ReSharper restore InconsistentNaming
        {
            return MapM_(f, @as.AsEnumerable());
        }

        public static State<TS, IEnumerable<TB>> ForM<TS, TA, TB>(IEnumerable<TA> @as, Func<TA, State<TS, TB>> f) 
        {
            return MonadCombinators<TS>.MapM<State<TS, IEnumerable<TB>>, TA, TB>(f, @as);
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> ForM_<TS, TA, TB>(IEnumerable<TA> @as, Func<TA, State<TS, TB>> f) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TS>.MapM_<State<TS, Unit>, TA, TB>(f, @as);
        }

        public static State<TS, IEnumerable<TA>> ReplicateM<TS, TA>(int n, State<TS, TA> ma) 
        {
            return ma.ReplicateM(n);
        }

        public static State<TS, IEnumerable<TA>> ReplicateM<TS, TA>(this State<TS, TA> ma, int n) 
        {
            return MonadCombinators<TS>.ReplicateM<State<TS, IEnumerable<TA>>, TA>(n, ma);
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> ReplicateM_<TS, TA>(int n, State<TS, TA> ma) 
        // ReSharper restore InconsistentNaming
        {
            return ma.ReplicateM_(n);
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> ReplicateM_<TS, TA>(this State<TS, TA> ma, int n) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TS>.ReplicateM_<State<TS, Unit>, TA>(n, ma);
        }

        public static State<TS, TA> FoldM<TS, TA, TB>(Func<TA, TB, State<TS, TA>> f, TA a, IEnumerable<TB> bs) 
        {
            return (State<TS, TA>)MonadCombinators<TS>.FoldMInternal(f, a, bs, new StateMonadAdapter<TS>());
        }

        public static State<TS, TA> FoldM<TS, TA, TB>(Func<TA, TB, State<TS, TA>> f, TA a, params TB[] bs) 
        {
            return FoldM(f, a, bs.AsEnumerable());
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> FoldM_<TS, TA, TB>(Func<TA, TB, State<TS, TA>> f, TA a, IEnumerable<TB> bs) 
        // ReSharper restore InconsistentNaming
        {
            return (State<TS, Unit>)MonadCombinators<TS>.FoldMInternal_(f, a, bs, new StateMonadAdapter<TS>());
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> FoldM_<TS, TA, TB>(Func<TA, TB, State<TS, TA>> f, TA a, params TB[] bs) 
        // ReSharper restore InconsistentNaming
        {
            return FoldM_(f, a, bs.AsEnumerable());
        }

        public static State<TS, IEnumerable<TC>> ZipWithM<TS, TA, TB, TC>(Func<TA, TB, State<TS, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs) 
        {
            return MonadCombinators<TS>.ZipWithM<State<TS, IEnumerable<TC>>, TA, TB, TC>(f, @as, bs);
        }

        // ReSharper disable InconsistentNaming
        public static State<TS, Unit> ZipWithM_<TS, TA, TB, TC>(Func<TA, TB, State<TS, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TS>.ZipWithM_<State<TS, Unit>, TA, TB, TC>(f, @as, bs);
        }

        public static State<TS, IEnumerable<TA>> FilterM<TS, TA>(Func<TA, State<TS, bool>> p, IEnumerable<TA> @as) 
        {
            return (State<TS, IEnumerable<TA>>)MonadCombinators<TS>.FilterMInternal(p, @as, new StateMonadAdapter<TS>());
        }

        public static State<TS, IEnumerable<TA>> FilterM<TS, TA>(Func<TA, State<TS, bool>> p, params TA[] @as) 
        {
            return FilterM(p, @as.AsEnumerable());
        }

        public static State<TS, Unit> When<TS>(bool b, State<TS, Unit> m) 
        {
            return m.When(b);
        }

        public static State<TS, Unit> When<TS>(this State<TS, Unit> m, bool b) 
        {
            return MonadCombinators<TS>.When(b, m);
        }

        public static State<TS, Unit> Unless<TS>(bool b, State<TS, Unit> m) 
        {
            return m.Unless(b);
        }

        public static State<TS, Unit> Unless<TS>(this State<TS, Unit> m, bool b) 
        {
            return MonadCombinators<TS>.Unless(b, m);
        }

        public static State<TS, TB> Forever<TS, TA, TB>(this State<TS, TA> m) 
        {
            return (State<TS, TB>)MonadCombinators<TS>.Forever<TA, TB>(m);
        }

        public static State<TS, Unit> Void<TS, TA>(this State<TS, TA> m) 
        {
            return (State<TS, Unit>)MonadCombinators<TS>.Void(m);
        }

        public static State<TS, TB> Ap<TS, TA, TB>(State<TS, Func<TA, TB>> mf, State<TS, TA> ma) 
        {
            return ma.Ap(mf);
        }

        public static State<TS, TB> Ap<TS, TA, TB>(this State<TS, TA> ma, State<TS, Func<TA, TB>> mf) 
        {
            return MonadCombinators<TS>.Ap<State<TS, TB>, TA, TB>(mf, ma);
        }

        public static Func<TA, State<TS, TC>> Compose<TS, TA, TB, TC>(Func<TA, State<TS, TB>> f, Func<TB, State<TS, TC>> g) 
        {
            return a => (State<TS, TC>)MonadCombinators<TS>.Compose(f, g)(a);
        }

    }

    public static partial class Reader
    {
        public static Reader<TR, TB> Select<TR, TA, TB>(this Reader<TR, TA> ma, Func<TA, TB> f) 
        {
            return ma.Map(f);
        }

        public static Reader<TR, TB> SelectMany<TR, TA, TB>(this Reader<TR, TA> ma, Func<TA, Reader<TR, TB>> f) 
        {
            return ma.Bind(f);
        }

        public static Reader<TR, TC> SelectMany<TR, TA, TB, TC>(this Reader<TR, TA> ma, Func<TA, Reader<TR, TB>> f1, Func<TA, TB, TC> f2) 
        {
            return ma.Bind(
                a => f1(a).Bind(
                    b => Reader<TR>.Return(f2(a, b))));
        }

        public static Reader<TR, TB> Bind<TR, TA, TB>(this Reader<TR, TA> ma, Func<TA, Reader<TR, TB>> f) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (Reader<TR, TB>)monadAdapter.Bind(ma, f);
        }

        public static Reader<TR, TB> BindIgnoringLeft<TR, TA, TB>(this Reader<TR, TA> ma, Reader<TR, TB> mb) 
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (Reader<TR, TB>)monadAdapter.BindIgnoringLeft(ma, mb);
        }

        public static Reader<TR, TB> Map<TR, TA, TB>(Func<TA, TB> f, Reader<TR, TA> ma) 
        {
            return ma.Map(f);
        }
        
        public static Reader<TR, TB> Map<TR, TA, TB>(this Reader<TR, TA> ma, Func<TA, TB> f) 
        {
            return ma.LiftM(f);
        }
        
        public static Reader<TR, TB> FlatMap<TR, TA, TB>(Func<TA, Reader<TR, TB>> f, Reader<TR, TA> ma) 
        {
            return ma.Bind(f);
        }

        public static Reader<TR, TB> FlatMap<TR, TA, TB>(this Reader<TR, TA> ma, Func<TA, Reader<TR, TB>> f) 
        {
            return ma.Bind(f);
        }
        
        public static Reader<TR, TB> LiftM<TR, TA, TB>(Func<TA, TB> f, Reader<TR, TA> ma) 
        {
            return ma.LiftM(f);
        }

        public static Reader<TR, TB> LiftM<TR, TA, TB>(this Reader<TR, TA> ma, Func<TA, TB> f) 
        {
            return MonadCombinators<TR>.LiftM<Reader<TR, TB>, TA, TB>(f, ma);
        }

        public static Reader<TR, TC> LiftM2<TR, TA, TB, TC>(Func<TA, TB, TC> f, Reader<TR, TA> ma, Reader<TR, TB> mb) 
        {
            return ma.LiftM2(mb, f);
        }

        public static Reader<TR, TC> LiftM2<TR, TA, TB, TC>(this Reader<TR, TA> ma, Reader<TR, TB> mb, Func<TA, TB, TC> f) 
        {
            return MonadCombinators<TR>.LiftM2<Reader<TR, TC>, TA, TB, TC>(f, ma, mb);
        }

        public static Reader<TR, TD> LiftM3<TR, TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, Reader<TR, TA> ma, Reader<TR, TB> mb, Reader<TR, TC> mc) 
        {
            return ma.LiftM3(mb, mc, f);
        }

        public static Reader<TR, TD> LiftM3<TR, TA, TB, TC, TD>(this Reader<TR, TA> ma, Reader<TR, TB> mb, Reader<TR, TC> mc, Func<TA, TB, TC, TD> f) 
        {
            return MonadCombinators<TR>.LiftM3<Reader<TR, TD>, TA, TB, TC, TD>(f, ma, mb, mc);
        }

        public static Reader<TR, TE> LiftM4<TR, TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, Reader<TR, TA> ma, Reader<TR, TB> mb, Reader<TR, TC> mc, Reader<TR, TD> md) 
        {
            return ma.LiftM4(mb, mc, md, f);
        }

        public static Reader<TR, TE> LiftM4<TR, TA, TB, TC, TD, TE>(this Reader<TR, TA> ma, Reader<TR, TB> mb, Reader<TR, TC> mc, Reader<TR, TD> md, Func<TA, TB, TC, TD, TE> f) 
        {
            return MonadCombinators<TR>.LiftM4<Reader<TR, TE>, TA, TB, TC, TD, TE>(f, ma, mb, mc, md);
        }

        public static Reader<TR, TF> LiftM5<TR, TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, Reader<TR, TA> ma, Reader<TR, TB> mb, Reader<TR, TC> mc, Reader<TR, TD> md, Reader<TR, TE> me) 
        {
            return ma.LiftM5(mb, mc, md, me, f);
        }

        public static Reader<TR, TF> LiftM5<TR, TA, TB, TC, TD, TE, TF>(this Reader<TR, TA> ma, Reader<TR, TB> mb, Reader<TR, TC> mc, Reader<TR, TD> md, Reader<TR, TE> me, Func<TA, TB, TC, TD, TE, TF> f) 
        {
            return MonadCombinators<TR>.LiftM5<Reader<TR, TF>, TA, TB, TC, TD, TE, TF>(f, ma, mb, mc, md, me);
        }

        public static Reader<TR, TA> Join<TR, TA>(Reader<TR, Reader<TR, TA>> mma) 
        {
            return MonadCombinators<TR>.Join<Reader<TR, Reader<TR, TA>>, Reader<TR, TA>, TA>(mma);
        }

        public static Reader<TR, IEnumerable<TA>> Sequence<TR, TA>(IEnumerable<Reader<TR, TA>> ms) 
        {
            return MonadCombinators<TR>.Sequence<Reader<TR, IEnumerable<TA>>, TA>(ms);
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> Sequence_<TR, TA>(IEnumerable<Reader<TR, TA>> ms) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TR>.Sequence_<Reader<TR, Unit>, TA>(ms);
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> Sequence_<TR, TA>(params Reader<TR, TA>[] ms) 
        // ReSharper restore InconsistentNaming
        {
            return Sequence_(ms.AsEnumerable());
        }

        public static Reader<TR, IEnumerable<TB>> MapM<TR, TA, TB>(Func<TA, Reader<TR, TB>> f, IEnumerable<TA> @as)  
        {
            return MonadCombinators<TR>.MapM<Reader<TR, IEnumerable<TB>>, TA, TB>(f, @as);
        }

        public static Reader<TR, IEnumerable<TB>> MapM<TR, TA, TB>(Func<TA, Reader<TR, TB>> f, params TA[] @as) 
        {
            return MapM(f, @as.AsEnumerable());
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> MapM_<TR, TA, TB>(Func<TA, Reader<TR, TB>> f, IEnumerable<TA> @as) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TR>.MapM_<Reader<TR, Unit>, TA, TB>(f, @as);
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> MapM_<TR, TA, TB>(Func<TA, Reader<TR, TB>> f, params TA[] @as) 
        // ReSharper restore InconsistentNaming
        {
            return MapM_(f, @as.AsEnumerable());
        }

        public static Reader<TR, IEnumerable<TB>> ForM<TR, TA, TB>(IEnumerable<TA> @as, Func<TA, Reader<TR, TB>> f) 
        {
            return MonadCombinators<TR>.MapM<Reader<TR, IEnumerable<TB>>, TA, TB>(f, @as);
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> ForM_<TR, TA, TB>(IEnumerable<TA> @as, Func<TA, Reader<TR, TB>> f) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TR>.MapM_<Reader<TR, Unit>, TA, TB>(f, @as);
        }

        public static Reader<TR, IEnumerable<TA>> ReplicateM<TR, TA>(int n, Reader<TR, TA> ma) 
        {
            return ma.ReplicateM(n);
        }

        public static Reader<TR, IEnumerable<TA>> ReplicateM<TR, TA>(this Reader<TR, TA> ma, int n) 
        {
            return MonadCombinators<TR>.ReplicateM<Reader<TR, IEnumerable<TA>>, TA>(n, ma);
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> ReplicateM_<TR, TA>(int n, Reader<TR, TA> ma) 
        // ReSharper restore InconsistentNaming
        {
            return ma.ReplicateM_(n);
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> ReplicateM_<TR, TA>(this Reader<TR, TA> ma, int n) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TR>.ReplicateM_<Reader<TR, Unit>, TA>(n, ma);
        }

        public static Reader<TR, TA> FoldM<TR, TA, TB>(Func<TA, TB, Reader<TR, TA>> f, TA a, IEnumerable<TB> bs) 
        {
            return (Reader<TR, TA>)MonadCombinators<TR>.FoldMInternal(f, a, bs, new ReaderMonadAdapter<TR>());
        }

        public static Reader<TR, TA> FoldM<TR, TA, TB>(Func<TA, TB, Reader<TR, TA>> f, TA a, params TB[] bs) 
        {
            return FoldM(f, a, bs.AsEnumerable());
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> FoldM_<TR, TA, TB>(Func<TA, TB, Reader<TR, TA>> f, TA a, IEnumerable<TB> bs) 
        // ReSharper restore InconsistentNaming
        {
            return (Reader<TR, Unit>)MonadCombinators<TR>.FoldMInternal_(f, a, bs, new ReaderMonadAdapter<TR>());
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> FoldM_<TR, TA, TB>(Func<TA, TB, Reader<TR, TA>> f, TA a, params TB[] bs) 
        // ReSharper restore InconsistentNaming
        {
            return FoldM_(f, a, bs.AsEnumerable());
        }

        public static Reader<TR, IEnumerable<TC>> ZipWithM<TR, TA, TB, TC>(Func<TA, TB, Reader<TR, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs) 
        {
            return MonadCombinators<TR>.ZipWithM<Reader<TR, IEnumerable<TC>>, TA, TB, TC>(f, @as, bs);
        }

        // ReSharper disable InconsistentNaming
        public static Reader<TR, Unit> ZipWithM_<TR, TA, TB, TC>(Func<TA, TB, Reader<TR, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs) 
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TR>.ZipWithM_<Reader<TR, Unit>, TA, TB, TC>(f, @as, bs);
        }

        public static Reader<TR, IEnumerable<TA>> FilterM<TR, TA>(Func<TA, Reader<TR, bool>> p, IEnumerable<TA> @as) 
        {
            return (Reader<TR, IEnumerable<TA>>)MonadCombinators<TR>.FilterMInternal(p, @as, new ReaderMonadAdapter<TR>());
        }

        public static Reader<TR, IEnumerable<TA>> FilterM<TR, TA>(Func<TA, Reader<TR, bool>> p, params TA[] @as) 
        {
            return FilterM(p, @as.AsEnumerable());
        }

        public static Reader<TR, Unit> When<TR>(bool b, Reader<TR, Unit> m) 
        {
            return m.When(b);
        }

        public static Reader<TR, Unit> When<TR>(this Reader<TR, Unit> m, bool b) 
        {
            return MonadCombinators<TR>.When(b, m);
        }

        public static Reader<TR, Unit> Unless<TR>(bool b, Reader<TR, Unit> m) 
        {
            return m.Unless(b);
        }

        public static Reader<TR, Unit> Unless<TR>(this Reader<TR, Unit> m, bool b) 
        {
            return MonadCombinators<TR>.Unless(b, m);
        }

        public static Reader<TR, TB> Forever<TR, TA, TB>(this Reader<TR, TA> m) 
        {
            return (Reader<TR, TB>)MonadCombinators<TR>.Forever<TA, TB>(m);
        }

        public static Reader<TR, Unit> Void<TR, TA>(this Reader<TR, TA> m) 
        {
            return (Reader<TR, Unit>)MonadCombinators<TR>.Void(m);
        }

        public static Reader<TR, TB> Ap<TR, TA, TB>(Reader<TR, Func<TA, TB>> mf, Reader<TR, TA> ma) 
        {
            return ma.Ap(mf);
        }

        public static Reader<TR, TB> Ap<TR, TA, TB>(this Reader<TR, TA> ma, Reader<TR, Func<TA, TB>> mf) 
        {
            return MonadCombinators<TR>.Ap<Reader<TR, TB>, TA, TB>(mf, ma);
        }

        public static Func<TA, Reader<TR, TC>> Compose<TR, TA, TB, TC>(Func<TA, Reader<TR, TB>> f, Func<TB, Reader<TR, TC>> g) 
        {
            return a => (Reader<TR, TC>)MonadCombinators<TR>.Compose(f, g)(a);
        }

    }

    public static partial class Writer
    {
        public static Writer<TMonoid, TW, TB> Select<TMonoid, TW, TA, TB>(this Writer<TMonoid, TW, TA> ma, Func<TA, TB> f) where TMonoid : IMonoid<TW>
        {
            return ma.Map(f);
        }

        public static Writer<TMonoid, TW, TB> SelectMany<TMonoid, TW, TA, TB>(this Writer<TMonoid, TW, TA> ma, Func<TA, Writer<TMonoid, TW, TB>> f) where TMonoid : IMonoid<TW>
        {
            return ma.Bind(f);
        }

        public static Writer<TMonoid, TW, TC> SelectMany<TMonoid, TW, TA, TB, TC>(this Writer<TMonoid, TW, TA> ma, Func<TA, Writer<TMonoid, TW, TB>> f1, Func<TA, TB, TC> f2) where TMonoid : IMonoid<TW>
        {
            return ma.Bind(
                a => f1(a).Bind(
                    b => Writer<TMonoid, TW>.Return(f2(a, b))));
        }

        public static Writer<TMonoid, TW, TB> Bind<TMonoid, TW, TA, TB>(this Writer<TMonoid, TW, TA> ma, Func<TA, Writer<TMonoid, TW, TB>> f) where TMonoid : IMonoid<TW>
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (Writer<TMonoid, TW, TB>)monadAdapter.Bind(ma, f);
        }

        public static Writer<TMonoid, TW, TB> BindIgnoringLeft<TMonoid, TW, TA, TB>(this Writer<TMonoid, TW, TA> ma, Writer<TMonoid, TW, TB> mb) where TMonoid : IMonoid<TW>
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (Writer<TMonoid, TW, TB>)monadAdapter.BindIgnoringLeft(ma, mb);
        }

        public static Writer<TMonoid, TW, TB> Map<TMonoid, TW, TA, TB>(Func<TA, TB> f, Writer<TMonoid, TW, TA> ma) where TMonoid : IMonoid<TW>
        {
            return ma.Map(f);
        }
        
        public static Writer<TMonoid, TW, TB> Map<TMonoid, TW, TA, TB>(this Writer<TMonoid, TW, TA> ma, Func<TA, TB> f) where TMonoid : IMonoid<TW>
        {
            return ma.LiftM(f);
        }
        
        public static Writer<TMonoid, TW, TB> FlatMap<TMonoid, TW, TA, TB>(Func<TA, Writer<TMonoid, TW, TB>> f, Writer<TMonoid, TW, TA> ma) where TMonoid : IMonoid<TW>
        {
            return ma.Bind(f);
        }

        public static Writer<TMonoid, TW, TB> FlatMap<TMonoid, TW, TA, TB>(this Writer<TMonoid, TW, TA> ma, Func<TA, Writer<TMonoid, TW, TB>> f) where TMonoid : IMonoid<TW>
        {
            return ma.Bind(f);
        }
        
        public static Writer<TMonoid, TW, TB> LiftM<TMonoid, TW, TA, TB>(Func<TA, TB> f, Writer<TMonoid, TW, TA> ma) where TMonoid : IMonoid<TW>
        {
            return ma.LiftM(f);
        }

        public static Writer<TMonoid, TW, TB> LiftM<TMonoid, TW, TA, TB>(this Writer<TMonoid, TW, TA> ma, Func<TA, TB> f) where TMonoid : IMonoid<TW>
        {
            return MonadCombinators<TMonoid, TW>.LiftM<Writer<TMonoid, TW, TB>, TA, TB>(f, ma);
        }

        public static Writer<TMonoid, TW, TC> LiftM2<TMonoid, TW, TA, TB, TC>(Func<TA, TB, TC> f, Writer<TMonoid, TW, TA> ma, Writer<TMonoid, TW, TB> mb) where TMonoid : IMonoid<TW>
        {
            return ma.LiftM2(mb, f);
        }

        public static Writer<TMonoid, TW, TC> LiftM2<TMonoid, TW, TA, TB, TC>(this Writer<TMonoid, TW, TA> ma, Writer<TMonoid, TW, TB> mb, Func<TA, TB, TC> f) where TMonoid : IMonoid<TW>
        {
            return MonadCombinators<TMonoid, TW>.LiftM2<Writer<TMonoid, TW, TC>, TA, TB, TC>(f, ma, mb);
        }

        public static Writer<TMonoid, TW, TD> LiftM3<TMonoid, TW, TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, Writer<TMonoid, TW, TA> ma, Writer<TMonoid, TW, TB> mb, Writer<TMonoid, TW, TC> mc) where TMonoid : IMonoid<TW>
        {
            return ma.LiftM3(mb, mc, f);
        }

        public static Writer<TMonoid, TW, TD> LiftM3<TMonoid, TW, TA, TB, TC, TD>(this Writer<TMonoid, TW, TA> ma, Writer<TMonoid, TW, TB> mb, Writer<TMonoid, TW, TC> mc, Func<TA, TB, TC, TD> f) where TMonoid : IMonoid<TW>
        {
            return MonadCombinators<TMonoid, TW>.LiftM3<Writer<TMonoid, TW, TD>, TA, TB, TC, TD>(f, ma, mb, mc);
        }

        public static Writer<TMonoid, TW, TE> LiftM4<TMonoid, TW, TA, TB, TC, TD, TE>(Func<TA, TB, TC, TD, TE> f, Writer<TMonoid, TW, TA> ma, Writer<TMonoid, TW, TB> mb, Writer<TMonoid, TW, TC> mc, Writer<TMonoid, TW, TD> md) where TMonoid : IMonoid<TW>
        {
            return ma.LiftM4(mb, mc, md, f);
        }

        public static Writer<TMonoid, TW, TE> LiftM4<TMonoid, TW, TA, TB, TC, TD, TE>(this Writer<TMonoid, TW, TA> ma, Writer<TMonoid, TW, TB> mb, Writer<TMonoid, TW, TC> mc, Writer<TMonoid, TW, TD> md, Func<TA, TB, TC, TD, TE> f) where TMonoid : IMonoid<TW>
        {
            return MonadCombinators<TMonoid, TW>.LiftM4<Writer<TMonoid, TW, TE>, TA, TB, TC, TD, TE>(f, ma, mb, mc, md);
        }

        public static Writer<TMonoid, TW, TF> LiftM5<TMonoid, TW, TA, TB, TC, TD, TE, TF>(Func<TA, TB, TC, TD, TE, TF> f, Writer<TMonoid, TW, TA> ma, Writer<TMonoid, TW, TB> mb, Writer<TMonoid, TW, TC> mc, Writer<TMonoid, TW, TD> md, Writer<TMonoid, TW, TE> me) where TMonoid : IMonoid<TW>
        {
            return ma.LiftM5(mb, mc, md, me, f);
        }

        public static Writer<TMonoid, TW, TF> LiftM5<TMonoid, TW, TA, TB, TC, TD, TE, TF>(this Writer<TMonoid, TW, TA> ma, Writer<TMonoid, TW, TB> mb, Writer<TMonoid, TW, TC> mc, Writer<TMonoid, TW, TD> md, Writer<TMonoid, TW, TE> me, Func<TA, TB, TC, TD, TE, TF> f) where TMonoid : IMonoid<TW>
        {
            return MonadCombinators<TMonoid, TW>.LiftM5<Writer<TMonoid, TW, TF>, TA, TB, TC, TD, TE, TF>(f, ma, mb, mc, md, me);
        }

        public static Writer<TMonoid, TW, TA> Join<TMonoid, TW, TA>(Writer<TMonoid, TW, Writer<TMonoid, TW, TA>> mma) where TMonoid : IMonoid<TW>
        {
            return MonadCombinators<TMonoid, TW>.Join<Writer<TMonoid, TW, Writer<TMonoid, TW, TA>>, Writer<TMonoid, TW, TA>, TA>(mma);
        }

        public static Writer<TMonoid, TW, IEnumerable<TA>> Sequence<TMonoid, TW, TA>(IEnumerable<Writer<TMonoid, TW, TA>> ms) where TMonoid : IMonoid<TW>
        {
            return MonadCombinators<TMonoid, TW>.Sequence<Writer<TMonoid, TW, IEnumerable<TA>>, TA>(ms);
        }

        // ReSharper disable InconsistentNaming
        public static Writer<TMonoid, TW, Unit> Sequence_<TMonoid, TW, TA>(IEnumerable<Writer<TMonoid, TW, TA>> ms) where TMonoid : IMonoid<TW>
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TMonoid, TW>.Sequence_<Writer<TMonoid, TW, Unit>, TA>(ms);
        }

        // ReSharper disable InconsistentNaming
        public static Writer<TMonoid, TW, Unit> Sequence_<TMonoid, TW, TA>(params Writer<TMonoid, TW, TA>[] ms) where TMonoid : IMonoid<TW>
        // ReSharper restore InconsistentNaming
        {
            return Sequence_(ms.AsEnumerable());
        }

        public static Writer<TMonoid, TW, IEnumerable<TB>> MapM<TMonoid, TW, TA, TB>(Func<TA, Writer<TMonoid, TW, TB>> f, IEnumerable<TA> @as)  where TMonoid : IMonoid<TW>
        {
            return MonadCombinators<TMonoid, TW>.MapM<Writer<TMonoid, TW, IEnumerable<TB>>, TA, TB>(f, @as);
        }

        public static Writer<TMonoid, TW, IEnumerable<TB>> MapM<TMonoid, TW, TA, TB>(Func<TA, Writer<TMonoid, TW, TB>> f, params TA[] @as) where TMonoid : IMonoid<TW>
        {
            return MapM(f, @as.AsEnumerable());
        }

        // ReSharper disable InconsistentNaming
        public static Writer<TMonoid, TW, Unit> MapM_<TMonoid, TW, TA, TB>(Func<TA, Writer<TMonoid, TW, TB>> f, IEnumerable<TA> @as) where TMonoid : IMonoid<TW>
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TMonoid, TW>.MapM_<Writer<TMonoid, TW, Unit>, TA, TB>(f, @as);
        }

        // ReSharper disable InconsistentNaming
        public static Writer<TMonoid, TW, Unit> MapM_<TMonoid, TW, TA, TB>(Func<TA, Writer<TMonoid, TW, TB>> f, params TA[] @as) where TMonoid : IMonoid<TW>
        // ReSharper restore InconsistentNaming
        {
            return MapM_(f, @as.AsEnumerable());
        }

        public static Writer<TMonoid, TW, IEnumerable<TB>> ForM<TMonoid, TW, TA, TB>(IEnumerable<TA> @as, Func<TA, Writer<TMonoid, TW, TB>> f) where TMonoid : IMonoid<TW>
        {
            return MonadCombinators<TMonoid, TW>.MapM<Writer<TMonoid, TW, IEnumerable<TB>>, TA, TB>(f, @as);
        }

        // ReSharper disable InconsistentNaming
        public static Writer<TMonoid, TW, Unit> ForM_<TMonoid, TW, TA, TB>(IEnumerable<TA> @as, Func<TA, Writer<TMonoid, TW, TB>> f) where TMonoid : IMonoid<TW>
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TMonoid, TW>.MapM_<Writer<TMonoid, TW, Unit>, TA, TB>(f, @as);
        }

        public static Writer<TMonoid, TW, IEnumerable<TA>> ReplicateM<TMonoid, TW, TA>(int n, Writer<TMonoid, TW, TA> ma) where TMonoid : IMonoid<TW>
        {
            return ma.ReplicateM(n);
        }

        public static Writer<TMonoid, TW, IEnumerable<TA>> ReplicateM<TMonoid, TW, TA>(this Writer<TMonoid, TW, TA> ma, int n) where TMonoid : IMonoid<TW>
        {
            return MonadCombinators<TMonoid, TW>.ReplicateM<Writer<TMonoid, TW, IEnumerable<TA>>, TA>(n, ma);
        }

        // ReSharper disable InconsistentNaming
        public static Writer<TMonoid, TW, Unit> ReplicateM_<TMonoid, TW, TA>(int n, Writer<TMonoid, TW, TA> ma) where TMonoid : IMonoid<TW>
        // ReSharper restore InconsistentNaming
        {
            return ma.ReplicateM_(n);
        }

        // ReSharper disable InconsistentNaming
        public static Writer<TMonoid, TW, Unit> ReplicateM_<TMonoid, TW, TA>(this Writer<TMonoid, TW, TA> ma, int n) where TMonoid : IMonoid<TW>
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TMonoid, TW>.ReplicateM_<Writer<TMonoid, TW, Unit>, TA>(n, ma);
        }

        public static Writer<TMonoid, TW, TA> FoldM<TMonoid, TW, TA, TB>(Func<TA, TB, Writer<TMonoid, TW, TA>> f, TA a, IEnumerable<TB> bs) where TMonoid : IMonoid<TW>
        {
            return (Writer<TMonoid, TW, TA>)MonadCombinators<TMonoid, TW>.FoldMInternal(f, a, bs, new WriterMonadAdapter<TMonoid, TW>());
        }

        public static Writer<TMonoid, TW, TA> FoldM<TMonoid, TW, TA, TB>(Func<TA, TB, Writer<TMonoid, TW, TA>> f, TA a, params TB[] bs) where TMonoid : IMonoid<TW>
        {
            return FoldM(f, a, bs.AsEnumerable());
        }

        // ReSharper disable InconsistentNaming
        public static Writer<TMonoid, TW, Unit> FoldM_<TMonoid, TW, TA, TB>(Func<TA, TB, Writer<TMonoid, TW, TA>> f, TA a, IEnumerable<TB> bs) where TMonoid : IMonoid<TW>
        // ReSharper restore InconsistentNaming
        {
            return (Writer<TMonoid, TW, Unit>)MonadCombinators<TMonoid, TW>.FoldMInternal_(f, a, bs, new WriterMonadAdapter<TMonoid, TW>());
        }

        // ReSharper disable InconsistentNaming
        public static Writer<TMonoid, TW, Unit> FoldM_<TMonoid, TW, TA, TB>(Func<TA, TB, Writer<TMonoid, TW, TA>> f, TA a, params TB[] bs) where TMonoid : IMonoid<TW>
        // ReSharper restore InconsistentNaming
        {
            return FoldM_(f, a, bs.AsEnumerable());
        }

        public static Writer<TMonoid, TW, IEnumerable<TC>> ZipWithM<TMonoid, TW, TA, TB, TC>(Func<TA, TB, Writer<TMonoid, TW, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs) where TMonoid : IMonoid<TW>
        {
            return MonadCombinators<TMonoid, TW>.ZipWithM<Writer<TMonoid, TW, IEnumerable<TC>>, TA, TB, TC>(f, @as, bs);
        }

        // ReSharper disable InconsistentNaming
        public static Writer<TMonoid, TW, Unit> ZipWithM_<TMonoid, TW, TA, TB, TC>(Func<TA, TB, Writer<TMonoid, TW, TC>> f, IEnumerable<TA> @as, IEnumerable<TB> bs) where TMonoid : IMonoid<TW>
        // ReSharper restore InconsistentNaming
        {
            return MonadCombinators<TMonoid, TW>.ZipWithM_<Writer<TMonoid, TW, Unit>, TA, TB, TC>(f, @as, bs);
        }

        public static Writer<TMonoid, TW, IEnumerable<TA>> FilterM<TMonoid, TW, TA>(Func<TA, Writer<TMonoid, TW, bool>> p, IEnumerable<TA> @as) where TMonoid : IMonoid<TW>
        {
            return (Writer<TMonoid, TW, IEnumerable<TA>>)MonadCombinators<TMonoid, TW>.FilterMInternal(p, @as, new WriterMonadAdapter<TMonoid, TW>());
        }

        public static Writer<TMonoid, TW, IEnumerable<TA>> FilterM<TMonoid, TW, TA>(Func<TA, Writer<TMonoid, TW, bool>> p, params TA[] @as) where TMonoid : IMonoid<TW>
        {
            return FilterM(p, @as.AsEnumerable());
        }

        public static Writer<TMonoid, TW, Unit> When<TMonoid, TW>(bool b, Writer<TMonoid, TW, Unit> m) where TMonoid : IMonoid<TW>
        {
            return m.When(b);
        }

        public static Writer<TMonoid, TW, Unit> When<TMonoid, TW>(this Writer<TMonoid, TW, Unit> m, bool b) where TMonoid : IMonoid<TW>
        {
            return MonadCombinators<TMonoid, TW>.When(b, m);
        }

        public static Writer<TMonoid, TW, Unit> Unless<TMonoid, TW>(bool b, Writer<TMonoid, TW, Unit> m) where TMonoid : IMonoid<TW>
        {
            return m.Unless(b);
        }

        public static Writer<TMonoid, TW, Unit> Unless<TMonoid, TW>(this Writer<TMonoid, TW, Unit> m, bool b) where TMonoid : IMonoid<TW>
        {
            return MonadCombinators<TMonoid, TW>.Unless(b, m);
        }

        public static Writer<TMonoid, TW, TB> Forever<TMonoid, TW, TA, TB>(this Writer<TMonoid, TW, TA> m) where TMonoid : IMonoid<TW>
        {
            return (Writer<TMonoid, TW, TB>)MonadCombinators<TMonoid, TW>.Forever<TA, TB>(m);
        }

        public static Writer<TMonoid, TW, Unit> Void<TMonoid, TW, TA>(this Writer<TMonoid, TW, TA> m) where TMonoid : IMonoid<TW>
        {
            return (Writer<TMonoid, TW, Unit>)MonadCombinators<TMonoid, TW>.Void(m);
        }

        public static Writer<TMonoid, TW, TB> Ap<TMonoid, TW, TA, TB>(Writer<TMonoid, TW, Func<TA, TB>> mf, Writer<TMonoid, TW, TA> ma) where TMonoid : IMonoid<TW>
        {
            return ma.Ap(mf);
        }

        public static Writer<TMonoid, TW, TB> Ap<TMonoid, TW, TA, TB>(this Writer<TMonoid, TW, TA> ma, Writer<TMonoid, TW, Func<TA, TB>> mf) where TMonoid : IMonoid<TW>
        {
            return MonadCombinators<TMonoid, TW>.Ap<Writer<TMonoid, TW, TB>, TA, TB>(mf, ma);
        }

        public static Func<TA, Writer<TMonoid, TW, TC>> Compose<TMonoid, TW, TA, TB, TC>(Func<TA, Writer<TMonoid, TW, TB>> f, Func<TB, Writer<TMonoid, TW, TC>> g) where TMonoid : IMonoid<TW>
        {
            return a => (Writer<TMonoid, TW, TC>)MonadCombinators<TMonoid, TW>.Compose(f, g)(a);
        }

    }
}
