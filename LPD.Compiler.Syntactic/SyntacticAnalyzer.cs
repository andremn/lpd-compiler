using LPD.Compiler.Lexical;
using LPD.Compiler.Semantic;
using LPD.Compiler.Shared;
using LPD.Compiler.SymbolsTable;
using System;
using static LPD.Compiler.Syntactic.Properties.Resources;

namespace LPD.Compiler.Syntactic
{
    /// <summary>
    /// Represents the syntactic analyzer. In other words, this is the parser.
    /// </summary>
    public class SyntacticAnalyzer
    {
        private LexicalAnalyzer _lexical;
        private Token _token;
        private CodePosition? _position = null;
        private ExpressionAnalyzer _expressionAnalyzer;
        private VectorSymbolTable _symbolTable;
        private string _analyzingLexeme = null;
        private string _currentFunctionLexeme = null;
        private bool _foundFuntionReturn = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntacticAnalyzer"/> class with the specified <see cref="LexicalAnalyzer"/>.
        /// </summary>
        /// <param name="lexical">The lexical analyzer.</param>
        public SyntacticAnalyzer(LexicalAnalyzer lexical)
        {
            if (lexical == null)
            {
                throw new ArgumentNullException(nameof(lexical));
            }

            _lexical = lexical;
            _symbolTable = new VectorSymbolTable();
        }

        /// <summary>
        /// Performs the code parsing.
        /// </summary>
        /// <returns></returns>
        public CompileError DoAnalysis()
        {
            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            try
            {
                if (_token.Symbol == Symbols.SPrograma)
                {
                    if (!NextToken())
                    {
                        RaiseUnexpectedEndOfFileMessage();
                    }

                    if (_token.Symbol == Symbols.SIdentificador)
                    {
                        if (!NextToken())
                        {
                            RaiseUnexpectedEndOfFileMessage();
                        }

                        if (_token.Symbol == Symbols.SPontoVirgula)
                        {
                            AnalyzeBlock();

                            if (_token.Symbol == Symbols.SPonto)
                            {
                                if (NextToken())
                                {
                                    RaiseUnexpectedTokenError("fim do arquivo");
                                }
                            }
                            else
                            {
                                RaiseUnexpectedTokenError("\".\"");
                            }
                        }
                        else
                        {
                            RaiseMissingSemicolonError();
                        }
                    }
                    else
                    {
                        RaiseUnexpectedTokenError("identificador");
                    }
                }
                else
                {
                    RaiseUnexpectedTokenError("'programa'");
                }
            }
            catch (CompilationException ex)
            {
                int column = _lexical.Position.Column - _token.Lexeme.Length;
                
                if (_position.HasValue)
                {
                    return new CompileError(_position.Value, ex.Message);
                }

                return new CompileError(_lexical.Position, ex.Message);
            }

            return null;
        }

        private bool NextToken()
        {
            LexicalItem item = new LexicalItem();

            if (!_lexical.GetToken(out item))
            {
                return false;
            }

            if (item.Error != null)
            {
                RaiseLexicalErrorMessage(item.Error.Message);
            }

            _token = item.Token;
            return true;
        }
        private void RaiseInvalidTerm()
        {
            int column = _lexical.Position.Column - _token.Lexeme.Length;

            _position = null;
            throw new CompilationException(string.Format(MissingInicioErrorMessage, _lexical.Position.Line, _lexical.Position.Line, column));
        }

        private void RaiseMissingInicioError()
        {
            int column = _lexical.Position.Column - _token.Lexeme.Length;

            _position = null;
            throw new CompilationException(string.Format(MissingInicioErrorMessage, _lexical.Position.Line, column));
        }

        private void RaiseMissingSemicolonError()
        {
            ushort column = (ushort)(_lexical.ReadTokens[_lexical.ReadTokens.Count - 1].Lexeme.Length);
            ushort line = (ushort)(_lexical.Position.Line - 1);

            _position = new CodePosition() { Line = line, Column = column };
            throw new CompilationException(string.Format(UnexpectedTokenErrorMessage, line, column, "\";\""));
        }

        private void RaiseUnexpectedTokenError(string message)
        {
            int column = _lexical.Position.Column - _token.Lexeme.Length;

            _position = null;
            throw new CompilationException(string.Format(UnexpectedTokenErrorMessage, _lexical.Position.Line, column, message));
        }

        private void RaiseLexicalErrorMessage(string message)
        {
            int column = _lexical.Position.Column - _token.Lexeme.Length;

            _position = null;
            throw new CompilationException(string.Format(LexicalErrorMessage, _lexical.Position.Line, column, message));
        }

        private void RaiseUnexpectedEndOfFileMessage()
        {
            _position = null;
            throw new CompilationException(string.Format(UnexpectedEndOfFileErrorMessage, _lexical.Position.Line, _lexical.Position.Column));
        }

        private void RaiseInvalidToken()
        {
            _position = null;
            throw new CompilationException(string.Format(InvalidTokenErrorMessage, _lexical.Position.Line, _lexical.Position.Column,_token.Lexeme));
        }

        private void RaiseMissingFunctionReturn()
        {
            var line = _lexical.Position.Line - 1;

            throw new CompilationException(string.Format(MissingFunctionReturnMessage, line, 0, _currentFunctionLexeme));
        }

        private void AnalyzeBlock()
        {
            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            AnalyzeVarsDcl();
            AnalyzeSubRoutines();
            AnalyzeCommands();
        }

        private void AnalyzeVars()
        {
            do
            {
                if (_token.Symbol == Symbols.SIdentificador)
                {
                    _symbolTable.Insert(new IdentificatorItem() { Lexeme = _token.Lexeme });

                    if (!NextToken())
                    {
                        RaiseUnexpectedEndOfFileMessage();
                    }

                    if (_token.Symbol == Symbols.SVirgula || _token.Symbol == Symbols.SDoisPontos)
                    {
                        if (_token.Symbol == Symbols.SVirgula)
                        {
                            if (!NextToken())
                            {
                                RaiseUnexpectedEndOfFileMessage();
                            }

                            if (_token.Symbol == Symbols.SDoisPontos)
                            {
                                RaiseLexicalErrorMessage("Esperado identificador");
                            }
                        }
                    }
                    else
                    {
                        RaiseUnexpectedTokenError("\",\" ou \":\"");
                    }
                }
            } while (_token.Symbol != Symbols.SDoisPontos);

            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            AnalyzeType();
        }

        private void AnalyzeVarsDcl()
        {
            if (_token.Symbol == Symbols.SVar)
            {
                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }
                if (_token.Symbol == Symbols.SIdentificador)
                {
                    while (_token.Symbol == Symbols.SIdentificador)
                    {
                        AnalyzeVars();

                        if (_token.Symbol == Symbols.SPontoVirgula)
                        {
                            if (!NextToken())
                            {
                                RaiseMissingSemicolonError();
                            }
                        }
                        else
                        {
                            RaiseMissingSemicolonError();
                        }
                    }
                }
                else
                {
                    RaiseUnexpectedTokenError("identificador");
                }
            }
        }

        private void AnalyzeType()
        {
            if (_token.Symbol != Symbols.SInteiro && _token.Symbol != Symbols.SBooleano)
            {
                RaiseUnexpectedTokenError("\"inteiro\" ou \"booleano\"");
            }
            else
            {
                _symbolTable.SetTypeLastestVars(_token.Symbol == Symbols.SInteiro ? ItemType.Integer : ItemType.Boolean);

                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }
            }
        }

        private void AnalyzeCommands()
        {
            if (_token.Symbol == Symbols.SInicio)
            {
                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }

                AnalyzeSimpleCommand();
                
                while (_token.Symbol != Symbols.SFim)
                {
                    if (_token.Symbol == Symbols.SPontoVirgula)
                    {
                        if (!NextToken())
                        {
                            RaiseUnexpectedEndOfFileMessage();
                        }

                        if (_token.Symbol != Symbols.SFim)
                        {
                            AnalyzeSimpleCommand();
                        }
                    }
                    else
                    {
                        RaiseMissingSemicolonError();
                    }
                }

                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }
            }
            else
            {
                RaiseInvalidToken();
            }
        }

        private void AnalyzeSimpleCommand()
        {
            if (_token.Symbol == Symbols.SIdentificador)
            {
                var item = _symbolTable.Search(_token.Lexeme);

                if (item == null)
                {
                    RaiseNotFoundIdentificatorError(_token.Lexeme);
                }

                _analyzingLexeme = _token.Lexeme;

                var funcItem = item as FunctionItem;

                if (funcItem?.Lexeme == _currentFunctionLexeme)
                {
                    if (!NextToken())
                    {
                        RaiseUnexpectedEndOfFileMessage();
                    }

                    if (_token.Symbol == Symbols.SAtribuicao)
                    {
                        AnalyzeAttribution();
                    }

                    _foundFuntionReturn = true;
                }
                else
                {
                    if (!NextToken())
                    {
                        RaiseUnexpectedEndOfFileMessage();
                    }

                    if (_token.Symbol == Symbols.SAtribuicao)
                    {
                        AnalyzeAttribution();
                    }
                    else
                    {
                        ProcCallAnalyze();
                    }
                }
            }
            else if (_token.Symbol == Symbols.SSe)
            {
                AnalyzeIf();
            }
            else if (_token.Symbol == Symbols.SEnquanto)
            {
                AnalyzeWhile();
            }
            else if (_token.Symbol == Symbols.SLeia)
            {
                AnalyzeRead();
            }
            else if (_token.Symbol == Symbols.SEscreva)
            {
                AnalyzeWrite();
            }
            else
            {
                AnalyzeCommands();
            }
        }

        private void RaiseNotFoundIdentificatorError(string lexeme)
        {
            ushort column = _lexical.Position.Column;
            ushort line = _lexical.Position.Line;

            throw new CompilationException(string.Format(NotFoundIdentifierErrorMessage, line, column, lexeme));
        }

        private void ProcCallAnalyze()
        {
        }

        private void AnalyzeRead()
        {
            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            if (_token.Symbol == Symbols.SAbreParenteses)
            {
                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }

                if (_token.Symbol == Symbols.SIdentificador)
                {
                    // doesnt exist yet!
                    /*
                    if(pesquisa_declvar_tabela(token.lexema))
                    {
                        
                    }
                    else
                    {
                        throw new SyntacticException();
                    }*/

                    if (!NextToken())
                    {
                        RaiseUnexpectedEndOfFileMessage();
                    }

                    if (_token.Symbol == Symbols.SFechaParenteses)
                    {
                        if (!NextToken())
                        {
                            RaiseUnexpectedEndOfFileMessage();
                        }
                    }
                    else
                    {
                        RaiseUnexpectedTokenError("\")\"");
                    }
                }
                else
                {
                    RaiseUnexpectedTokenError("identificador");
                }
            }
            else
            {
                RaiseUnexpectedTokenError("\"(\"");
            }
        }

        private void AnalyzeWrite()
        {
            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }
            if (_token.Symbol == Symbols.SAbreParenteses)
            {
                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }

                if (_token.Symbol == Symbols.SIdentificador)
                {
                    // doesnt exist yet!
                    /*
                    if(Search(token.lexema))
                    {
                        
                    }
                    else
                    {
                        throw new SyntacticException();
                    }
                    */
                    if (!NextToken())
                    {
                        RaiseUnexpectedEndOfFileMessage();
                    }

                    if (_token.Symbol == Symbols.SFechaParenteses)
                    {
                        if (!NextToken())
                        {
                            RaiseUnexpectedEndOfFileMessage();
                        }
                    }
                    else
                    {
                        RaiseUnexpectedTokenError("\")\"");
                    }
                }
                else
                {
                    RaiseUnexpectedTokenError("identificador");
                }
            }
            else
            {
                RaiseUnexpectedTokenError("\"(\"");
            }

        }

        private void AnalyzeWhile()
        {
            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            var type = AnalyzeExpressionType();

            if (_token.Symbol != Symbols.SFaca)
            {
                RaiseUnexpectedTokenError("\"faca\"");
            }

            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            AnalyzeSimpleCommand();
        }

        private void AnalyzeIf()
        {
            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }
            
            var expressionType = AnalyzeExpressionType();

            if (expressionType != ItemType.Boolean)
            {
                ushort column = _lexical.Position.Column;
                ushort line = _lexical.Position.Line;

                throw new CompilationException(string.Format(IncompatibleIfExpressionErrorMessage, line, column));
            }

            if (_token.Symbol != Symbols.SEntao)
            {
                RaiseUnexpectedTokenError("\"entao\"");
            }

            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            AnalyzeSimpleCommand();

            if (_token.Symbol == Symbols.SSenao)
            {
                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }

                AnalyzeSimpleCommand();
            }
        }

        private void AnalyzeSubRoutines()
        {
            if (_token.Symbol == Symbols.SProcedimento || _token.Symbol == Symbols.SFuncao)
            {
                //Todo: code generator
            }

            while (_token.Symbol == Symbols.SProcedimento || _token.Symbol == Symbols.SFuncao)
            {
                if (_token.Symbol == Symbols.SProcedimento)
                {
                    AnalyzeProcDcl();
                }
                else
                {
                    AnalyzeFuncDcl();
                }

                if (_token.Symbol == Symbols.SPontoVirgula)
                {
                    if (!NextToken())
                    {
                        RaiseUnexpectedEndOfFileMessage();
                    }
                }
                else
                {
                    RaiseMissingSemicolonError();
                }
            }
        }

        private void AnalyzeProcDcl()
        {
            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            if (_token.Symbol != Symbols.SIdentificador)
            {
                RaiseUnexpectedTokenError("identificador");
            }

            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            if (_token.Symbol != Symbols.SPontoVirgula)
            {
                RaiseMissingSemicolonError();
            }

            AnalyzeBlock();
        }

        private void AnalyzeFuncDcl()
        {
            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            if (_token.Symbol != Symbols.SIdentificador)
            {
                RaiseUnexpectedTokenError("identificador");
            }

            var funcItem = new FunctionItem() { Lexeme = _token.Lexeme };

            _currentFunctionLexeme = _token.Lexeme;

            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            if (_token.Symbol != Symbols.SDoisPontos)
            {
                RaiseUnexpectedTokenError("\":\"");
            }

            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            if (_token.Symbol != Symbols.SInteiro && _token.Symbol != Symbols.SBooleano)
            {
                RaiseUnexpectedTokenError("\"inteiro\" ou \"booleano\"");
            }

            var type = _token.Symbol == Symbols.SInteiro ? ItemType.Integer : ItemType.Boolean;

            funcItem.Type = type;
            _symbolTable.Insert(funcItem);

            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            if (_token.Symbol != Symbols.SPontoVirgula)
            {
                RaiseMissingSemicolonError();
            }

            AnalyzeBlock();

            if (!_foundFuntionReturn)
            {
                RaiseMissingFunctionReturn();
            }  

            _foundFuntionReturn = false;
            _currentFunctionLexeme = null;
        }

        private void AnalyzeExpression()
        {
            AnalyzeSimpleExpression();

            if (_token.Symbol == Symbols.SMaior ||
                _token.Symbol == Symbols.SMaiorIg ||
                _token.Symbol == Symbols.SMenor ||
                _token.Symbol == Symbols.SMenorIg ||
                _token.Symbol == Symbols.SIg ||
                _token.Symbol == Symbols.SDif)
            {
                _expressionAnalyzer.Add(_token);

                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }

                AnalyzeSimpleExpression();
            }
        }

        private void AnalyzeSimpleExpression()
        {
            if (_token.Symbol == Symbols.SMais || _token.Symbol == Symbols.SMenos)
            {
                var token = new Token() { Lexeme = _token.Lexeme };

                if (_token.Symbol == Symbols.SMais)
                {
                    token.Symbol = Symbols.SMaisUnario;
                }
                else
                {
                    token.Symbol = Symbols.SMenosUnario;
                }

                _expressionAnalyzer.Add(token);

                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }
            }

            AnalyzeTerm();

            while (_token.Symbol == Symbols.SMais || _token.Symbol == Symbols.SMenos || _token.Symbol == Symbols.SOu)
            {
                _expressionAnalyzer.Add(_token);

                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }

                AnalyzeTerm();
            }
        }

        private void AnalyzeTerm()
        {
            AnalyzeFactor();

            if (_token.Symbol == Symbols.SIdentificador)
            {
                RaiseUnexpectedTokenError($"um operador, mas o identificador '{_token.Lexeme}' foi encontrado no lugar");
            }

            while (_token.Symbol == Symbols.SMult || _token.Symbol == Symbols.SDiv || _token.Symbol == Symbols.SE)
            {
                _expressionAnalyzer.Add(_token);

                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }

                AnalyzeFactor();
            }
        }

        private void AnalyzeFactor()
        {
            if (_token.Symbol == Symbols.SIdentificador)
            {
                AnalyzeFuncCall();
            }
            else if (_token.Symbol == Symbols.SNumero)
            {
                _expressionAnalyzer.Add(_token);

                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }
            }
            else if (_token.Symbol == Symbols.SNao)
            {
                _expressionAnalyzer.Add(_token);

                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }

                AnalyzeFactor();
            }
            else if (_token.Symbol == Symbols.SAbreParenteses)
            {
                _expressionAnalyzer.Add(_token);

                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }

                AnalyzeExpression();

                if (_token.Symbol != Symbols.SFechaParenteses)
                {
                    RaiseUnexpectedTokenError("\")\"");
                }

                _expressionAnalyzer.Add(_token);

                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }
            }
            else if (_token.Symbol == Symbols.SVerdadeiro || _token.Symbol == Symbols.SFalso)
            {
                _expressionAnalyzer.Add(_token);

                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }
            }
        }

        private void AnalyzeAttribution()
        {
            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }
            
            var item = _symbolTable.Search(_analyzingLexeme);            
            var rightType = AnalyzeExpressionType();
            var leftType = ItemType.None;
            var identificatorItem = item as IdentificatorItem;
            var funcItem = item as FunctionItem;

            if (identificatorItem != null)
            {
                leftType = identificatorItem.Type;
            }

            if (funcItem != null)
            {
                leftType = funcItem.Type;
            }

            if (leftType != rightType)
            {
                ushort column = _lexical.Position.Column;
                ushort line = _lexical.Position.Line;

                throw new CompilationException(string.Format(IncompatibleAttributionErrorMessage, line, column, 
                    rightType.GetFriendlyName(), leftType.GetFriendlyName()));
            }
        }

        private void AnalyzeFuncCall()
        {
            _expressionAnalyzer.Add(_token);

            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }
        }

        private ItemType AnalyzeExpressionType()
        {
            _expressionAnalyzer = new ExpressionAnalyzer(_symbolTable);
            AnalyzeExpression();

            var type = ItemType.None;

            try
            {
                type = _expressionAnalyzer.Analyze();
            }
            catch (ExpressionException ex)
            {
                RaiseNotFoundIdentificatorError(ex.Message);
            }

            return type;
        }
    }
}
