using System;
using System.IO;
using System.Text;

namespace LPD.Compiler.Lexical
{
    public class CharReader : IDisposable
    {
        private BinaryReader _reader;
        private char? _current;

        public CharReader(Stream stream, Encoding encoding)
        {
            _reader = new BinaryReader(stream, encoding, true);
        }

        public char? Current
        {
            get { return _current; }
        }

        public event EventHandler CharRead;

        public char? Read()
        {
            if (_reader.PeekChar() < 0)
            {
                return null;
            }

            _current = _reader.ReadChar();

            if (CharRead != null)
            {
                CharRead(this, EventArgs.Empty);
            }

            return _current;
        }

        public char? Peek()
        {
            return (char)_reader.PeekChar();
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
