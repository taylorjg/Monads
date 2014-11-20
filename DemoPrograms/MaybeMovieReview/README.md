
http://book.realworldhaskell.org/read/programming-with-monads.html

```Haskell
data MovieReview = MovieReview {
      revTitle :: String
    , revUser :: String
    , revReview :: String
    }

lookup1 key alist = case lookup key alist of
                      Just (Just s@(_:_)) -> Just s
                      _ -> Nothing

liftedReview alist =
    liftM3 MovieReview (lookup1 "title" alist)
                       (lookup1 "user" alist)
                       (lookup1 "review" alist)
```

