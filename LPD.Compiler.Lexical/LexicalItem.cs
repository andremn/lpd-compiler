using LPD.Compiler.Shared;

namespace LPD.Compiler.Lexical
{
    public class LexicalItem
    {
        public Token Token { get; set; }
        
        public CompileError Error { get; set; }
    }
}
