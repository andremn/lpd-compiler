using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPD.Compiler.SymbolsTable
{
    public class SymbolTableItemCollection
    {
        private List<SymbolTableItem> itens;

        public SymbolTableItem this[int index]
        {
            get
            {
                return itens[index];
            }
        }

        public int Count { get { return itens.Count; } }

        public SymbolTableItemCollection()
        {
            itens = new List<SymbolTableItem>();
        }

        public void Add(SymbolTableItem item)
        {
            itens.Add(item);
        }

        public void Remove(SymbolTableItem item)
        {
            itens.Remove(item);
        }
        public SymbolTableItem Search(string Lexeme)
        {
            return itens.SingleOrDefault(item => item.Lexeme == Lexeme);
        }

    }
}
