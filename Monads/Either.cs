using System;

namespace Monads
{
    public sealed class Either<TE, TA> : IMonad<TA>
    {
        internal static Either<TE, TA> MakeLeft(TE e)
        {
            return new Either<TE, TA>
                {
                    _leftOrRight = LeftOrRight.Left,
                    _e = e
                };
        }

        internal static Either<TE, TA> MakeRight(TA a)
        {
            return new Either<TE, TA>
                {
                    _leftOrRight = LeftOrRight.Right,
                    _a = a
                };
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

        private LeftOrRight _leftOrRight;
        private TE _e;
        private TA _a;
    }

    internal class EitherMonadAdapter<TE> : IMonadAdapter<TE>
    {
        public IMonad<TA> Unit<TA>(TA a)
        {
            return Either.Right<TE, TA>(a);
        }

        public IMonad<TB> Bind<TA, TB>(IMonad<TA> ma, Func<TA, IMonad<TB>> f)
        {
            var eitherA = (Either<TE, TA>)ma;
            return eitherA.IsRight ? f(eitherA.Right) : Either.Left<TE, TB>(eitherA.Left);
        }
    }

    public static class Either
    {
        public static Either<TE, TA> Left<TE, TA>(TE e)
        {
            return Either<TE, TA>.MakeLeft(e);
        }

        public static Either<TE, TA> Right<TE, TA>(TA a)
        {
            return Either<TE, TA>.MakeRight(a);
        }

        public static Either<TE, TA> Unit<TE, TA>(TA a)
        {
            return Right<TE, TA>(a);
        }
    }
}
