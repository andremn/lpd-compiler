namespace LPD.Compiler.Shared
{
    /// <summary>
    /// Contains informations of an unsuccessful compilation.
    /// </summary>
    public class CompilationError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationError"/> class with the specified program posistion.
        /// </summary>
        /// <param name="position">The position of the program this exception is being thrown.</param>
        public CompilationError(CodePosition position)
        {
            Position = position;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationError"/> class with the specified program posistion and message.
        /// </summary>
        /// <param name="position">The position of the program this exception is being thrown.</param>
        /// <param name="message">The exception message describing the error.</param>
        public CompilationError(CodePosition position, string message)
        {
            Position = position;
            Message = message;
        }

        /// <summary>
        /// Gets or sets the position of the program this exception was thrown.
        /// </summary>
        public CodePosition Position { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; }
    }
}
