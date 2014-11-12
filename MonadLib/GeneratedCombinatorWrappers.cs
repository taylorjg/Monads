using System;
using System.Collections.Generic;

namespace MonadLib.Generated
{
	public static partial class Maybe
	{
		public static Maybe<TB> Bind<TA, TB>(this Maybe<TA> ma, Func<TA, Maybe<TB>> f)
		{
			var monadAdapter = ma.GetMonadAdapter();
			return (Maybe<TB>)monadAdapter.Bind(ma, f);
		}
	}

	public static partial class Either
	{
		public static Either<TLeft, TB> Bind<TLeft, TA, TB>(this Either<TLeft, TA> ma, Func<TA, Either<TLeft, TB>> f)
		{
			var monadAdapter = ma.GetMonadAdapter();
			return (Either<TLeft, TB>)monadAdapter.Bind(ma, f);
		}
	}

	public static partial class State
	{
		public static State<TS, TB> Bind<TS, TA, TB>(this State<TS, TA> ma, Func<TA, State<TS, TB>> f)
		{
			var monadAdapter = ma.GetMonadAdapter();
			return (State<TS, TB>)monadAdapter.Bind(ma, f);
		}
	}

	public static partial class Reader
	{
		public static Reader<TR, TB> Bind<TR, TA, TB>(this Reader<TR, TA> ma, Func<TA, Reader<TR, TB>> f)
		{
			var monadAdapter = ma.GetMonadAdapter();
			return (Reader<TR, TB>)monadAdapter.Bind(ma, f);
		}
	}
}
