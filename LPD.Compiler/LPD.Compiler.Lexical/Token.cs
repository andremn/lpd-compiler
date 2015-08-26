namespace LPD.Compiler.Lexical
{
    public struct Token
    {
        public Symbols Symbol { get; set; }

        public string Lexema { get; set; }

        public override string ToString()
        {
            return Symbol.ToString() + " " + Lexema;
        }
    }
}
