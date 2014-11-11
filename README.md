
## Description

I am currently learning Haskell and trying to understand monads. As part of this learning process, I started playing around with monads in C#. 

Although it wasn't the original intention, it occurred to me that what I had implemented might actually form a useful monad library.

I have implemented the following monads:

* Maybe
* Either
* State
* Reader

I created my own simple <code>Unit</code> type to represent Haskell's <code>()</code> type.

I have also implemented some of the common monad functions:

* Sequence and Sequence_
* ReplicateM and ReplicateM_
* FoldM and FoldM_
* MapM and MapM_
* ZipWithM and ZipWithM_
* ForM
* FilterM
* LiftM through LiftM5
* Join
* When
* Unless
* Forever
* Void
* Compose (Left-to-right Kleisli composition of monads)

## NuGet

MonadLib is available as a NuGet package:

* http://www.nuget.org/packages/MonadLib/

## Design

I have tried to use the Haskell names as much as possible. I have tweaked these slightly
to match C# conventions e.g. <code>Sequence</code> instead of <code>sequence</code>. I have used the name <code>Bind</code> because C# does not allow me to use <code>>>=</code>.

Whilst I have an <code>IMonad</code> interface, it does not have the expected <code>Return</code> and <code>Bind</code>
members. Instead, these methods, along with the other common monad functions, are available
as extension methods. This allows these methods to take and return the correct types e.g.
<code>Maybe</code>'s <code>Bind</code> method returns a <code>Maybe</code> rather than an <code>IMonad</code>. Internally, these extension methods are actually wrappers around
common implementations of the monad functions with appropriate casting. Whilst it is tedious for me to write these wrappers, the hope is that this results in an API that is convenient and easy to use.

Most of my implementations of the common monad functions are noticably very similar to the
Haskell implementations. This was a deliberate goal and trying to achieve it guided my design choices.

## Demo Programs

* [MaybeMovieReview](https://github.com/taylorjg/Monads/tree/master/DemoPrograms/MaybeMovieReview)
* [EitherMovieReview](https://github.com/taylorjg/Monads/tree/master/DemoPrograms/EitherMovieReview)
* [StateZipWithIndex](https://github.com/taylorjg/Monads/tree/master/DemoPrograms/StateZipWithIndex)
* [StateBinTreeBuild](https://github.com/taylorjg/Monads/tree/master/DemoPrograms/StateBinTreeBuild)
* [StateGame](https://github.com/taylorjg/Monads/tree/master/DemoPrograms/StateGame)
* [ReaderBasicAsk](https://github.com/taylorjg/Monads/tree/master/DemoPrograms/ReaderBasicAsk)
* [ReaderBasicLocal](https://github.com/taylorjg/Monads/tree/master/DemoPrograms/ReaderBasicLocal)
* [ReaderHaskellDocsExample1](https://github.com/taylorjg/Monads/tree/master/DemoPrograms/ReaderHaskellDocsExample1)
* [ReaderHaskellDocsExample2](https://github.com/taylorjg/Monads/tree/master/DemoPrograms/ReaderHaskellDocsExample2)
* [ReaderAllAboutMonadsExample](https://github.com/taylorjg/Monads/tree/master/DemoPrograms/ReaderAllAboutMonadsExample)

## Usage Examples

This section is a "work in progress". I hope to add more examples. The unit tests also provide usage examples.

### Maybe

The following Maybe methods exist but are not yet covered by the examples below. The Haskell function names are shown in parentheses.

* FromMaybe (fromMaybe)
* ToEnumerable (maybeToList)
* ListToMaybe (listToMaybe)
* MapMaybe (mapMaybe)
* CatMaybes (catMaybes)
* MapOrDefault (maybe)

```C#
// Just and Nothing
var justInt = Maybe.Just(10);
var nothingString = Maybe.Nothing<string>();

// FromJust
var x = justInt.FromJust;

// Conversion from/to Nullable<T>
int? n1 = 10;
var mn = n1.ToMaybe();
var n2 = mn.ToNullable();

// Dictionary lookup
var dict = new Dictionary<int, string>
    {
        {1, "one"},
        {3, "three"}
    };
var mvJust = dict.GetValue(1);
var mvNothing = dict.GetValue(2);

// Basic pattern matching
Console.WriteLine("mvJust: {0}", mvJust.Match(a => a, () => "Nothing"));
Console.WriteLine("mvNothing: {0}", mvNothing.Match(a => a, () => "Nothing"));

// Bind
var ma = Maybe.Just(42);
var mb1 = ma.Bind(a => Maybe.Just(a * a)); // returns Maybe<int>
var mb2 = ma.Bind(a => Maybe.Return(a * a)); // returns Maybe<int>
var mb3 = ma.Bind(a => Maybe.Just(Convert.ToString(a * a))); // returns Maybe<string>
var mb4 = ma.Bind(a => Maybe.Return(Convert.ToString(a * a))); // returns Maybe<string>

// LiftM
var mc = Maybe.Just(12);
var md1 = mc.LiftM(a => a * a); // returns Maybe<int>
var md2 = mc.LiftM(a => Convert.ToString(a * a)); // returns Maybe<string>
```

The following example is my version of an example in the early part of
[chapter 15](http://book.realworldhaskell.org/read/programming-with-monads.html)
of [Real World Haskell](http://book.realworldhaskell.org/) (search for "liftedReview"):

```C#
using System;
using System.Collections.Generic;
using MonadLib;

namespace Monads
{
    using AssociationList = Dictionary<string, Maybe<string>>;

    public static class MaybeExample
    {
        public static void Demo()
        {
            var alist = new AssociationList
                {
                    {"title", Maybe.Just("Jaws")},
                    {"user", Maybe.Just("Jon")},
                    {"review", Maybe.Just("A film about a shark")}
                };

            // Using void Maybe.Match()
            GetMovieReview(alist).Match(
                movieReview => Console.WriteLine("GetMovieReview returned {0}.", MovieReview.Format(movieReview)),
                () => Console.WriteLine("GetMovieReview returned Nothing."));

            // Using T Maybe.Match<T>()
            Console.WriteLine(
                "GetMovieReview returned {0}.",
                GetMovieReview(alist).Match(
                    MovieReview.Format,
                    () => "Nothing"));
        }

        private static Maybe<MovieReview> GetMovieReview(AssociationList alist)
        {
            return Maybe.LiftM3(
                MovieReview.MakeMovieReview,
                Lookup(alist, "title"),
                Lookup(alist, "user"),
                Lookup(alist, "review"));
        }

        private static Maybe<string> Lookup(AssociationList alist, string key)
        {
            return alist
                .GetValue(key)
                .Bind(v => v.MFilter(s => !string.IsNullOrEmpty(s)));
        }
    }
}
```

### Either

The following Either methods exist but are not yet covered by the examples below. The Haskell function names are shown in parentheses.

* Lefts (lefts)
* Rights (rights)
* PartitionEithers (partitionEithers)
* MapEither (either)

```C#
// Either has two type parameters - TLeft and TA.
// This is a little trick to make EitherString an alias for Either<String>.
// EitherString is then a monad with a single type parameter - TA.
using EitherString = Either<String>;

// Creating a Left and Right
var eitherLeft = EitherString.Left<int>("an error message");
var eitherRight = EitherString.Right(10);

// Extracting values via Left and Right
var left = eitherLeft.Left;
var right = eitherRight.Right;

// Basic pattern matching
Console.WriteLine("eitherLeft: {0}", eitherLeft.Match(l => Convert.ToString(l), r => Convert.ToString(r)));
Console.WriteLine("eitherRight: {0}", eitherRight.Match(l => Convert.ToString(l), r => Convert.ToString(r)));

// Bind
var eitherRightSquared1 = eitherRight.Bind(r => EitherString.Right(r * r));
var eitherRightSquared2 = eitherRight.Bind(r => EitherString.Return(r * r));

// LiftM
var eitherRightSquared3 = eitherRight.LiftM(r => r * r);
```

### State

```C#
// State has two type parameters - TS and TA.
// This is a little trick to make StateString an alias for State<string>.
// StateString is then a monad with a single type parameter - TA.
using TickState = State<int>;

var tick = TickState
    .Get()
    .Bind(n => TickState
                    .Put(n + 1)
                    .BindIgnoringLeft(TickState.Return(n)));

Console.WriteLine("tick.EvalState(5): {0}", tick.EvalState(5));
Console.WriteLine("tick.ExecState(5): {0}", tick.ExecState(5));
```

### Reader

```C#
// Reader has two type parameters - TR and TA.
// This is a little trick to make ReaderConfig an alias for Reader<Config>.
// ReaderConfig is then a monad with a single type parameter - TA.
using ReaderConfig = Reader<Config>;

var config = new Config(2);

var reader1 = ReaderConfig
    .Ask()
    .Bind(c1 => ReaderConfig.Return(c1.Multiplier * 3));
Console.WriteLine("reader1.RunReader(config): {0}", reader1.RunReader(config));

var reader2 = ReaderConfig
    .Ask()
    .Local(c1 => new Config(c1.Multiplier * 2))
    .Bind(c2 => ReaderConfig.Return(c2.Multiplier * 3));
Console.WriteLine("reader2.RunReader(config): {0}", reader2.RunReader(config));
```

## Documentation

It is on my TODO list to add XML documentation comments to the source code and then subsequently to generate Web-based MSDN-style documentation using Sandcastle.

## TODO

* ~~Implement missing Maybe methods:~~
 * ~~maybe :: b -> (a -> b) -> Maybe a -> b~~
 * ~~mapMaybe :: (a -> Maybe b) -> [a] -> [b]~~
 * ~~maybeToList :: Maybe a -> [a]~~
 * ~~listToMaybe :: [a] -> Maybe a~~
 * ~~catMaybes :: [Maybe a] -> [a]~~
* ~~Implement missing Either methods:~~
 * ~~either :: (a -> c) -> (b -> c) -> Either a b -> c~~
 * ~~Data.Either.lefts :: [Either a b] -> [a]~~
 * ~~Data.Either.rights :: [Either a b] -> [b]~~
 * ~~Data.Either.partitionEithers :: [Either a b] -> ([a], [b])~~
* ~~Override Equals() and GetHashCode() on:~~
 * ~~Maybe<TA>~~
 * ~~Either<TE, TA>~~
* ~~Add an example to MonadApp showing the State monad in use~~
* ~~Add an example to MonadApp showing the Reader monad in use~~
* Implement more monad combinators e.g.:
 * ~~sequence~~
 * ~~sequence&#95;~~
 * ~~replicateM~~
 * ~~replicateM&#95;~~
 * ~~filterM~~
 * ~~foldM~~
 * ~~foldM&#95;~~
 * ~~zipWithM~~
 * ~~zipWithM&#95;~~
 * mapAndUnzipM
 * ~~mapM~~
 * ~~mapM&#95;~~
 * ~~forM~~
 * ~~forM&#95;~~
 * ~~liftM4~~
 * ~~liftM5~~
 * guard
 * ~~when~~
 * ~~unless~~
 * ~~join~~
 * ~~forever~~
 * ~~void~~
 * ~~ap~~
* Implement more monads e.g.:
 * ~~State~~
 * ~~Reader~~
 * Writer
* ~~Implement MonadPlus~~
* Implement some monad transformers ?
 * MaybeT
 * EitherT
 * StateT
 * ReaderT
 * WriterT
* Documentation
 * Add XML Documentation comments
 * Create Sandcastle documentation from the XML Documentation comments
 * Make the Sandcastle documentation available via a gh-pages branch of this repo - like the [Flinq documentation](http://taylorjg.github.io/Flinq/)
* ~~Make MonadLib available as a NuGet package~~

## Links

* http://www.haskell.org/haskellwiki/Monad
* http://www.haskell.org/haskellwiki/All_About_Monads
* http://en.wikibooks.org/wiki/Haskell/YAHT/Monads#Monadic_Combinators
* https://hackage.haskell.org/package/base-4.7.0.1/docs/Control-Monad.html
* https://hackage.haskell.org/package/base-4.7.0.1/docs/src/Control-Monad.html
* http://en.wikibooks.org/wiki/Haskell/Monad_transformers
* https://hackage.haskell.org/package/transformers-0.3.0.0/docs/Control-Monad-Trans-Maybe.html
* https://hackage.haskell.org/package/transformers-0.3.0.0/docs/src/Control-Monad-Trans-Maybe.html#MaybeT
