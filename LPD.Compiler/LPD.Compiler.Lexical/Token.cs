namespace LPD.Compiler.Lexical
{
    public struct Token
    {
        public Symbols Symbol { get; set; }

        public string Lexeme { get; set; }

        public override string ToString()
        {
            return Lexeme + " " + Symbol;
        }
    }
}
