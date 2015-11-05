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

        public void RemoveUntil(string level)
        {
            for (int i = _itemsCollection.Count - 1; i >= 0; i--)
            {
                FunctionItem functionItem = _itemsCollection[i] as FunctionItem;
                
                if (functionItem != null && functionItem.Level == level)
                {
                    break;
                }

                _itemsCollection.RemoveAt(i);
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

        public SymbolTableItem SearchByLevel(string lexeme, string level)
        {
            var ids = GetIdentificatorsByLevel(level);

            return ids.SingleOrDefault(item => item.Lexeme == lexeme);
        }

        public void CleanUpToLevel(string level)
        {
            var identificators = GetIdentificatorsByLevel(level).ToList();

            for (int i = 0; i < identificators.Count; i++)
            {
                _itemsCollection.Remove(identificators[i]);
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

        private IEnumerable<SymbolTableItem> GetIdentificatorsByLevel(string level)
        {
            bool found = false;

            for (int i = 0; i < _itemsCollection.Count; i++)
            {
                var item = _itemsCollection[i];
                var func = item as FunctionItem;
                var proc = item as ProcItem;

                if (func != null)
                {
                    if (found)
                    {
                        break;
                    }

                    if (func.Level == level)
                    {
                        found = true;
                        continue;
                    }
                }

                if (proc != null)
                {
                    if (found)
                    {
                        break;
                    }

                    if (proc.Level == level)
                    {
                        found = true;
                        continue;
                    }
                }

                if (found)
                {
                    yield return item;
                }
            }
        }
    }
}
