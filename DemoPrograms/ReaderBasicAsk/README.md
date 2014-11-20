
http://book.realworldhaskell.org/read/programming-with-monads.html

```Haskell
import Control.Monad.Reader

main =
	putStrLn $ show $ runReader (ask >>= \x -> return (x * 3)) 2
```

