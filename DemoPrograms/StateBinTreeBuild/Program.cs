namespace StateBinTreeBuild
{
    internal class Program
    {
        private static void Main()
        {
            var xs = new[] {1, 2, 3, 4, 5};
            NonMonadicBuilder.Build(xs).Dump().ToConsole();
            NonMonadicBuilder2.Build(xs).Dump().ToConsole();
            MonadicBuilder.Build(xs).Dump().ToConsole();
            MonadicBuilder2.Build(xs).Dump().ToConsole();
        }
    }
}
