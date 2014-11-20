
http://book.realworldhaskell.org/read/programming-with-monads.html

```Haskell
runReader (ask >>= \x -> return (x * 3)) 2
```

