
## Description

I am currently learning Haskell and trying to understand monads. As part of this learning process, I am started playing around with monads in C#. 

Although it wasn't really the original intention, it occurred to me that what I had implemented might actually form a useful monad library. I have implemented the following monads:

* Maybe
* Either
* State
* Reader

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

## Design

I have tried to use the Haskell names as much as possible. I have tweaked these slightly
to match C# conventions e.g. <code>Sequence</code> instead of <code>sequence</code>. I have used the name <code>Bind</code> because C# does not allow me to use <code>(>>=)</code>.

Whilst I have an <code>IMonad</code> interface, it does not have the expected <code>Return</code> and <code>Bind</code>
members. Instead, these methods, along with the other common monad functions, are available
as extension methods. This allows these methods to take and return the correct types e.g.
<code>Maybe&lt;TA&gt;.Bind(...)</code> returns a <code>Maybe&lt;TA&gt;</code> rather than an <code>IMonad&lt;TA&gt;</code>. Internally, these extension methods are actually wrappers around
common implementations of the monad functions with appropriate casting. Whilst it is tedious for me to write these wrappers, the hope is that this results in a convenient API.


## Usage Examples

### Maybe

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
var mb = ma.Bind(a => Maybe.Just(Convert.ToString(a * a)));

// LiftM
var mc = Maybe.Just(12);
var md = mc.LiftM(a => Convert.ToString(a * a));
```

### Either



### State



### Reader



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
* Override Equals() and GetHashCode() on:
 * Maybe<TA>
 * Either<TE, TA>
* Add an example to MonadApp showing the State monad in use
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
 * Reader
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
* Make MonadLib available as a NuGet package

## Links

* http://www.haskell.org/haskellwiki/Monad
* http://www.haskell.org/haskellwiki/All_About_Monads
* http://en.wikibooks.org/wiki/Haskell/YAHT/Monads#Monadic_Combinators
* https://hackage.haskell.org/package/base-4.7.0.1/docs/Control-Monad.html
* https://hackage.haskell.org/package/base-4.7.0.1/docs/src/Control-Monad.html
* http://en.wikibooks.org/wiki/Haskell/Monad_transformers
* https://hackage.haskell.org/package/transformers-0.3.0.0/docs/Control-Monad-Trans-Maybe.html
* https://hackage.haskell.org/package/transformers-0.3.0.0/docs/src/Control-Monad-Trans-Maybe.html#MaybeT
