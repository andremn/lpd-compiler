using System;

namespace LPD.Compiler.Lexical
{
    /// <summary>
    /// The exception that is thrown when the <see cref="LexicalAnalizer"/> finds an invalid token.
    /// </summary>
    public class InvalidTokenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTokenException"/> class with the specified program posistion.
        /// </summary>
        /// <param name="position">The position of the program this exception is being thrown.</param>
        public InvalidTokenException(Position position)
        {
            Position = position;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTokenException"/> class with the specified program posistion and message.
        /// </summary>
        /// <param name="position">The position of the program this exception is being thrown.</param>
        /// <param name="message">The exception message describing the error.</param>
        public InvalidTokenException(Position position, string message)
            : base(message)
        {
            Position = position;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTokenException"/> class with the specified program posistion, message and the inner exception.
        /// </summary>
        /// <param name="position">The position of the program this exception is being thrown.</param>
        /// <param name="message">The exception message describing the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public InvalidTokenException(Position position, string message, Exception innerException)
            : base(message, innerException)
        {
            Position = position;
        }

        /// <summary>
        /// Gets or sets the position of the program this exception was thrown.
        /// </summary>
        public Position Position { get; set; }
    }
}
