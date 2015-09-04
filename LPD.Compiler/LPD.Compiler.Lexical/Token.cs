using System.Diagnostics;

namespace LPD.Compiler.Lexical
{
#if DEBUG
    [DebuggerDisplay("{Symbol Lexeme}")]
#endif
    public struct Token
    {
        public Symbols Symbol { get; set; }

        public string Lexeme { get; set; }
    }
}
