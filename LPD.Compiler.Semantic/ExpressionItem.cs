using LPD.Compiler.Lexical;
using LPD.Compiler.SymbolsTable;
using System.Diagnostics;

namespace LPD.Compiler.Semantic
{
    [DebuggerDisplay("{Symbol} {Lexeme} {Type}")]
    internal struct ExpressionItem
    {
        public Symbols Symbol { get; set; }

        public string Lexeme { get; set; }

        public ItemType Type { get; set; }
    }
}
