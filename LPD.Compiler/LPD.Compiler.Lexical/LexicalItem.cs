namespace LPD.Compiler.Lexical
{
    public class LexicalItem
    {
        public Token Token { get; set; }
        
        public InvalidTokenError Error { get; set; }
    }
}
