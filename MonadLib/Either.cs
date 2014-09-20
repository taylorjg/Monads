using System;
using System.Collections.Generic;
using System.Linq;
using Flinq;

namespace MonadLib
{
    public sealed class Either<TE, TA> : IMonad<TE, TA>
    {
        private Either(LeftOrRight leftOrRight, TE e, TA a)
        {
            _leftOrRight = leftOrRight;
            _e = e;
            _a = a;
        }

        internal Either(TE e)
            : this(LeftOrRight.Left, e, default(TA))
        {
        }

        internal Either(TA a)
            : this(LeftOrRight.Right, default(TE), a)
        {
        }

        public bool IsLeft {
            get { return _leftOrRight == LeftOrRight.Left; }
        }

        public bool IsRight {
            get { return _leftOrRight == LeftOrRight.Right; }
        }

        public TE Left
        {
            get
            {
                if (!IsLeft) throw new InvalidOperationException("Left called on Either that is Right.");
                return _e;
            }
        }

        public TA Right
        {
            get
            {
                if (!IsRight) throw new InvalidOperationException("Right called on Either that is Left.");
                return _a;
            }
        }

        public void Match(Action<TE> leftAction, Action<TA> rightAction)
        {
            if (IsLeft)
                leftAction(Left);
            else
                rightAction(Right);
        }

        public T Match<T>(Func<TE, T> leftFunc, Func<TA, T> rightFunc)
        {
            return IsLeft ? leftFunc(Left) : rightFunc(Right);
        }

        private enum LeftOrRight
        {
            Left,
            Right
        }

        private readonly LeftOrRight _leftOrRight;
        private readonly TE _e;
        private readonly TA _a;

        private IMonadAdapter<TE> _monadAdapter;

        public IMonadAdapter<TE> GetMonadAdapter()
        {
            return _monadAdapter ?? (_monadAdapter = new EitherMonadAdapter<TE>());
        }
    }

    public static class Either
    {
        public static IEnumerable<TE> Lefts<TE, TA>(IEnumerable<Either<TE, TA>> eithers)
        {
            return eithers.Where(e => e.IsLeft).Select(e => e.Left);
        }

        public static IEnumerable<TA> Rights<TE, TA>(IEnumerable<Either<TE, TA>> eithers)
        {
            return eithers.Where(e => e.IsRight).Select(e => e.Right);
        }

        public static Tuple<IEnumerable<TE>, IEnumerable<TA>> PartitionEithers<TE, TA>(IEnumerable<Either<TE, TA>> eithers)
        {
            var z = Tuple.Create(
                System.Linq.Enumerable.Empty<TE>(),
                System.Linq.Enumerable.Empty<TA>());

            return eithers.FoldRight(z, (either, acc) => either.Match(
                e => Tuple.Create(System.Linq.Enumerable.Repeat(e, 1).Concat(acc.Item1), acc.Item2),
                a => Tuple.Create(acc.Item1, System.Linq.Enumerable.Repeat(a, 1).Concat(acc.Item2))));
        }

        public static TB Match<TE, TA, TB>(Func<TE, TB> f, Func<TA, TB> g, Either<TE, TA> either)
        {
            return either.Match(f, g);
        }

        public static Either<TE, TB> Bind<TE, TA, TB>(this Either<TE, TA> ma, Func<TA, Either<TE, TB>> f)
        {
            var monadAdapter = ma.GetMonadAdapter();
            return (Either<TE, TB>)monadAdapter.Bind(ma, f);
        }

        public static Either<TE, TB> LiftM<TE, TA, TB>(this Either<TE, TA> ma, Func<TA, TB> f)
        {
            return (Either<TE, TB>)MonadCombinators<TE>.LiftM(f, ma);
        }

        public static Either<TE, TB> LiftM<TE, TA, TB>(Func<TA, TB> f, Either<TE, TA> ma)
        {
            return (Either<TE, TB>)MonadCombinators<TE>.LiftM(f, ma);
        }

        public static Either<TE, TC> LiftM2<TE, TA, TB, TC>(this Either<TE, TA> ma, Either<TE, TB> mb, Func<TA, TB, TC> f)
        {
            return (Either<TE, TC>)MonadCombinators<TE>.LiftM2(f, ma, mb);
        }

        public static Either<TE, TC> LiftM2<TE, TA, TB, TC>(Func<TA, TB, TC> f, Either<TE, TA> ma, Either<TE, TB> mb)
        {
            return (Either<TE, TC>)MonadCombinators<TE>.LiftM2(f, ma, mb);
        }

        public static Either<TE, TD> LiftM3<TE, TA, TB, TC, TD>(this Either<TE, TA> ma, Either<TE, TB> mb, Either<TE, TC> mc, Func<TA, TB, TC, TD> f)
        {
            return (Either<TE, TD>)MonadCombinators<TE>.LiftM3(f, ma, mb, mc);
        }

        public static Either<TE, TD> LiftM3<TE, TA, TB, TC, TD>(Func<TA, TB, TC, TD> f, Either<TE, TA> ma, Either<TE, TB> mb, Either<TE, TC> mc)
        {
            return (Either<TE, TD>)MonadCombinators<TE>.LiftM3(f, ma, mb, mc);
        }

        public static Either<TE, IEnumerable<TA>> Sequence<TE, TA>(IEnumerable<Either<TE, TA>> ms)
        {
            return (Either<TE, IEnumerable<TA>>)MonadCombinators<TE>.Sequence(ms);
        }

        public static Either<TE, IEnumerable<TB>> MapM<TE, TA, TB>(Func<TA, Either<TE, TB>> f, IEnumerable<TA> @as)
        {
            return (Either<TE, IEnumerable<TB>>)MonadCombinators<TE>.MapM(f, @as);
        }

        public static Either<TE, IEnumerable<TA>> ReplicateM<TE, TA>(int n, Either<TE, TA> ma)
        {
            return (Either<TE, IEnumerable<TA>>)MonadCombinators<TE>.ReplicateM(n, ma);
        }

        public static Either<TE, TA> Join<TE, TA>(Either<TE, Either<TE, TA>> mma)
        {
            // Ideally, we would like to use MonadCombinators<TE>.Join(mma) but there
            // is a casting issue that I have figured out how to fix.
            var monadAdapter = mma.GetMonadAdapter();
            return (Either<TE, TA>)monadAdapter.Bind(mma, MonadHelpers.Identity);
        }
    }

    public static class Either<TE>
    {
        public static Either<TE, TA> Left<TA>(TE e)
        {
            return new Either<TE, TA>(e);
        }

        public static Either<TE, TA> Right<TA>(TA a)
        {
            return new Either<TE, TA>(a);
        }

        public static Either<TE, TA> Unit<TA>(TA a)
        {
            return Right(a);
        }
    }

    internal class EitherMonadAdapter<TE> : IMonadAdapter<TE>
    {
        public IMonad<TE, TA> Unit<TA>(TA a)
        {
            return Either<TE>.Right(a);
        }

        public IMonad<TE, TB> Bind<TA, TB>(IMonad<TE, TA> ma, Func<TA, IMonad<TE, TB>> f)
        {
            var eitherA = (Either<TE, TA>)ma;
            return eitherA.IsRight ? f(eitherA.Right) : Either<TE>.Left<TB>(eitherA.Left);
        }
    }
}
