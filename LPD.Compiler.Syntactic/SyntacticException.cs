using System;

namespace LPD.Compiler.Syntactic
{
    public class SyntacticException : Exception
    {
        public SyntacticException()
        {
        }
        
        public SyntacticException(string message)
            : base(message)
        {
        }

        public SyntacticException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
