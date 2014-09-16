using System;

namespace Monads
{
    public sealed class Either<TE, TA> : IMonad<TA>
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

        public Either<TE, TB> Bind<TB>(Func<TA, Either<TE, TB>> f)
        {
            var monadAdapter = GetMonadAdapter<TE>();
            var mb = monadAdapter.Bind(this, f);
            return (Either<TE, TB>)mb;
        }

        public Either<TE, TB> LiftM<TB>(Func<TA, TB> f)
        {
            return (Either<TE, TB>)this.LiftM<TE, TA, TB>(f);
        }

        public IMonadAdapter GetMonadAdapter()
        {
            return null;
        }

        public IMonadAdapter<T1> GetMonadAdapter<T1>()
        {
            return new EitherMonadAdapter<T1>();
        }

        private enum LeftOrRight
        {
            Left,
            Right
        }

        private readonly LeftOrRight _leftOrRight;
        private readonly TE _e;
        private readonly TA _a;
    }

    internal class EitherMonadAdapter<TE> : IMonadAdapter<TE>
    {
        public IMonad<TA> Unit<TA>(TA a)
        {
            return Either<TE>.Right(a);
        }

        public IMonad<TB> Bind<TA, TB>(IMonad<TA> ma, Func<TA, IMonad<TB>> f)
        {
            var eitherA = (Either<TE, TA>)ma;
            return eitherA.IsRight ? f(eitherA.Right) : Either<TE>.Left<TB>(eitherA.Left);
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
}
