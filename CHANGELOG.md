
<a name="1.1.1.0"></a>
### 1.1.1.0  (29 November 2014)

#### Bug Fixes

* Bug in the implementation of Reader's LiftM wrapper - it was using Either instead of Reader due to a copy/paste error. This was picked up whilst introducing the use of T4 to generate these wrappers thus eliminating the need for copy/paste.
* Several of the State combinator wrappers were using the wrong monad adapter: Sequence, Sequence&#95;, MapM, MapM&#95;, ForM, ForM&#95;, Void. Again, the introduction of T4 should have eliminated these errors now.

#### Features

* New Monads
    * Writer
* Monoids:
    * ListMonoid
* New Monad combinators:
    * Map (internally, it calls LiftM)
    * FlatMap (internally, it calls Bind)
* New overloads for existing Monad combinators so that they can be called as extension methods:
    * ReplicateM
    * ReplicateM&#95;
    * When
    * Unless
    * Forever
    * Void
    * Ap
    * MapOrDefault (method on Maybe)
* New overloads for existing Monad combinators so that they can be called with IEnumerable&lt;T&gt; or params T[]:
    * Sequence
    * Sequence&#95;
    * MapM
    * MapM&#95;
    * FoldM
    * FoldM&#95;
    * FilterM
* New MonadPlus combinators:
    * Guard
    * MSum
* Overhaul of existing demo programs and addition of new ones including a couple of extended demo programs
* Now using T4 to generate monad combinator wrappers
* Added more unit tests

<a name="1.0.1.0"></a>
### 1.0.1.0  (29 September 2014)

#### Features

* Monads
    * Maybe
    * Either
    * State
    * Reader
* Monad combinators:
    * Sequence
    * Sequence&#95;
    * ReplicateM
    * ReplicateM&#95;
    * FilterM
    * FoldM
    * FoldM&#95;
    * ZipWithM
    * ZipWithM&#95;
    * MapM
    * MapM&#95;
    * ForM
    * ForM&#95;
    * LiftM
    * LiftM2
    * LiftM3
    * LiftM4
    * LiftM5
    * Join
    * When
    * Unless
    * Forever
    * Void
    * Ap
* MonadPlus combinators:
	* MFilter
