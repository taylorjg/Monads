
This demo program is a translation of listing 11.8 in [Functional Programming in Scala](http://www.manning.com/bjarnason/) (pages 202-203) to C#. The following Scala code is adapted from this listing. It uses the State monad from [scalaz](https://github.com/scalaz/scalaz) instead of the State monad that is developed in the book.

```Scala
import scalaz._
import scalaz.State._
import scalaz.StateT._

object ZipWithIndex {

	def main(args: Array[String]): Unit = {
		println(zipWithIndex(List("A", "B", "C")))
	}

	private def zipWithIndex[A](as: List[A]): List[(Int,A)] = {
		val F = stateMonad[Int];
		val z = F.pure(List[(Int,A)]())
		val m = as.foldLeft(z)((acc,a) => for {
			xs <- acc
			n <- get
			_ <- put(n + 1)
		} yield (n,a) :: xs)
		m.eval(0).reverse
	}
}
```

## Screenshot

![Screenshot](https://raw.githubusercontent.com/taylorjg/Monads/master/Images/StateZipWithIndex.png "Screenshot")
