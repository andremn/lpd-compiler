using System.Collections;
using System.Collections.Generic;

namespace LPD.Compiler.Lexical
{
    public class TokenCollection : IEnumerable<Token>
    {
        private readonly IList<Token> _internalTokens;

        public TokenCollection()
        {
            _internalTokens = new List<Token>();
        }

        public void Append(Token token)
        {
            _internalTokens.Add(token);
        }

        public IEnumerator<Token> GetEnumerator()
        {
            return _internalTokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalTokens.GetEnumerator();
        }
    }
}
