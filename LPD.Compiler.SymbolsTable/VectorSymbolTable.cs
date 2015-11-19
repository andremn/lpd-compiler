using System;
using System.Collections.Generic;
using System.Linq;

namespace LPD.Compiler.SymbolsTable
{
    public class VectorSymbolTable
    {
        private SymbolTableItemCollection _itemsCollection;
        
        public IReadOnlyCollection<SymbolTableItem> Items
        {
            get { return _itemsCollection.ToList().AsReadOnly(); }
        }

        public VectorSymbolTable()
        {
            _itemsCollection = new SymbolTableItemCollection();
        }

        public void Insert(SymbolTableItem item)
        {
            _itemsCollection.Add(item);
        }

        public void RemoveUntil(uint level)
        {
            var items = _itemsCollection.Select(item => item as LeveledItem)
                                        .Where(item => item != null && item.Level == level)
                                        .ToList();

            foreach (var item in items)
            {
                _itemsCollection.Remove(item);
            }
        }

        public SymbolTableItem Search(string lexeme)
        {
            return _itemsCollection.Search(lexeme).LastOrDefault();
        }

        public bool SearchDouble(string lexeme)
        {
            return _itemsCollection.Search(lexeme).Count > 1;
        }

        public SymbolTableItem SearchByLevel(string lexeme, uint level)
        {
            var ids = GetIdentificatorsByLevel(level);

            return ids.SingleOrDefault(item => item.Lexeme == lexeme);
        }

        public void CleanUpToLevel(uint level)
        {
            var identificators = GetIdentificatorsByLevel(level).ToList();

            for (int i = 0; i < identificators.Count; i++)
            {
                if (identificators[i] is IdentificatorItem)
                {
                    _itemsCollection.Remove(identificators[i]);
                }
            }
        }

        public void CleanUp()
        {
            for (int i = _itemsCollection.Count; i >= 0; i--)
            {
                var item = _itemsCollection[i];

                if (item is FunctionItem || item is ProcItem)
                {
                    break;
                }

                _itemsCollection.RemoveAt(i);
            }
        }

        public void SetFunctionType(string lexeme, ItemType type)
        {
            var func = (from item in _itemsCollection
                        select item as FunctionItem)
                        .Where(item => item != null)
                        .SingleOrDefault(item => item.Lexeme == lexeme);

            if (func == null)
            {
                throw new ArgumentException($"Function with lexeme {lexeme} not found.");
            }

            func.Type = type;
        }

        public void SetTypeLastestVars(ItemType type)
        {
            var identificators = (from item in _itemsCollection
                                  select item as IdentificatorItem)
                                  .Where(item => item != null && item.Type == ItemType.None);

            foreach (var identificator in identificators)
            {
                identificator.Type = type;
            }
        }

        public void Clear()
        {
            _itemsCollection.Clear();
        }

        private IEnumerable<SymbolTableItem> GetIdentificatorsByLevel(uint level)
        {
            return _itemsCollection.Select(item => item as LeveledItem).Where(item => item != null && item.Level == level);
        }
    }
}
