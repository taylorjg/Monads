
## Description

This demo program is a C# version of an example from "Thinking Functionally in Haskell" by Richard Bird (see page 249).
The idea is to build a binary tree given a list of values. Two implementations are presented - one in which the state
(the list of remaining values) is explicit and another in which a State monad is used to house the state.

```Haskell
import Control.Monad.State

data BinTree a =
    Leaf a |
    Fork (BinTree a) (BinTree a)
    deriving Show

nonMonadicBuild :: [a] -> BinTree a
nonMonadicBuild xs = fst (build2 (length xs) xs)
    where
        build2 :: Int -> [a] -> (BinTree a, [a])
        build2 1 xs = (Leaf (head xs), tail xs)
        build2 n xs = (Fork u v, xs'')
            where
                (u, xs') = build2 m xs
                (v, xs'') = build2 (n - m) xs'
                m = n `div` 2

monadicBuild :: [a] -> BinTree a
monadicBuild xs = evalState (build2 (length xs)) xs
    where
        build2 :: Int -> State [a] (BinTree a)
        build2 1 = do
                        x:xs <- get
                        put xs
                        return (Leaf x)
        build2 n = do
                        u <- build2 m
                        v <- build2 (n - m)
                        return (Fork u v)
                            where
                                m = n `div` 2

main = do
    let xs = [1,2,3,4,5]
    putStrLn $ show $ nonMonadicBuild xs
    putStrLn $ show $ monadicBuild xs
```

I give four implementations - two non-monadic and two monadic. Two of the implementations are quite close to
the Haskell implementations in that they split the list of values into head and tail. However, this is
probably not very efficient in C# because we have to keep building a new list containing the tail. So I have
another pair of implementations that maintain an index to identify the head item as the algorithm progresses.
