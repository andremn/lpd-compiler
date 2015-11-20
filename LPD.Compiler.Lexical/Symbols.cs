namespace LPD.Compiler.Lexical
{
    /// <summary>
    /// Represents the possible symbols a <see cref="LexicalAnalyzer"/> will identify.
    /// </summary>
    public enum Symbols
    {
        /// <summary>
        /// Begining of a program block.
        /// </summary>
        SInicio = 0,
        /// <summary>
        /// End of a program block.
        /// </summary>
        SFim = 1,
        /// <summary>
        /// Procedure.
        /// </summary>
        SProcedimento = 2,
        /// <summary>
        /// Function.
        /// </summary>
        SFuncao = 3,
        /// <summary>
        /// If.
        /// </summary>
        SSe = 4,
        /// <summary>
        /// The.
        /// </summary>
        SEntao = 5,
        /// <summary>
        /// Else.
        /// </summary>
        SSenao = 6,
        /// <summary>
        /// While.
        /// </summary>
        SEnquanto = 7,
        /// <summary>
        /// Do.
        /// </summary>
        SFaca = 8,
        /// <summary>
        /// Attribution.
        /// </summary>
        SAtribuicao = 9,
        /// <summary>
        /// Write.
        /// </summary>
        SEscreva = 10,
        /// <summary>
        /// Read.
        /// </summary>
        SLeia = 11,
        /// <summary>
        /// Var.
        /// </summary>
        SVar = 12,
        /// <summary>
        /// Integer.
        /// </summary>
        SInteiro = 13,
        /// <summary>
        /// Boolean.
        /// </summary>
        SBooleano = 14,
        /// <summary>
        /// Identificator.
        /// </summary>
        SIdentificador = 15,
        /// <summary>
        /// Number.
        /// </summary>
        SNumero = 16,
        /// <summary>
        /// Dot.
        /// </summary>
        SPonto = 17,
        /// <summary>
        /// Semicolon.
        /// </summary>
        SPontoVirgula = 18,
        /// <summary>
        /// Comma.
        /// </summary>
        SVirgula = 19,
        /// <summary>
        /// Opening parenthesis.
        /// </summary>
        SAbreParenteses = 20,
        /// <summary>
        /// Closing parenthesis.
        /// </summary>
        SFechaParenteses = 21,
        /// <summary>
        /// Greater than.
        /// </summary>
        SMaior = 22,
        /// <summary>
        /// Greater than or equal.
        /// </summary>
        SMaiorIg = 23,
        /// <summary>
        /// Equal.
        /// </summary>
        SIg = 24,
        /// <summary>
        /// Less than.
        /// </summary>
        SMenor = 25,
        /// <summary>
        /// Less than or equal.
        /// </summary>
        SMenorIg = 26,
        /// <summary>
        /// Not equal.
        /// </summary>
        SDif = 27,
        /// <summary>
        /// Plus.
        /// </summary>
        SMais = 28,
        /// <summary>
        /// Minus.
        /// </summary>
        SMenos = 29,
        /// <summary>
        /// Multiplication.
        /// </summary>
        SMult = 30,
        /// <summary>
        /// Division.
        /// </summary>
        SDiv = 31,
        /// <summary>
        /// And.
        /// </summary>
        SE = 32,
        /// <summary>
        /// Or.
        /// </summary>
        SOu = 33,
        /// <summary>
        /// Not.
        /// </summary>
        SNao = 34,
        /// <summary>
        /// Two points.
        /// </summary>
        SDoisPontos = 35,
        /// <summary>
        /// Begining of a program.
        /// </summary>
        SPrograma = 36,
        /// <summary>
        /// True.
        /// </summary>
        SVerdadeiro = 37,
        /// <summary>
        /// False.
        /// </summary>
        SFalso = 38,
        /// <summary>
        /// Unary plus.
        /// </summary>
        SMaisUnario = 39,
        /// <summary>
        /// Unary minus.
        /// </summary>
        SMenosUnario = 40,
        /// <summary>
        /// Function return. 
        /// </summary>
        SRetorno = 42,
        /// <summary>
        /// None.
        /// </summary>
        None = 41
    }
}
