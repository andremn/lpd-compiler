using LPD.Compiler.CodeGeneration;
using LPD.Compiler.Lexical;
using LPD.Compiler.Semantic;
using LPD.Compiler.Shared;
using LPD.Compiler.SymbolsTable;
using System;
using System.Collections.Generic;
using static LPD.Compiler.CodeGeneration.Instructions;
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
        private CodeGenerator _codeGenerator;
        private uint _lastLabel = 0;
        private ushort _level = 0;
        private uint _memory = 0;
        private string _analyzingLexeme = null;
        private string _currentFunctionLexeme = null;
        private bool _foundFuntionReturn = false;
        private uint _allocBase = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntacticAnalyzer"/> class with the specified <see cref="LexicalAnalyzer"/>.
        /// </summary>
        /// <param name="lexical">The lexical analyzer.</param>
        public SyntacticAnalyzer(LexicalAnalyzer lexical)
        {
            //Todo: FIX FUNCTION RETURN AND VARIABLES DECLARATIONS NOT FOUND!

            if (lexical == null)
            {
                throw new ArgumentNullException(nameof(lexical));
            }

            _lexical = lexical;
            _symbolTable = new VectorSymbolTable();
            _codeGenerator = new CodeGenerator();
        }

        /// <summary>
        /// Performs the code parsing.
        /// </summary>
        /// <returns></returns>
        public CompileError DoAnalysis()
        {
            NextToken();

            try
            {
                if (_token.Symbol == Symbols.SPrograma)
                {
                    NextToken();

                    if (_token.Symbol == Symbols.SIdentificador)
                    {
                        _symbolTable.Insert(new ProgramNameItem() { Lexeme = _token.Lexeme });
                        _codeGenerator.GenerateInstruction(START);
                        NextToken();

                        if (_token.Symbol == Symbols.SPontoVirgula)
                        {
                            AnalyzeBlock();

                            if (_token.Symbol == Symbols.SPonto)
                            {
                                LexicalItem lexicalItem;

                                if (_lexical.GetToken(out lexicalItem))
                                {
                                    _token = lexicalItem.Token;
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
            finally
            {
                _expressionAnalyzer = null;
                _symbolTable.Clear();
                _currentFunctionLexeme = null;
                _level = 0;
            }

            _codeGenerator.GenerateInstruction(HLT);
            _codeGenerator.SaveToFileAsync(@"C:\Users\andre\Desktop\Facul\Compiladores\Gerado\generated.asmd");
            return null;
        }

        private void NextToken()
        {
            LexicalItem item = new LexicalItem();

            if (!_lexical.GetToken(out item))
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            if (item.Error != null)
            {
                RaiseLexicalErrorMessage(item.Error.Message);
            }

            _token = item.Token;
        }

        private void AnalyzeBlock()
        {
            uint totalVars;

            NextToken();
            totalVars = AnalyzeVarsDcl();

            if (totalVars > 0)
            {
                _codeGenerator.GenerateInstruction(ALLOC, _allocBase, totalVars);
                _allocBase += totalVars;
            }

            AnalyzeSubRoutines();
            AnalyzeCommands();

            if (totalVars > 0)
            {
                _allocBase -= totalVars;
                _codeGenerator.GenerateInstruction(DALLOC, _allocBase, totalVars);
                _memory -= totalVars;
            }

        }

        private uint AnalyzeVars()
        {
            uint count = 0;

            do
            {
                if (_token.Symbol == Symbols.SIdentificador)
                {
                    if (_symbolTable.SearchDouble(_token.Lexeme))
                    {
                        RaiseDoubleIdentificatorError();
                    }

                    _symbolTable.Insert(new IdentificatorItem()
                    {
                        Lexeme = _token.Lexeme,
                        Level = _level,
                        Memory = _memory
                    });

                    count++;
                    _memory++;
                    NextToken();

                    if (_token.Symbol == Symbols.SVirgula || _token.Symbol == Symbols.SDoisPontos)
                    {
                        if (_token.Symbol == Symbols.SVirgula)
                        {
                            NextToken();

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

            NextToken();
            AnalyzeType();

            return count;
        }

        private uint AnalyzeVarsDcl()
        {
            uint count = 0;

            if (_token.Symbol == Symbols.SVar)
            {
                NextToken();

                if (_token.Symbol == Symbols.SIdentificador)
                {
                    while (_token.Symbol == Symbols.SIdentificador)
                    {
                        count += AnalyzeVars();

                        if (_token.Symbol == Symbols.SPontoVirgula)
                        {
                            NextToken();
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

            return count;
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
                NextToken();
            }
        }

        private void AnalyzeCommands()
        {
            if (_token.Symbol == Symbols.SInicio)
            {
                NextToken();
                AnalyzeSimpleCommand();

                while (_token.Symbol != Symbols.SFim)
                {
                    if (_token.Symbol == Symbols.SPontoVirgula)
                    {
                        NextToken();

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

                NextToken();
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
                    NextToken();

                    if (_token.Symbol == Symbols.SAtribuicao)
                    {
                        AnalyzeAttribution();
                    }

                    _foundFuntionReturn = true;
                }
                else
                {
                    NextToken();

                    if (_token.Symbol == Symbols.SAtribuicao)
                    {
                        AnalyzeAttribution();
                    }
                    else
                    {
                        AnalyzeProcCall();
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

        private void AnalyzeProcCall()
        {
            var procItem = _symbolTable.Search(_analyzingLexeme) as ProcItem;

            if (procItem == null)
            {
                RaiseMissingProcError();
            }

            if (_token.Symbol != Symbols.SPontoVirgula)
            {
                RaiseMissingSemicolonError();
            }

            _codeGenerator.GenerateInstruction(CALL, procItem.Label);
        }

        private void AnalyzeRead()
        {
            NextToken();

            if (_token.Symbol == Symbols.SAbreParenteses)
            {
                NextToken();

                if (_token.Symbol == Symbols.SIdentificador)
                {
                    var item = _symbolTable.Search(_token.Lexeme);

                    if (item == null)
                    {
                        RaiseNotFoundIdentificatorError(_token.Lexeme);
                    }

                    NextToken();

                    if (_token.Symbol == Symbols.SFechaParenteses)
                    {
                        NextToken();
                    }
                    else
                    {
                        RaiseUnexpectedTokenError("\")\"");
                    }

                    _codeGenerator.GenerateInstruction(RD);
                    _codeGenerator.GenerateInstruction(STR, (item as IdentificatorItem).Memory);
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
            NextToken();

            if (_token.Symbol == Symbols.SAbreParenteses)
            {
                NextToken();

                if (_token.Symbol == Symbols.SIdentificador)
                {
                    var item = _symbolTable.Search(_token.Lexeme);

                    if (item == null)
                    {
                        RaiseNotFoundIdentificatorError(_token.Lexeme);
                    }

                    if (item != null)
                    {
                        if (!(item is FunctionItem) && !(item is IdentificatorItem))
                        {
                            throw new CompilationException(string.Format(NotAFuncVarErrorMessage, _lexical.Position.Line, _lexical.Position.Column, _token.Lexeme));
                        }
                    }

                    NextToken();

                    if (_token.Symbol == Symbols.SFechaParenteses)
                    {
                        NextToken();
                    }
                    else
                    {
                        RaiseUnexpectedTokenError("\")\"");
                    }

                    _codeGenerator.GenerateInstruction(LDV, (item as IdentificatorItem).Memory);
                    _codeGenerator.GenerateInstruction(PRN);
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
            uint label1 = _lastLabel;
            uint label2;

            _codeGenerator.GenerateLabel(_lastLabel);
            _lastLabel++;
            NextToken();

            var type = AnalyzeExpressionType();

            if (type != ItemType.Boolean)
            {
                RaiseIncompatibleTypeError();
            }

            if (_token.Symbol != Symbols.SFaca)
            {
                RaiseUnexpectedTokenError("\"faca\"");
            }

            label2 = _lastLabel;
            _codeGenerator.GenerateInstruction(JMPF, _codeGenerator.GetStringLabelFor(_lastLabel));
            _lastLabel++;

            NextToken();
            AnalyzeSimpleCommand();

            _codeGenerator.GenerateInstruction(JMP, _codeGenerator.GetStringLabelFor(label1));
            _codeGenerator.GenerateLabel(label2);
        }

        private void AnalyzeIf()
        {
            uint firstLabel;
            uint secondLabel;

            _lastLabel++;
            firstLabel = secondLabel = _lastLabel;
            NextToken();

            var expressionType = AnalyzeExpressionType();

            if (expressionType != ItemType.Boolean)
            {
                RaiseIncompatibleTypeError();
            }

            _codeGenerator.GenerateInstruction(JMPF, _codeGenerator.GetStringLabelFor(firstLabel));

            if (_token.Symbol != Symbols.SEntao)
            {
                RaiseUnexpectedTokenError("\"entao\"");
            }

            NextToken();
            AnalyzeSimpleCommand();

            if (_token.Symbol == Symbols.SSenao)
            {
                _lastLabel++;
                secondLabel = _lastLabel;
                _codeGenerator.GenerateInstruction(JMP, _codeGenerator.GetStringLabelFor(secondLabel));
                _codeGenerator.GenerateLabel(firstLabel);
                NextToken();
                AnalyzeSimpleCommand();
            }

            _codeGenerator.GenerateLabel(secondLabel);
        }

        private void AnalyzeSubRoutines()
        {
            uint label = _lastLabel;
            var hasGenerated = false;

            if (_token.Symbol == Symbols.SProcedimento || _token.Symbol == Symbols.SFuncao)
            {
                _codeGenerator.GenerateInstruction(JMP, _codeGenerator.GetStringLabelFor(_lastLabel));
                _lastLabel++;
                hasGenerated = true;
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
                    NextToken();
                }
                else
                {
                    RaiseMissingSemicolonError();
                }
            }

            if (hasGenerated)
            {
                _codeGenerator.GenerateLabel(label);
            }
        }

        private void AnalyzeProcDcl()
        {
            NextToken();

            if (_token.Symbol != Symbols.SIdentificador)
            {
                RaiseUnexpectedTokenError("identificador");
            }

            var item = _symbolTable.SearchByLevel(_token.Lexeme, _level);

            if (item != null)
            {
                RaiseDoubleIdentificatorError();
            }

            item = new ProcItem
            {
                Lexeme = _token.Lexeme,
                Level = _level
            };

            _symbolTable.Insert(item);
            _level++;
            NextToken();

            if (_token.Symbol != Symbols.SPontoVirgula)
            {
                RaiseMissingSemicolonError();
            }

            AnalyzeBlock();
            _symbolTable.CleanUpToLevel(_level);
            _level--;
        }

        private void AnalyzeFuncDcl()
        {
            NextToken();

            if (_token.Symbol != Symbols.SIdentificador)
            {
                RaiseUnexpectedTokenError("identificador");
            }

            var item = _symbolTable.Search(_token.Lexeme);

            if (item != null)
            {
                RaiseDoubleIdentificatorError();
            }

            var funcItem = new FunctionItem
            {
                Lexeme = _token.Lexeme,
                Level = _level
            };

            _currentFunctionLexeme = _token.Lexeme;
            _level++;
            NextToken();

            if (_token.Symbol != Symbols.SDoisPontos)
            {
                RaiseUnexpectedTokenError("\":\"");
            }

            NextToken();

            if (_token.Symbol != Symbols.SInteiro && _token.Symbol != Symbols.SBooleano)
            {
                RaiseUnexpectedTokenError("\"inteiro\" ou \"booleano\"");
            }

            var type = _token.Symbol == Symbols.SInteiro ? ItemType.Integer : ItemType.Boolean;

            funcItem.Type = type;
            _symbolTable.Insert(funcItem);
            NextToken();

            if (_token.Symbol != Symbols.SPontoVirgula)
            {
                RaiseMissingSemicolonError();
            }

            AnalyzeBlock();

            if (!_foundFuntionReturn)
            {
                RaiseMissingFunctionReturn();
            }

            _symbolTable.CleanUpToLevel(_level);
            _foundFuntionReturn = false;
            _currentFunctionLexeme = null;
            _level--;
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
                NextToken();
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
                NextToken();
            }

            AnalyzeTerm();

            while (_token.Symbol == Symbols.SMais || _token.Symbol == Symbols.SMenos || _token.Symbol == Symbols.SOu)
            {
                _expressionAnalyzer.Add(_token);
                NextToken();
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
                NextToken();
                AnalyzeFactor();
            }
        }

        private void AnalyzeFactor()
        {
            if (_token.Symbol == Symbols.SIdentificador)
            {
                var item = _symbolTable.SearchByLevel(_token.Lexeme, _level);

                if (item == null)
                {
                    RaiseNotFoundIdentificatorError(_token.Lexeme);
                }

                var funcItem = item as FunctionItem;

                if (funcItem != null)
                {
                    AnalyzeFuncCall(funcItem);
                }
                else
                {
                    _expressionAnalyzer.Add(_token);
                    NextToken();
                }
            }
            else if (_token.Symbol == Symbols.SNumero)
            {
                _expressionAnalyzer.Add(_token);
                NextToken();
            }
            else if (_token.Symbol == Symbols.SNao)
            {
                _expressionAnalyzer.Add(_token);
                NextToken();
                AnalyzeFactor();
            }
            else if (_token.Symbol == Symbols.SAbreParenteses)
            {
                _expressionAnalyzer.Add(_token);
                NextToken();
                AnalyzeExpression();

                if (_token.Symbol != Symbols.SFechaParenteses)
                {
                    RaiseUnexpectedTokenError("\")\"");
                }

                _expressionAnalyzer.Add(_token);
                NextToken();
            }
            else if (_token.Symbol == Symbols.SVerdadeiro || _token.Symbol == Symbols.SFalso)
            {
                _expressionAnalyzer.Add(_token);
                NextToken();
            }
        }

        private void AnalyzeAttribution()
        {
            NextToken();

            var item = _symbolTable.Search(_analyzingLexeme);
            var rightType = AnalyzeExpressionType();
            var leftType = ItemType.None;
            var identificatorItem = item as IdentificatorItem;

            if (identificatorItem != null)
            {
                leftType = identificatorItem.Type;
            }

            if (leftType != rightType)
            {
                ushort column = _lexical.Position.Column;
                ushort line = _lexical.Position.Line;

                throw new CompilationException(string.Format(IncompatibleAttributionErrorMessage, line, column,
                    rightType.GetFriendlyName(), leftType.GetFriendlyName()));
            }

            _codeGenerator.GenerateInstruction(STR, identificatorItem.Memory);
        }

        private void AnalyzeFuncCall(FunctionItem currentFunction)
        {
            _codeGenerator.GenerateInstruction(CALL, currentFunction.Label);
            _expressionAnalyzer.Add(_token);
            NextToken();
        }

        private ItemType AnalyzeExpressionType()
        {
            _expressionAnalyzer = new ExpressionAnalyzer(_symbolTable);
            AnalyzeExpression();

            var type = ItemType.None;

            try
            {
                var tokens = _expressionAnalyzer.Analyze(out type);

                GenerateCodeForExpression(tokens);
            }
            catch (ExpressionException ex)
            {
                RaiseNotFoundIdentificatorError(ex.Message);
            }

            return type;
        }

        private void GenerateCodeForExpression(IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                switch (token.Symbol)
                {
                    case Symbols.SNumero:
                        _codeGenerator.GenerateInstruction(LDC, token.Lexeme);
                        break;
                    case Symbols.SVerdadeiro:
                        _codeGenerator.GenerateInstruction(LDC, 1);
                        break;
                    case Symbols.SFalso:
                        _codeGenerator.GenerateInstruction(LDC, 0);
                        break;
                    case Symbols.SNao:
                        _codeGenerator.GenerateInstruction(NEG);
                        break;
                    case Symbols.SMenosUnario:
                        _codeGenerator.GenerateInstruction(INV);
                        break;
                    case Symbols.SMais:
                        _codeGenerator.GenerateInstruction(ADD);
                        break;
                    case Symbols.SMenos:
                        _codeGenerator.GenerateInstruction(SUB);
                        break;
                    case Symbols.SMult:
                        _codeGenerator.GenerateInstruction(MULT);
                        break;
                    case Symbols.SDiv:
                        _codeGenerator.GenerateInstruction(DIVI);
                        break;
                    case Symbols.SE:
                        _codeGenerator.GenerateInstruction(AND);
                        break;
                    case Symbols.SOu:
                        _codeGenerator.GenerateInstruction(OR);
                        break;
                    case Symbols.SIg:
                        _codeGenerator.GenerateInstruction(CEQ);
                        break;
                    case Symbols.SDif:
                        _codeGenerator.GenerateInstruction(CDIF);
                        break;
                    case Symbols.SMaior:
                        _codeGenerator.GenerateInstruction(CMA);
                        break;
                    case Symbols.SMaiorIg:
                        _codeGenerator.GenerateInstruction(CMAQ);
                        break;
                    case Symbols.SMenor:
                        _codeGenerator.GenerateInstruction(CME);
                        break;
                    case Symbols.SMenorIg:
                        _codeGenerator.GenerateInstruction(CMEQ);
                        break;
                    case Symbols.SIdentificador:
                        var item = _symbolTable.Search(token.Lexeme) as IdentificatorItem;

                        _codeGenerator.GenerateInstruction(LDV, item.Memory);
                        break;
                }
            }
        }

        #region Errors

        private void RaiseNotFoundIdentificatorError(string lexeme)
        {
            ushort column = _lexical.Position.Column;
            ushort line = _lexical.Position.Line;

            throw new CompilationException(string.Format(NotFoundIdentifierErrorMessage, line, column, lexeme));
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
            throw new CompilationException(string.Format(InvalidTokenErrorMessage, _lexical.Position.Line, _lexical.Position.Column, _token.Lexeme));
        }

        private void RaiseMissingFunctionReturn()
        {
            var line = _lexical.Position.Line - 1;

            throw new CompilationException(string.Format(MissingFunctionReturnMessage, line, 0, _currentFunctionLexeme));
        }

        private void RaiseMissingProcError()
        {
            _position = null;
            throw new CompilationException(string.Format(NotFoundProcErrorMessage, _lexical.Position.Line, _lexical.Position.Column, _token.Lexeme));
        }

        private void RaiseMissingFuncError()
        {
            _position = null;
            throw new CompilationException(string.Format(NotFoundFuncErrorMessage, _lexical.Position.Line, _lexical.Position.Column, _token.Lexeme));
        }

        private void RaiseDoubleIdentificatorError()
        {
            _position = null;
            throw new CompilationException(string.Format(DoubleIdentificatorErrorMessage, _lexical.Position.Line, _lexical.Position.Column, _token.Lexeme));
        }

        private void RaiseIncompatibleTypeError()
        {
            _position = null;
            throw new CompilationException(string.Format(IncompatibleIfExpressionErrorMessage, _lexical.Position.Line, _lexical.Position.Column));
        }

        #endregion
    }
}
