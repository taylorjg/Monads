
http://book.realworldhaskell.org/read/programming-with-monads.html

This Either demo program is based on the same Haskell example as the MaybeMovieReview demo program.
I have adapted the lookup function to return an Either instead of a Maybe in order to demo the
capabilities of the Either monad.

```Haskell
import Control.Monad

data MovieReview = MovieReview {
	revTitle :: String,
	revUser :: String,
	revReview :: String
	} deriving Show

lookup1 :: (Eq a, Show a) => a -> [(a, Maybe [t])] -> Either String [t]
lookup1 key alist =
	case lookup key alist of
		Just x ->
			case x of
				Just s@(_:_) -> Right s
				Just _ -> Left $ "Found key " ++ show key ++ " but its value is empty"
				_ -> Left $ "Found key " ++ show key ++ " but it has no value"
		_ -> Left $ "Failed to find a value for key " ++ show key

liftedReview :: [(String, Maybe String)] -> Either String MovieReview
liftedReview alist =
    liftM3 MovieReview (lookup1 "title" alist)
                       (lookup1 "user" alist)
                       (lookup1 "review" alist)
                       
main = do
	-- All keys present and correct
	putStrLn $ show $ liftedReview [
					("title", Just "Jaws"),
					("user", Just "Jon"),
					("review", Just "A film about a shark")]

	-- Missing "user" key
	putStrLn $ show $ liftedReview [
					("title", Just "Jaws"),
					("review", Just "A film about a shark")]

	-- Value of "user" key is empty
	putStrLn $ show $ liftedReview [
					("title", Just "Jaws"),
					("user", Just ""),
					("review", Just "A film about a shark")]

	-- Value of "user" key is Nothing
	putStrLn $ show $ liftedReview [
					("title", Just "Jaws"),
					("user", Nothing),
					("review", Just "A film about a shark")]
```

