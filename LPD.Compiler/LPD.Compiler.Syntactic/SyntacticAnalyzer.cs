using LPD.Compiler.Lexical;
using LPD.Compiler.Shared;
using System;

namespace LPD.Compiler.Syntactic
{
    public class SyntacticAnalyzer
    {
        private LexicalAnalyzer _lexical;
        private Token _token;

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
                //TODO - Criar excessao
            }

            try
            {
                if (_token.Symbol == Symbols.SPrograma)
                {
                    if (!NextToken())
                    {
                        //TODO - Criar excessao
                    }
                    if (_token.Symbol == Symbols.SIdentificador)
                    {
                        if (!NextToken())
                        {
                            //TODO - Criar excessao
                        }
                        if (_token.Symbol == Symbols.SPontoVirgula)
                        {
                            AnalyzeBlock();
                            if (_token.Symbol == Symbols.SPonto && !NextToken())
                            {
                                //TODO - SUCESSO
                            }
                            else
                            {
                                throw new SyntacticException();
                            }
                        }
                        else
                        {
                            throw new SyntacticException();
                        }
                    }
                    else
                    {
                        throw new SyntacticException();
                    }
                }
                else
                {
                    throw new SyntacticException();
                }


            }
            catch (SyntacticException)
            {
                
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
                throw new SyntacticException(item.Error.Message);
            }

            _token = item.Token;
            return true;
        }

        private void AnalyzeBlock()
        {
            if (!NextToken())
            {
                //TODO - Criar excessao
            }
            //Repensar
            AnalyzeVarsDcl();
            AnalyzeSubRoutine();
            AnalyzeCommands();
        }

        private void AnalyzeVar()
        {
            do
            {
                if (_token.Symbol == Symbols.SIdentificador)
                {
                    if (!NextToken())
                    {
                        //TODO - Criar excessao
                    }
                    if (_token.Symbol == Symbols.SVirgula || _token.Symbol == Symbols.SDoisPontos)
                    {
                        if (_token.Symbol == Symbols.SVirgula)
                        {
                            if (!NextToken())
                            {
                                //TODO - Criar excessao
                            }
                            if (_token.Symbol == Symbols.SDoisPontos)
                            {
                                //TODO - ERRO
                                throw new SyntacticException();
                            }
                        }
                        else
                        {
                            throw new SyntacticException();
                        }
                    }
                }
            } while (_token.Symbol == Symbols.SDoisPontos);

            if (!NextToken())
            {
                //TODO - Criar excessao
            }
            AnalyzeType();
        }

        private void AnalyzeVarsDcl()
        {
            if (_token.Symbol == Symbols.SVar)
            {
                if (!NextToken())
                {
                    //TODO - Criar excessao
                }
                if (_token.Symbol == Symbols.SIdentificador)
                {
                    while(_token.Symbol == Symbols.SIdentificador)
                    {
                        AnalyzeVar();
                        if (_token.Symbol == Symbols.SPontoVirgula)
                        {
                            if (!NextToken())
                            {
                                //TODO - Criar excessao
                            }
                        }
                        else
                        {
                            throw new SyntacticException();
                        }
                    }
                }
                else
                {
                    throw new SyntacticException();
                }
            }            
        }

        private void AnalyzeType()
        {
            if (_token.Symbol != Symbols.SInteiro && _token.Symbol != Symbols.SBooleano)
            {
                throw new SyntacticException();
            }
            else
            {
                if (!NextToken())
                {
                    //TODO - Criar excessao
                }
            }
        }

        private void AnalyzeCommands()
        {
            if (_token.Symbol == Symbols.SInicio)
            {
                if (!NextToken())
                {
                    //TODO - Criar excessao
                }
                AnalyzeSimpleCommand();
                while (_token.Symbol != Symbols.SFim)
                {
                    if (_token.Symbol == Symbols.SPontoVirgula)
                    {
                        if (!NextToken())
                        {
                            //TODO - Criar excessao
                        }
                        if (_token.Symbol != Symbols.SFim)
                        {
                            AnalyzeSimpleCommand();
                        }
                        else
                        {
                            throw new SyntacticException();
                        }
                    }
                    else
                    {
                        throw new SyntacticException();
                    }
                }
            }
            else
            {
                throw new SyntacticException();
            }
        }

        private void AnalyzeSimpleCommand()
        {
            if (_token.Symbol == Symbols.SIdentificador)
            {
                if (!NextToken())
                {
                    //TODO - Criar excessao
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
            if (!NextToken())
            {
                //TODO - Criar excessao
            }
        }

        private void AnalyzeRead()
        {
            if (!NextToken())
            {
                //TODO - Criar excessao
            }
            if (_token.Symbol == Symbols.SAbreParenteses)
            {
                if (!NextToken())
                {
                    //TODO - Criar excessao
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
                        //TODO - Criar excessao
                    }
                    if (_token.Symbol == Symbols.SFechaParenteses)
                    {
                        if (!NextToken())
                        {
                            //TODO - Criar excessao
                        }
                    }
                    else
                    {
                        throw new SyntacticException();
                    }
                }
                else
                {
                    throw new SyntacticException();
                }
            }
            else
            {
                throw new SyntacticException();
            }

        }

        private void AnalyzeWrite()
        {
            if (!NextToken())
            {
                //TODO - Criar excessao
            }
            if (_token.Symbol == Symbols.SAbreParenteses)
            {
                if (!NextToken())
                {
                    //TODO - Criar excessao
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
                        //TODO - Criar excessao
                    }
                    if (_token.Symbol == Symbols.SAbreParenteses)
                    {
                        if (!NextToken())
                        {
                            //TODO - Criar excessao
                        }
                    }
                    else
                    {
                        throw new SyntacticException();
                    }
                }
                else
                {
                    throw new SyntacticException();
                }
            }
            else
            {
                throw new SyntacticException();
            }

        }

        private void AnalyzeWhile()
        {
            if (!NextToken())
            {
                //Todo: exception
            }

            AnalyzeExpression();

            if (_token.Symbol != Symbols.SFaca)
            {
                throw new SyntacticException();
            }

            if (!NextToken())
            {
                //Todo: exception
            }

            AnalyzeSimpleCommand();
        }

        private void AnalyzeIf()
        {
            if (!NextToken())
            {
                //Todo: exception
            }

            AnalyzeExpression();

            if (_token.Symbol != Symbols.SEntao)
            {
                throw new SyntacticException();
            }

            if (!NextToken())
            {
                //Todo: exception
            }

            AnalyzeSimpleCommand();

            if (_token.Symbol == Symbols.SSenao)
            {
                if (!NextToken())
                {
                    //Todo: exception
                }

                AnalyzeSimpleCommand();
            }
        }

        private void AnalyzeSubRoutine()
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
                        //Todo: exception
                    }
                }
                else
                {
                    throw new SyntacticException();
                }
            }
        }

        private void AnalyzeProcDcl()
        {
            if (!NextToken())
            {
                //Todo: exception
            }

            if (_token.Symbol != Symbols.SIdentificador)
            {
                throw new SyntacticException();
            }

            if (!NextToken())
            {
                //Todo: exception
            }

            if (_token.Symbol != Symbols.SPontoVirgula)
            {
                throw new SyntacticException();
            }

            AnalyzeBlock();
        }

        private void AnalyzeFuncDcl()
        {
            if (!NextToken())
            {
                //Todo: exception
            }

            if (_token.Symbol != Symbols.SIdentificador)
            {
                throw new SyntacticException();
            }

            if (!NextToken())
            {
                //Todo: exception
            }

            if (_token.Symbol != Symbols.SDoisPontos)
            {
                throw new SyntacticException();
            }

            if (_token.Symbol != Symbols.SInteiro && _token.Symbol != Symbols.SBooleano)
            {
                throw new SyntacticException();
            }

            if (!NextToken())
            {
                //Todo: exception
            }

            if (_token.Symbol != Symbols.SPontoVirgula)
            {
                throw new SyntacticException();
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
                    //Todo: exception
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
                    //Todo: exception
                }
            }

            AnalyzeTerm();

            while (_token.Symbol == Symbols.SMais || _token.Symbol == Symbols.SMenos || _token.Symbol == Symbols.SOu)
            {
                if (!NextToken())
                {
                    //Todo: exception
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
                    //Todo: exception
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
            else
            {
                if (_token.Symbol == Symbols.SNumero)
                {
                    if (!NextToken())
                    {
                        //Todo: exception
                    }
                }
                else if (_token.Symbol == Symbols.SNao)
                {
                    if (!NextToken())
                    {
                        //Todo: exception
                    }

                    AnalyzeFactor();
                }
                else if (_token.Symbol == Symbols.SAbreParenteses)
                {
                    if (!NextToken())
                    {
                        //Todo: exception
                    }

                    AnalyzeExpression();

                    if (_token.Symbol != Symbols.SFechaParenteses)
                    {
                        throw new SyntacticException();
                    }

                    if (_token.Symbol != Symbols.SVerdadeiro && _token.Symbol != Symbols.SFalso)
                    {
                        if (!NextToken())
                        {
                            //Todo: exception
                        }
                    }
                }
            }
        }

        private void AnalyzeAttribution()
        {
            if (!NextToken())
            {
                //Todo: exception
            }

            AnalyzeExpression();
        }

        private void AnalyzeFuncCall()
        {
            //Todo: semântico deve verificar se o tipo do identificador é o mesmo da função

            if (!NextToken())
            {
                //Todo: exception
            }
        }
    }
}
