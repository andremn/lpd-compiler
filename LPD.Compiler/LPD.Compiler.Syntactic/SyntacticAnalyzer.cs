using LPD.Compiler.Lexical;
using LPD.Compiler.Shared;
using System;
using static LPD.Compiler.Syntactic.Properties.Resources;

namespace LPD.Compiler.Syntactic
{
    public class SyntacticAnalyzer
    {
        private LexicalAnalyzer _lexical;
        private Token _token;
        private CodePosition? _position = null;

        public SyntacticAnalyzer(LexicalAnalyzer lexical)
        {
            if (lexical == null)
            {
                throw new ArgumentNullException(nameof(lexical));
            }

            _lexical = lexical;
        }

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
            catch (SyntacticException ex)
            {
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

        private void RaiseMissingInicioError()
        {
            int column = _lexical.Position.Column - _token.Lexeme.Length;

            _position = null;
            throw new SyntacticException(string.Format(MissingInicioErrorMessage, _lexical.Position.Line, column));
        }

        private void RaiseMissingSemicolonError()
        {
            ushort column = (ushort)(_lexical.ReadTokens[_lexical.ReadTokens.Count - 1].Lexeme.Length);
            ushort line = (ushort)(_lexical.Position.Line - 1);

            _position = new CodePosition() { Line = line, Column = column };
            throw new SyntacticException(string.Format(UnexpectedTokenErrorMessage, line, column, "\";\""));
        }

        private void RaiseUnexpectedTokenError(string message)
        {
            int column = _lexical.Position.Column - _token.Lexeme.Length;

            _position = null;
            throw new SyntacticException(string.Format(UnexpectedTokenErrorMessage, _lexical.Position.Line, column, message));
        }

        private void RaiseLexicalErrorMessage(string message)
        {
            int column = _lexical.Position.Column - _token.Lexeme.Length;

            _position = null;
            throw new SyntacticException(string.Format(LexicalErrorMessage, _lexical.Position.Line, column, message));
        }

        private void RaiseUnexpectedEndOfFileMessage()
        {
            _position = null;
            throw new SyntacticException(string.Format(UnexpectedEndOfFileErrorMessage, _lexical.Position.Line, _lexical.Position.Column));
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
                                RaiseUnexpectedTokenError("\":\"");
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
                RaiseUnexpectedTokenError("\"inicio\"");
            }
        }

        private void AnalyzeSimpleCommand()
        {
            if (_token.Symbol == Symbols.SIdentificador)
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

        private void AnalyzeWhile()
        {
            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }

            AnalyzeExpression();

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

            AnalyzeExpression();

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
                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }
            }

            AnalyzeTerm();

            while (_token.Symbol == Symbols.SMais || _token.Symbol == Symbols.SMenos || _token.Symbol == Symbols.SOu)
            {
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

            while (_token.Symbol == Symbols.SMult || _token.Symbol == Symbols.SDiv || _token.Symbol == Symbols.SE)
            {
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
                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }
            }
            else if (_token.Symbol == Symbols.SNao)
            {
                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }

                AnalyzeFactor();
            }
            else if (_token.Symbol == Symbols.SAbreParenteses)
            {
                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }

                AnalyzeExpression();

                if (_token.Symbol != Symbols.SFechaParenteses)
                {
                    RaiseUnexpectedTokenError("\")\"");
                }

                if (!NextToken())
                {
                    RaiseUnexpectedEndOfFileMessage();
                }
            }
            else if (_token.Symbol == Symbols.SVerdadeiro || _token.Symbol == Symbols.SFalso)
            {
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

            AnalyzeExpression();
        }

        private void AnalyzeFuncCall()
        {
            //Todo: semântico deve verificar se o tipo do identificador é o mesmo da função

            if (!NextToken())
            {
                RaiseUnexpectedEndOfFileMessage();
            }
        }
    }
}
