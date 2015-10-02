using LPD.Compiler.Lexical;
using LPD.Compiler.Shared;

namespace LPD.Compiler.Syntactic
{
    public class SyntacticAnaliser
    {
        private LexicalAnalizer _lexical;
        private Token _token;

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
                            BlockAnalyze();
                            if (!NextToken())
                            {
                                //TODO - Criar excessao
                            }
                        }
                    }
                }

            }
            catch (SyntacticException ex)
            {
                
            }

            return null;
        }

        private bool NextToken()
        {
            LexicalItem item = new LexicalItem();

            if (!_lexical.Next(out item))
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

        private void BlockAnalyze()
        {
            if (!NextToken())
            {
                //TODO - Criar excessao
            }
            VarsDclAnalyze();
            SubRoutineAnalise();
            CommandsAnalise();
        }

        private void VarAnalyze()
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
            TypeAnalyze();
        }

        private void VarsDclAnalyze()
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
                        VarAnalyze();
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

        private void TypeAnalyze()
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

        private void CommandsAnalyze()
        {
            if (_token.Symbol == Symbols.SInicio)
            {
                if (!NextToken())
                {
                    //TODO - Criar excessao
                }
                SimpleCommandAnalyze();
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
                            SimpleCommandAnalyze();
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

        private void SimpleCommandAnalyze()
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
                IfAnalyze();
            }
            else if (_token.Symbol == Symbols.SEnquanto)
            {
                WhileAnalyze();
            }
            else if (_token.Symbol == Symbols.SLeia)
            {
                ReadAnalyze();
            }
            else if (_token.Symbol == Symbols.SEscreva)
            {
                WriteAnalyze();
            }
            else 
            {
                CommandsAnalyze();
            }
        }

        private void AttrAnalyze()
        {
            
        }

        private void ProcCallAnalyze()
        {

        }

        private void ReadAnalyze()
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

        private void WriteAnalyze()
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
        //Ate aqui
        private void WhileAnalyze()
        {

        }

        private void IfAnalyze()
        {

        }

        private void SubRoutineAnalise()
        {

        }

        private void ProcDclAnalise()
        {

        }

        private void FuncDclAnalise()
        {

        }

        private void ExpressionAnalise()
        {

        }

        private void SimpleExpressionAnalise()
        {

        }

        private void TermAnalise()
        {

        }

        private void FactorAnalise()
        {

        }

        private void FuncCallAnalise()
        {

        }
    }
}
