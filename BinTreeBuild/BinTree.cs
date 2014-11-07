namespace BinTreeBuild
{
    public abstract class BinTree
    {
        public string Padding(int level)
        {
            return System.Environment.NewLine + new string(' ', level * 2);
        }

        public abstract string Dump(int level = 0);
    }

    public class Leaf<TA> : BinTree
    {
        public TA Value { get; private set; }

        public Leaf(TA value)
        {
            Value = value;
        }

        public override string Dump(int _ = 0)
        {
            return string.Format("Leaf: {0}", Value);
        }
    }

    public class Fork : BinTree
    {
        public BinTree Left { get; private set; }
        public BinTree Right { get; private set; }

        public Fork(BinTree left, BinTree right)
        {
            Left = left;
            Right = right;
        }

        public override string Dump(int level = 0)
        {
            var nextLevel = level + 1;
            return string.Format(
                "Fork:{0}Left: {1}{2}Right: {3}",
                Padding(nextLevel),
                Left.Dump(nextLevel),
                Padding(nextLevel),
                Right.Dump(nextLevel));
        }
    }
}
