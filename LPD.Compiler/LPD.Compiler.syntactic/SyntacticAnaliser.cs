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

        private void BlockAnalise()
        {

        }

        private void VarAnalise()
        {

        }

        private void VarsAnalise()
        {

        }

        private void TypeAnalise()
        {

        }

        private void CommandsAnalise()
        {

        }

        private void SimpleCommandAnalise()
        {

        }

        private void AttrProcAnalise()
        {

        }

        private void ProcCallAnalise()
        {

        }

        private void ReadAnalise()
        {

        }

        private void WriteAnalise()
        {

        }

        private void WhileAnalise()
        {

        }

        private void IfAnalise()
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
