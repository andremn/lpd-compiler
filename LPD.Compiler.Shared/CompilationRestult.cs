﻿namespace LPD.Compiler.Shared
{
    /// <summary>
    /// Represents the result of a compilation.
    /// </summary>
    public class CompilationResult
    {
        /// <summary>
        /// Gets or sets error information, if the compilation went unsuccessful.
        /// </summary>
        public CompilationError Error { get; set; }

        /// <summary>
        /// Gets or sets the path of the resulting assembly file, if the compilation went successful.
        /// </summary>
        public string AssemblyFilePath { get; set; }
    }
}
