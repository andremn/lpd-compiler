using System;
using System.Collections;
using System.Collections.Generic;

namespace LPD.Compiler.Lexical
{
    public sealed class TokenCollection : IEnumerable
    {
        private IList<Token> _tokens;

        public Token this[int index]
        {
            get { return _tokens[index]; }
        }

        public int Count
        {
            get { return _tokens.Count; }
        }

        public TokenCollection()
        {
            _tokens = new List<Token>();
        }

        public void Append(Token token)
        {
            _tokens.Add(token);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_tokens).GetEnumerator();
        }
    }
}