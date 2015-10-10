using System.Diagnostics;

namespace LPD.Compiler.Lexical
{
    /// <summary>
    /// Represent a token.
    /// </summary>
#if DEBUG
    [DebuggerDisplay("{Symbol} {Lexeme}")]
#endif
    public struct Token
    {
        /// <summary>
        /// Gets an empty token.
        /// </summary>
        public static readonly Token Empty = new Token() { Symbol = Symbols.None, Lexeme = null };

        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        public Symbols Symbol { get; set; }

        /// <summary>
        /// Gets or sets the lexeme.
        /// </summary>
        public string Lexeme { get; set; }

        /// <summary>
        /// Returns a string representation for this object.
        /// </summary>
        /// <returns>Símbolo: <see cref="Token.Symbol"/> | Lexema: <see cref="Token.Lexeme"/></returns>
        public override string ToString()
        {
            return "Símbolo: " + Symbol + " | Lexema: " + Lexeme;
        }
    }
}
