using LPD.Compiler.Shared;

namespace LPD.Compiler.Lexical
{
    /// <summary>
    /// Represents an item read by a <see cref="LexicalAnalyzer"/>.
    /// </summary>
    public class LexicalItem
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public Token Token { get; set; }
        
        /// <summary>
        /// Gets or sets the compilation error.
        /// </summary>
        public CompileError Error { get; set; }
    }
}
