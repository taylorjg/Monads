
https://hackage.haskell.org/package/mtl-2.2.1/docs/src/Control-Monad-Reader.html

```Haskell
calculateContentLen :: Reader String Int
calculateContentLen = do
    content <- ask
    return (length content);

-- Calls calculateContentLen after adding a prefix to the Reader content.
calculateModifiedContentLen :: Reader String Int
calculateModifiedContentLen = local ("Prefix " ++) calculateContentLen

main = do
    let s = "12345";
    let modifiedLen = runReader calculateModifiedContentLen s
    let len = runReader calculateContentLen s
    putStrLn $ "Modified 's' length: " ++ (show modifiedLen)
    putStrLn $ "Original 's' length: " ++ (show len)
```

## Screenshot

![Screenshot](https://raw.githubusercontent.com/taylorjg/Monads/master/Images/ReaderHaskellDocsExample2.png "Screenshot")
