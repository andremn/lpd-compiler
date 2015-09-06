using System;
using System.Collections;
using System.Collections.Generic;

namespace LPD.Compiler.Lexical
{
    public class TokenPositionCollection : IEnumerable<Tuple<CodePosition, Token>>
    {
        private readonly IList<Tuple<CodePosition, Token>> _internalTokens;

        internal TokenPositionCollection()
        {
            _internalTokens = new List<Tuple<CodePosition, Token>>();
        }
        
        internal void Append(CodePosition position, Token token)
        {
            _internalTokens.Add(new Tuple<CodePosition, Token>(position, token));
        }

        public IEnumerator<Tuple<CodePosition, Token>> GetEnumerator()
        {
            return _internalTokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalTokens.GetEnumerator();
        }
    }
}
