using LPD.Compiler.Lexical;
using LPD.Compiler.Shared;

namespace LPD.Compiler.Syntactic
{
    public class SyntacticAnalyzer
    {
        private LexicalAnalizer _lexical;
        private Token _token;

        public SyntacticAnalyzer(LexicalAnalizer lexical)
        {
            _lexical = lexical;
        }

        public CompileError DoAnalysis()
        {
            try
            {

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

        private void AnalyzeBlock()
        {

        }

        private void AnalyzeVar()
        {

        }

        private void AnalyzeVars()
        {

        }

        private void AnalyzeType()
        {

        }

        private void AnalyzeCommands()
        {

        }

        private void AnalyzeSimpleCommand()
        {

        }

        private void AnalyzeAttrProc()
        {

        }

        private void AnalyzeProcCall()
        {

        }

        private void AnalyzeRead()
        {

        }

        private void AnalyzePrint()
        {

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
            if (!NextToken())
            {
                //Todo: exception
            }

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
        }
    }
}
