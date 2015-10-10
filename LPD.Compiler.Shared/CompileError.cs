using System;

namespace LPD.Compiler.Shared
{
    /// <summary>
    /// The exception that is thrown when the <see cref="LexicalAnalizer"/> finds an invalid token.
    /// </summary>
    public class CompileError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompileError"/> class with the specified program posistion.
        /// </summary>
        /// <param name="position">The position of the program this exception is being thrown.</param>
        public CompileError(CodePosition position)
        {
            Position = position;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompileError"/> class with the specified program posistion and message.
        /// </summary>
        /// <param name="position">The position of the program this exception is being thrown.</param>
        /// <param name="message">The exception message describing the error.</param>
        public CompileError(CodePosition position, string message)
        {
            Position = position;
            Message = message;
        }

        /// <summary>
        /// Gets or sets the position of the program this exception was thrown.
        /// </summary>
        public CodePosition Position { get; set; }

        public string Message { get; set; }
    }
}
