using System.Diagnostics;

namespace LPD.Compiler.Lexical
{
#if DEBUG
    [DebuggerDisplay("{Symbol} {Lexeme}")]
#endif
    public struct Token
    {
        public static readonly Token Empty = new Token() { Symbol = Symbols.None, Lexeme = null };

        public Symbols Symbol { get; set; }

        public string Lexeme { get; set; }

        public override string ToString()
        {
            return Symbol + " " + Lexeme;
        }
    }
}
