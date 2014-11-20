* https://gist.github.com/davidallsopp/b7ecf8789efa584971c1
* http://learnyouahaskell.com/for-a-few-monads-more#writer

```Haskell
import Control.Monad.Writer
      
logNumber :: Int -> Writer [String] Int  
logNumber x = do
    tell ["Got number: " ++ show x]
    return x            
      
multWithLog :: Writer [String] Int  
multWithLog = do  
    a <- logNumber 3  
    b <- logNumber 5
    tell ["multiplying " ++ show a ++ " and " ++ show b ]
    return (a*b)
    
main :: IO ()
main = print $ runWriter multWithLog -- (15,["Got number: 3","Got number: 5","multiplying 3 and 5"])
```

