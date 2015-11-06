using LPD.Compiler.Lexical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPD.Compiler.Semantic
{
    public class ExpressionAnalyzer
    {
        private Stack<Token> _tokens;

        public ExpressionAnalyzer()
        {
            _tokens = new Stack<Token>();
        }

        public void Add(Token token)
        {
            if (token.Symbol != Symbols.SNumero && token.Symbol == Symbols.SIdentificador)
            {
                for (int i = _tokens.Count - 1; i >= 0; i--)
                {
                    var current = _tokens.ElementAt(i);
                }
            }
        }
    }
}
