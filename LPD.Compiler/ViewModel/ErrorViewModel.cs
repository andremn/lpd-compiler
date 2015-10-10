using LPD.Compiler.Lexical;
using LPD.Compiler.Shared;

namespace LPD.Compiler.ViewModel
{
    /// <summary>
    /// The view model for displaying errors informations.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the position of the error within the code.
        /// </summary>
        public CodePosition Position { get; set; }

        /// <summary>
        /// Gets or sets the error's message.
        /// </summary>
        public string Message { get; set; }
    }
}
