using System;
using System.IO;
using System.Text;

namespace LPD.Compiler.Lexical
{
    public class CharReader : IDisposable
    {
        private BinaryReader _reader;
        private char? _current;

        public CharReader(string filePath, Encoding encoding)
        {
            FileStream fileStream = File.OpenRead(filePath);

            _reader = new BinaryReader(fileStream, encoding, false);
        }

        public char? Current
        {
            get { return _current; }
        }

        public event EventHandler CharRead;

        public char? Read()
        {
            if (!Peek().HasValue)
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
            if (_reader.PeekChar() < 0)
            {
                return null;
            }

            return (char)_reader.PeekChar();
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
