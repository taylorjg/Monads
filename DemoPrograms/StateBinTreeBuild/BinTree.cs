namespace StateBinTreeBuild
{
    public abstract class BinTree
    {
        public string Padding(int level)
        {
            return System.Environment.NewLine + new string(' ', level * 2);
        }

        public abstract string Dump();
    }

    public class Leaf<TA> : BinTree
    {
        public TA Value { get; private set; }

        public Leaf(TA value)
        {
            Value = value;
        }

        public override string Dump()
        {
            return string.Format("Leaf {0}", Value);
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

        public override string Dump()
        {
            return string.Format("Fork ({0}) ({1})", Left.Dump(), Right.Dump());
        }
    }
}
