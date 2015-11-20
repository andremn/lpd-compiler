using LPD.Compiler.Lexical;
using System.Linq;

namespace LPD.Compiler.Syntactic
{
    internal static class FunctionHelper
    {
        public static bool AllPathsReturns(SyntaxTreeNode rootNode)
        {
            bool hasReturnOnRoot = rootNode.Children.Any(node => node.Value == Symbols.SRetorno);

            if (hasReturnOnRoot)
            {
                return true;
            }

            return InternalAllPathsReturns(rootNode);
        }

        private static bool InternalAllPathsReturns(SyntaxTreeNode node)
        {
            var foundReturn = true;

            foreach (var child in node.Children)
            {
                var rootReturn = child.Children.Any(childNode => childNode.Value == Symbols.SRetorno);

                if (rootReturn)
                {
                    return foundReturn & rootReturn;
                }

                foundReturn &= InternalAllPathsReturns(child);
            }

            return node.Children.Count > 0 && foundReturn;
        }
    }
}
