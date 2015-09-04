using System;
using System.Collections;
using System.Collections.Generic;

namespace LPD.Compiler.Lexical
{
    public class TokenPositionCollection : IEnumerable<Tuple<Position, Token>>
    {
        private readonly IList<Tuple<Position, Token>> _internalTokens;

        internal TokenPositionCollection()
        {
            _internalTokens = new List<Tuple<Position, Token>>();
        }
        
        internal void Append(Position position, Token token)
        {
            _internalTokens.Add(new Tuple<Position, Token>(position, token));
        }

        public IEnumerator<Tuple<Position, Token>> GetEnumerator()
        {
            return _internalTokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalTokens.GetEnumerator();
        }
    }
}
