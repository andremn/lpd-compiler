using LPD.Compiler.SymbolsTable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace LPD.Compiler.Test.SymbolsTableTests
{
    [TestClass]
    public class SymbolsTableItemCollectionTests
    {
        [TestMethod]
        public void ADD_NEW_ITEM_TEST()
        {
            var collection = new SymbolTableItemCollection();
            var internalCollection = GetInternalCollection(collection);
            var item = new IdentificatorItem
            {
                Lexeme = "x",
            };

            collection.Add(item);
            Assert.IsTrue(internalCollection.Count == 1);
            Assert.IsTrue(collection.Count == 1);
            Assert.AreEqual(internalCollection[0], item);
        }

        [TestMethod]
        public void ADD_NEW_ITEMS_TEST()
        {
            var collection = new SymbolTableItemCollection();
            var internalCollection = GetInternalCollection(collection);
            var item1 = new FunctionItem
            {
                Lexeme = "func"
            };
            var item2 = new ProcItem
            {
                Lexeme = "proc"
            };

            collection.Add(item1);
            collection.Add(item2);
            Assert.IsTrue(internalCollection.Count == 2);
            Assert.IsTrue(collection.Count == 2);
            Assert.AreEqual(internalCollection[0], item1);
            Assert.AreEqual(internalCollection[1], item2);
        }

        [TestMethod]
        public void REMOVE_NEW_ITEM_TEST()
        {
            var collection = new SymbolTableItemCollection();
            var internalCollection = GetInternalCollection(collection);
            var item = new ProgramNameItem
            {
                Lexeme = "teste",
            };

            collection.Add(item);
            Assert.IsTrue(internalCollection.Count == 1);
            Assert.AreEqual(internalCollection[0], item);
            collection.Remove(item);
            Assert.IsTrue(internalCollection.Count == 0);
        }

        [TestMethod]
        public void SEARCH_ITEM_TEST()
        {
            var collection = new SymbolTableItemCollection();
            var innerCollection = GetInternalCollection(collection);
            var item = new FunctionItem
            {
                Lexeme = "func"
            };

            collection.Add(item);
            Assert.IsTrue(innerCollection.Count == 1);
            Assert.AreEqual(innerCollection[0], item);

            var searchResult = collection.Search(item.Lexeme);
            
            Assert.AreEqual(searchResult[0], item);
        }

        [TestMethod]
        public void INDEXER_TEST()
        {
            var collection = new SymbolTableItemCollection();
            var item = new FunctionItem
            {
                Lexeme = "func"
            };

            collection.Add(item);
            Assert.AreEqual(collection[0], item);
        }

        private IList<SymbolTableItem> GetInternalCollection(SymbolTableItemCollection collection)
        {
            var fieldInfo = typeof(SymbolTableItemCollection)
                .GetField("_items", System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance);
            var innerCollection = fieldInfo.GetValue(collection) as IList<SymbolTableItem>;

            Assert.IsNotNull(innerCollection);
            return innerCollection;
        }
    }
}
