using System;
using System.IO;
using System.Text;

namespace LPD.Compiler.Lexical
{
    /// <summary>
    /// A file reader that reads one character at time.
    /// </summary>
    public class CharReader : IDisposable
    {
        private BinaryReader _reader;
        private char? _current;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharReader"/> class with the specified file and econding.
        /// </summary>
        /// <param name="filePath">The file to read the characters from.</param>
        /// <param name="encoding">The enconding used to read the characters from the file.</param>
        public CharReader(string filePath, Encoding encoding)
        {
            FileStream fileStream = File.OpenRead(filePath);

            _reader = new BinaryReader(fileStream, encoding, false);
        }

        /// <summary>
        /// Gets the last read character.
        /// </summary>
        public char? Current
        {
            get { return _current; }
        }

        /// <summary>
        /// Raised when a char is read.
        /// </summary>
        public event EventHandler CharRead;

        /// <summary>
        /// Reads the next character from the file.
        /// </summary>
        /// <returns>The read char, if any; null if the end of the file was reached.</returns>
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

        /// <summary>
        /// Returns the next char from the file without consuming it.
        /// </summary>
        /// <returns>The read char, if any; null if the end of the file was reached.</returns>
        public char? Peek()
        {
            if (_reader.PeekChar() < 0)
            {
                return null;
            }

            return (char)_reader.PeekChar();
        }

        /// <summary>
        /// Releases the resources used by this object.
        /// </summary>
        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
