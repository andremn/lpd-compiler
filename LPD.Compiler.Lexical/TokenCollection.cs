using System.Collections;
using System.Collections.Generic;

namespace LPD.Compiler.Lexical
{
    /// <summary>
    /// Represents a collection of tokens.
    /// </summary>
    public sealed class TokenCollection : IEnumerable
    {
        private IList<Token> _tokens;

        /// <summary>
        /// Gets the token by the specified zero-based index.
        /// </summary>
        /// <param name="index">The zero-based index.</param>
        /// <returns><see cref="Token"/>.</returns>
        public Token this[int index]
        {
            get { return _tokens[index]; }
        }

        /// <summary>
        /// Gets the number of tokens in this collection.
        /// </summary>
        public int Count
        {
            get { return _tokens.Count; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCollection"/> class.
        /// </summary>
        public TokenCollection()
        {
            _tokens = new List<Token>();
        }

        /// <summary>
        /// Appends a new token to the end of the collection.
        /// </summary>
        /// <param name="token">The <see cref="Token"/> to be appended.</param>
        public void Append(Token token)
        {
            _tokens.Add(token);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_tokens).GetEnumerator();
        }
    }
}