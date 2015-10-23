using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPD.Compiler.SymbolsTable
{
    public class SymbolTableItemCollection : IEnumerable<SymbolTableItem>
    {
        private IList<SymbolTableItem> _items;

        public SymbolTableItem this[int index]
        {
            get { return _items[index]; }
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public SymbolTableItemCollection()
        {
            _items = new List<SymbolTableItem>();
        }

        public void Add(SymbolTableItem item)
        {
            _items.Add(item);
        }

        public void Remove(SymbolTableItem item)
        {
            _items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public IList<SymbolTableItem> Search(string lexeme)
        {
            return _items.Where(item => item.Lexeme == lexeme).ToList();
        }

        public IEnumerator<SymbolTableItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
