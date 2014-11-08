using System.Collections.Immutable;

namespace StateBinTreeBuild
{
    internal class Program
    {
        private static void Main()
        {
            var xs = ImmutableList.Create(new[] { 1, 2, 3, 4, 5 });
            NonMonadicBuilder.Build(xs).Dump().ToConsole();
            MonadicBuilder.Build(xs).Dump().ToConsole();
            MonadicBuilder2.Build(xs).Dump().ToConsole();
        }
    }
}
