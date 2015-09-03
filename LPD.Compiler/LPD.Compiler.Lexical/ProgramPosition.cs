namespace LPD.Compiler.Lexical
{
    /// <summary>
    /// Represents a position in the source code.
    /// </summary>
    public struct ProgramPosition
    {
        /// <summary>
        /// Gets or sets the line.
        /// </summary>
        public ushort Line { get; set; }

        /// <summary>
        /// Gets or sets the column.
        /// </summary>
        public ushort Column { get; set; }
    }
}
