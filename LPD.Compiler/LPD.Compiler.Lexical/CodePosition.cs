using System.Diagnostics;

namespace LPD.Compiler.Lexical
{
#if DEBUG
    [DebuggerDisplay("{Line, Column}")]
#endif
    /// <summary>
    /// Represents a position in the source code.
    /// </summary>
    public struct CodePosition
    {
        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        public ushort Line { get; set; }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        public ushort Column { get; set; }

        /// <summary>
        /// Gets or sets the absolute char position within the text.
        /// </summary>
        public int Index { get; set; }

        public override bool Equals(object obj)
        {
            return (obj is CodePosition) && (this == (CodePosition)obj);
        }

        public override int GetHashCode()
        {
            return Line.GetHashCode() ^ Column.GetHashCode();
        }

        public static bool operator ==(CodePosition position1, CodePosition position2)
        {
            return (position1.Line == position2.Line) && (position1.Column == position2.Column);
        }

        public static bool operator !=(CodePosition position1, CodePosition position2)
        {
            return !(position1 == position2);
        }
    }
}
