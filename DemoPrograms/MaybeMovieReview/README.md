
http://book.realworldhaskell.org/read/programming-with-monads.html

```Haskell
import Control.Monad

data MovieReview = MovieReview {
	revTitle :: String,
	revUser :: String,
	revReview :: String
	} deriving Show

lookup1 :: Eq a => a -> [(a, Maybe [t])] -> Maybe [t]
lookup1 key alist =
	case lookup key alist of
		Just (Just s@(_:_)) -> Just s
		_ -> Nothing

liftedReview :: [([Char], Maybe [Char])] -> Maybe MovieReview
liftedReview alist =
    liftM3 MovieReview (lookup1 "title" alist)
                       (lookup1 "user" alist)
                       (lookup1 "review" alist)
main = do
	let alist = [
					("title", Just "Jaws"),
					("user", Just "Jon"),
					("review", Just "A film about a shark")
				]
	putStrLn $ show $ liftedReview alist
```

