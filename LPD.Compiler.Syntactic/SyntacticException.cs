using System;

namespace LPD.Compiler.Syntactic
{
    /// <summary>
    /// Represents the general exception thrown for compilation errors.
    /// </summary>
    public class CompilationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationException"/>.
        /// </summary>
        public CompilationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationException"/> with the specified message.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        public CompilationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationException"/> with the specified message and a inner exception.
        /// </summary>
        /// <param name="message">The message describing the exception.</param>
        /// <param name="innerException">The root exception.</param>
        public CompilationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
