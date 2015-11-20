using LPD.Compiler.Lexical;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LPD.Compiler.Syntactic
{
    internal class SyntaxTreeNode
    {
        private readonly IList<SyntaxTreeNode> _children;
        
        internal Symbols Value { get; set; }
        
        internal SyntaxTreeNode Parent { get; set; }

        internal IList<SyntaxTreeNode> Children
        {
            get { return _children; }
        }

        internal int Count
        {
            get { return _children.Count; }
        }

        internal SyntaxTreeNode()
        {
            _children = new List<SyntaxTreeNode>();
        }
    }
}
