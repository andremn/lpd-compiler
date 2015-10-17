
using System.Collections.Generic;
using System.Linq;

namespace LPD.Compiler.SymbolsTable
{
    public class SymbolsTable
    {
        private SymbolTableItemCollection _itemsCollection;

        public SymbolsTable()
        {
            _itemsCollection = new SymbolTableItemCollection();
        }

        public void InsertTable(SymbolTableItem item)
        {
            _itemsCollection.Add(item);
        }

        public void RemoveTable(string lexeme)
        {
            for (int i = _itemsCollection.Count - 1; i >= 0; i--)
            {
                SymbolTableItem item = _itemsCollection[i];

                if (item.Lexeme == lexeme)
                {
                    break;
                }

                _itemsCollection.Remove(item);
            }
        }

        public IList<SymbolTableItem> SearchTable(string lexeme)
        {
            return _itemsCollection.Search(lexeme);
        }

        public SymbolTableItem SearchTableByLevel(string lexeme, string level)
        {
            var items = _itemsCollection.Search(lexeme);
            var funcs = items.Select(item => item as FunctionItem);

            return funcs.SingleOrDefault(function => function.Level == level);
        }
    }
}
