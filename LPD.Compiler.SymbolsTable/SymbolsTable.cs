
namespace LPD.Compiler.SymbolsTable
{
    public class SymbolsTable
    {
        private SymbolTableItemCollection itens;
        public SymbolsTable()
        {
            itens = new SymbolTableItemCollection();
        }

        public void InsertTable(SymbolTableItem item)
        {
            itens.Add(item);
        }

        public void RemoveTable(string Lexeme)
        {
            for (int i = itens.Count - 1; i >= 0; i--)
            {
                SymbolTableItem item = itens[i];
                if (item.Lexeme != Lexeme)
                {
                    itens.Remove(item);
                }
                else
                {
                    break;
                }

            }
        }

        public SymbolTableItem SearchTable(string Lexeme)
        {

        }
    }
}
