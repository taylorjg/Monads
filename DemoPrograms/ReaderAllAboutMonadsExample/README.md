## Links

* https://www.haskell.org/haskellwiki/All_About_Monads#Example_7
* https://github.com/dag/all-about-monads/blob/master/examples/example16.hs

## Example of Reader monad in Haskell

```Haskell
-- This the abstract syntax representation of a template
--              Text       Variable     Quote        Include                   Compound
data Template = T String | V Template | Q Template | I Template [Definition] | C [Template]
data Definition = D Template Template
 
-- Our environment consists of an association list of named templates and
-- an association list of named variable values. 
data Environment = Env {templates::[(String,Template)],
                        variables::[(String,String)]}
 
-- lookup a variable from the environment
lookupVar :: String -> Environment -> Maybe String
lookupVar name env = lookup name (variables env)
 
-- lookup a template from the environment
lookupTemplate :: String -> Environment -> Maybe Template
lookupTemplate name env = lookup name (templates env)
 
-- add a list of resolved definitions to the environment
addDefs :: [(String,String)] -> Environment -> Environment
addDefs defs env = env {variables = defs ++ (variables env)}
 
-- resolve a Definition and produce a (name,value) pair
resolveDef :: Definition -> Reader Environment (String,String)
resolveDef (D t d) = do name <- resolve t
                        value <- resolve d
                        return (name,value)
 
-- resolve a template into a string
resolve :: Template -> Reader Environment (String)
resolve (T s)    = return s
resolve (V t)    = do varName  <- resolve t
                      varValue <- asks (lookupVar varName)
              return $ maybe "" id varValue
resolve (Q t)    = do tmplName <- resolve t
                      body     <- asks (lookupTemplate tmplName)
                      return $ maybe "" show body 
resolve (I t ds) = do tmplName <- resolve t
                      body     <- asks (lookupTemplate tmplName)
                      case body of
                        Just t' -> do defs <- mapM resolveDef ds
                                      local (addDefs defs) (resolve t')
                        Nothing -> return ""
resolve (C ts)   = (liftM concat) (mapM resolve ts)
```
