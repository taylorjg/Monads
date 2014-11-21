
http://book.realworldhaskell.org/read/monad-transformers.html

```Haskell
import Control.Monad.Reader

myName step = do
	name <- ask
	return (step ++ ", I am " ++ name)

localExample :: Reader String (String, String, String)
localExample = do
	a <- myName "First"
	b <- local (++"dy") (myName "Second")
	c <- myName "Third"
	return (a, b, c)

main = do
	putStrLn $ show $ runReader localExample "Fred"
```

## Screenshot

![Screenshot](https://raw.githubusercontent.com/taylorjg/Monads/master/Images/ReaderBasicLocal.png "Screenshot")
