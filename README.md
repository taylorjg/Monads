
## Description

I am currently learning Haskell and trying to understand monads. As part of this learning process, I
am playing around with monads in C#.

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
 * filterM
 * foldM
 * foldM&#95;
 * zipWithM
 * zipWithM_
 * mapAndUnzipM
 * ~~mapM~~
 * ~~mapM&#95;~~
 * forM
 * forM&#95;
 * ~~liftM4~~
 * ~~liftM5~~
 * guard
 * when
 * unless
 * ~~join~~
 * forever
 * void
 * ap
* Implement more monads e.g.:
 * Reader
 * Writer
 * ~~State~~
 * IO
* Implement MonadPlus
* Implement some monad transformers ?
 * MaybeT
 * ReaderT
 * WriterT
 * StateT
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